using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStoneInteract : MonoBehaviour
{
    [Header("Поиск камня")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] LayerMask stoneLayerMask;

    [Header("Точка удержания")]
    [SerializeField] Transform holdPoint;

    [Header("Бросок")]
    [SerializeField] float throwForce = 8f;
    // [SerializeField] float returnDelay = 3f;

    [Header("Подсказка")]
    [SerializeField] GameObject interactPrompt;

    ThrowableObject currentStone;
    ThrowableObject heldStone;
    // ThrowableObject thrownStone;

    Rigidbody currentRigidbody;
    Collider[] currentColliders;

    // Coroutine returnCoroutine;

    bool isLookingAtStone;
    bool isHoldingStone;

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (isHoldingStone)
        {
            return;
        }

        ScanForStone();
    }

    private void LateUpdate()
    {
        if (!isHoldingStone) return;
        if (heldStone == null) return;
        if (holdPoint == null) return;

        heldStone.transform.position = holdPoint.position;
        heldStone.transform.rotation = holdPoint.rotation;
    }

    private void ScanForStone()
    {
        bool wasLookingAtStone = isLookingAtStone;

        isLookingAtStone = false;
        currentStone = null;

        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, stoneLayerMask, QueryTriggerInteraction.Ignore))
        {
            ThrowableObject stone = hit.collider.GetComponent<ThrowableObject>();

            if (stone == null)
            {
                stone = hit.collider.GetComponentInParent<ThrowableObject>();
            }

            if (stone != null)
            {
                isLookingAtStone = true;
                currentStone = stone;
            }
        }

        if (!wasLookingAtStone && isLookingAtStone)
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }

        if (wasLookingAtStone && !isLookingAtStone)
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (isHoldingStone) return;
        if (!isLookingAtStone) return;
        if (currentStone == null) return;
        if (holdPoint == null) return;

        PickUpStone();
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        if (!isHoldingStone) return;
        if (heldStone == null) return;

        ThrowStone();
    }

    private void PickUpStone()
    {
        heldStone = currentStone;
        currentStone = null;

        currentRigidbody = heldStone.GetComponent<Rigidbody>();
        currentColliders = heldStone.GetComponentsInChildren<Collider>();

        if (currentRigidbody != null)
        {
            currentRigidbody.linearVelocity = Vector3.zero;
            currentRigidbody.angularVelocity = Vector3.zero;

            currentRigidbody.useGravity = false;
            currentRigidbody.isKinematic = true;
            currentRigidbody.interpolation = RigidbodyInterpolation.None;
        }

        foreach (Collider stoneCollider in currentColliders)
        {
            stoneCollider.enabled = false;
        }

        Vector3 stoneWorldScale = heldStone.transform.lossyScale;

        heldStone.transform.SetParent(holdPoint, true);
        heldStone.transform.position = holdPoint.position;
        heldStone.transform.rotation = holdPoint.rotation;

        SetWorldScale(heldStone.transform, stoneWorldScale);

        isHoldingStone = true;
        isLookingAtStone = false;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
        
        SoundManager.PlaySound(SoundType.Stone_PickUp, 1f);
    }

    private void ThrowStone()
    {
        // thrownStone = heldStone;

        heldStone.transform.SetParent(null, true);

        if (currentRigidbody != null)
        {
            currentRigidbody.isKinematic = false;
            currentRigidbody.useGravity = true;
            currentRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            currentRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            currentRigidbody.linearVelocity = Vector3.zero;
            currentRigidbody.angularVelocity = Vector3.zero;
        }

        foreach (Collider stoneCollider in currentColliders)
        {
            stoneCollider.enabled = true;
        }

        Vector3 direction = cameraTransform.forward;

        if (currentRigidbody != null)
        {
            currentRigidbody.AddForce(direction * throwForce, ForceMode.Impulse);
        }
        
        SoundManager.PlaySound(SoundType.Stone_Throw, 1f);

        heldStone = null;
        currentRigidbody = null;
        currentColliders = null;
        isHoldingStone = false;

        // if (returnCoroutine != null)
        // {
        //     StopCoroutine(returnCoroutine);
        // }
        //
        // returnCoroutine = StartCoroutine(ReturnStoneAfterDelay(thrownStone));
    }

    // private IEnumerator ReturnStoneAfterDelay(ThrowableObject stone)
    // {
    //     yield return new WaitForSeconds(returnDelay);
    //
    //     if (stone != null)
    //     {
    //         stone.ReturnToRespawnPoint();
    //     }
    //
    //     thrownStone = null;
    //     returnCoroutine = null;
    // }

    private void SetWorldScale(Transform target, Vector3 worldScale)
    {
        if (target.parent == null)
        {
            target.localScale = worldScale;
            return;
        }

        Vector3 parentScale = target.parent.lossyScale;

        target.localScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );
    }

    private void OnDisable()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }
}