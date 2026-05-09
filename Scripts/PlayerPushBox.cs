using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerPushBox : MonoBehaviour
{
    [Header("Поиск ящика")] [SerializeField]
    Transform cameraTransform; // ссылка на камеру

    [SerializeField] float interactDistance = 3f; // дистанция поиска ящика
    [SerializeField] LayerMask boxLayerMask; // слой Box

    [SerializeField] PlayerRotation rotationScript; // скрипт вращения игрока

    [Header("Толкание")] [SerializeField] float playerDistanceFromBox = 1.3f; // расстояние от центра ящика до игрока
    [SerializeField] float pushSpeed = 1.0f; // обычная скорость толкания/тяги
    [SerializeField] float wallCheckDistance = 0.6f; // длина рейкаста перед ящиком
    [SerializeField] LayerMask obstacleLayerMask; // слои препятствий (Wall, Pillar)

    [Header("Физическое давление (шкала)")] [SerializeField]
    RectTransform pressureBarRect; // RectTransform шкалы (высота)

    [SerializeField] RectTransform pressureIndicator; // RectTransform индикатора (двигаем по Y)

    [SerializeField] float pressAdd = 0.10f; // сколько добавляет один клик (0..1)
    [SerializeField] float fallSpeed = 0.70f; // скорость падения индикатора
    [SerializeField] float lockFallSpeed = 2.50f; // скорость падения при блокировке
    [SerializeField] float pressActiveTime = 0.25f; // сколько времени после клика разрешаем движение

    public bool IsPushing { get; private set; }
    public bool IsLookingAtBox { get; private set; }

    public UnityEvent OnLookAtBox;
    public UnityEvent OnLookAwayFromBox;
    public UnityEvent OnGrab;
    public UnityEvent OnRelease;

    CharacterController controller;

    Rigidbody boxRb;
    Transform boxTr;
    Collider boxCol;
    RaycastHit currentHit;

    Vector3 pushAxis;
    Vector3 playerOffset;

    float input; // ось W/S из OnMove

    float pressure01; // 0..1
    float redTimer; // таймер красной зоны
    bool isLocked; // блокировка
    float lastPressTime; // время последнего клика
    int lastDir; // 1 = вперёд, -1 = назад, 0 = нет

    const float YELLOW_END = 2f / 3f;
    const float GREEN_END = (2f / 3f) + (1f / 4f); // 11/12

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!IsPushing)
        {
            ScanForBox();
            return;
        }

        UpdatePressure();
        UpdatePressureUI();
        FollowBox();
    }

    private void FixedUpdate()
    {
        if (!IsPushing || boxRb == null) return;

        if (isLocked)
        {
            boxRb.linearVelocity = Vector3.zero;
            return;
        }

        bool pressIsFresh = (Time.time - lastPressTime) <= pressActiveTime;
        if (!pressIsFresh)
        {
            boxRb.linearVelocity = Vector3.zero;
            return;
        }

        float signedInput = Mathf.Abs(input) > 0.01f ? Mathf.Sign(input) : 0f;
        if (Mathf.Abs(signedInput) < 0.01f)
        {
            boxRb.linearVelocity = Vector3.zero;
            return;
        }

        float speedScale = GetSpeedScaleByPressure(pressure01);
        if (speedScale <= 0.001f)
        {
            boxRb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 vel = pushAxis * (signedInput * pushSpeed * speedScale);

        // проверка стены
        if (boxCol != null)
        {
            Vector3 dir = pushAxis * Mathf.Sign(signedInput);
            Vector3 origin = boxRb.worldCenterOfMass;

            float ext = boxCol.bounds.extents.magnitude * 0.9f;
            float dist = ext + wallCheckDistance;

            if (Physics.Raycast(origin, dir, dist, obstacleLayerMask, QueryTriggerInteraction.Ignore))
                vel = Vector3.zero;
        }

        boxRb.linearVelocity = new Vector3(vel.x, 0f, vel.z); // Y не трогаем
    }

    private void ScanForBox()
    {
        bool wasLooking = IsLookingAtBox;

        IsLookingAtBox = false;
        boxRb = null;
        boxTr = null;
        boxCol = null;

        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, boxLayerMask, QueryTriggerInteraction.Ignore))
        {
            currentHit = hit;

            if (hit.collider.CompareTag("Box"))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null)
                {
                    IsLookingAtBox = true;
                    boxRb = rb;
                    boxTr = rb.transform;
                    boxCol = hit.collider;
                }
            }
        }

        if (!wasLooking && IsLookingAtBox) OnLookAtBox?.Invoke();
        if (wasLooking && !IsLookingAtBox) OnLookAwayFromBox?.Invoke();
    }

    private void OnInteract(InputValue _)
    {
        if (IsPushing)
        {
            Release();
            return;
        }

        if (IsLookingAtBox && boxRb != null && CanGrabBox(currentHit))
            Grab();
    }

    private void Grab()
    {
        IsPushing = true;
        input = 0f;

        boxRb.isKinematic = false;
        boxRb.useGravity = false;
        boxRb.constraints = RigidbodyConstraints.FreezeRotation;
        boxRb.interpolation = RigidbodyInterpolation.Interpolate;

        boxRb.linearVelocity = Vector3.zero;
        boxRb.angularVelocity = Vector3.zero;

        ComputeGrabPose();
        
        if (rotationScript != null)
        {
            rotationScript.SetRotationEnabled(false);
            rotationScript.ResetVerticalLook(); // камера смотрит прямо
        }

        pressure01 = 0f;
        redTimer = 0f;
        isLocked = false;
        lastPressTime = -999f;
        lastDir = 0;

        if (rotationScript != null)
            rotationScript.SetRotationEnabled(false);

        OnGrab?.Invoke();
    }

    private void Release()
    {
        if (boxRb != null)
        {
            boxRb.linearVelocity = Vector3.zero;
            boxRb.angularVelocity = Vector3.zero;

            boxRb.isKinematic = true;
            boxRb.useGravity = false;
            boxRb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        IsPushing = false;
        input = 0f;

        boxRb = null;
        boxTr = null;
        boxCol = null;

        IsLookingAtBox = false; // важно: чтобы OnLookAtBox гарантированно сработал снова

        if (rotationScript != null)
            rotationScript.SetRotationEnabled(true);

        OnRelease?.Invoke();
    }

    private void ComputeGrabPose()
    {
        Vector3 toPlayer = transform.position - boxTr.position;
        toPlayer.y = 0f;

        Vector3 localDir = boxTr.InverseTransformDirection(toPlayer.normalized);

        Vector3 snapped;
        if (Mathf.Abs(localDir.x) > Mathf.Abs(localDir.z))
            snapped = new Vector3(Mathf.Sign(localDir.x), 0f, 0f);
        else
            snapped = new Vector3(0f, 0f, Mathf.Sign(localDir.z));

        Vector3 faceNormal = boxTr.TransformDirection(snapped);
        faceNormal.y = 0f;
        faceNormal.Normalize();

        pushAxis = -faceNormal;
        playerOffset = faceNormal * playerDistanceFromBox;

        transform.rotation = Quaternion.LookRotation(pushAxis);
    }

    private void FollowBox()
    {
        Vector3 target = boxTr.position + playerOffset;
        target.y = transform.position.y;

        Vector3 delta = target - transform.position;
        delta.y = 0f;

        controller.Move(delta);
    }

    private void OnMove(InputValue value)
    {
        if (!IsPushing) return;
        input = value.Get<Vector2>().y;
    }

    private void OnPressForward(InputValue _)
    {
        if (!IsPushing) return;
        RegisterPress(1);
    }

    private void OnPressBack(InputValue _)
    {
        if (!IsPushing) return;
        RegisterPress(-1);
    }

    private void RegisterPress(int dir) // если сменилось направление — сбрасываем давление
    {
        if (lastDir != 0 && dir != lastDir)
        {
            pressure01 = 0f;
            redTimer = 0f;
        }

        lastDir = dir;
        lastPressTime = Time.time;
        pressure01 = Mathf.Clamp01(pressure01 + pressAdd);
    }

    private void UpdatePressure() // Обновление логики шкалы давления
    {
        if (isLocked)
        {
            pressure01 = Mathf.MoveTowards(pressure01, 0f, lockFallSpeed * Time.deltaTime);

            if (pressure01 <= 0.0001f)
            {
                pressure01 = 0f;
                isLocked = false;
                redTimer = 0f;
                lastDir = 0;
            }

            return;
        }

        bool pressIsFresh = (Time.time - lastPressTime) <= pressActiveTime;
        if (!pressIsFresh)
            pressure01 = Mathf.MoveTowards(pressure01, 0f, fallSpeed * Time.deltaTime);

        if (pressure01 >= GREEN_END)
        {
            redTimer += Time.deltaTime;
            if (redTimer >= 0.1f)
            {
                isLocked = true;
                input = 0f;
            }
        }
        else
        {
            redTimer = 0f;
        }
    }

    private float GetSpeedScaleByPressure(float p01) // Расчёт коэффициента скорости по шкале давления
    {
        if (p01 < YELLOW_END)
        {
            float t = Mathf.InverseLerp(0f, YELLOW_END, p01);
            return Mathf.Clamp01(t);
        }

        if (p01 < GREEN_END)
            return 1f;

        return 1f;
    }

    private void UpdatePressureUI() // Обновление UI-индикатора
    {
        if (pressureIndicator == null || pressureBarRect == null) return;

        float h = pressureBarRect.rect.height;

        Vector2 pos = pressureIndicator.anchoredPosition;
        pos.y = pressure01 * h;
        pressureIndicator.anchoredPosition = pos;
    }
    
    bool CanGrabBox(RaycastHit hit)
    {
        if (boxCol == null)
            return false;

        Bounds bounds = boxCol.bounds;

        // если игрок стоит сверху на ящике
        if (transform.position.y > bounds.max.y - 0.05f)
            return false;

        // если смотрим в верхнюю часть ящика
        float hitHeight = hit.point.y;
        float topLimit = bounds.max.y - 0.05f;

        if (hitHeight >= topLimit)
            return false;

        return true;
    }
}