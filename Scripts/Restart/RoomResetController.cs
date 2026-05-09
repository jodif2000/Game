using UnityEngine;
using UnityEngine.AI;

public class RoomResetController : MonoBehaviour
{
    [Header("Ящики комнаты")]
    [SerializeField] Transform[] boxes;

    [Header("Сброс двери")]
    [SerializeField] DoorAnimationScript door;

    [Header("Сброс пазла")]
    [SerializeField] BoxOrderPuzzle puzzle;

    [Header("Дым")]
    [SerializeField] GameObject poisonGas;

    [Header("Сброс мясника")]
    [SerializeField] Transform butcher;
    [SerializeField] Transform butcherStartPoint;
    [SerializeField] Room03StartTrigger room03StartTrigger;

    Vector3[] startPositions;
    Quaternion[] startRotations;
    Rigidbody[] boxRigidbodies;

    Rigidbody butcherRigidbody;
    NavMeshAgent butcherAgent;
    Animator butcherAnimator;
    AudioSource butcherAudioSource;

    KillerAttackLogic butcherAttackLogic;
    KillerRoutineController butcherRoutineController;

    MonoBehaviour[] butcherScripts;

    private void Start()
    {
        startPositions = new Vector3[boxes.Length];
        startRotations = new Quaternion[boxes.Length];
        boxRigidbodies = new Rigidbody[boxes.Length];

        for (int i = 0; i < boxes.Length; i++)
        {
            startPositions[i] = boxes[i].position;
            startRotations[i] = boxes[i].rotation;
            boxRigidbodies[i] = boxes[i].GetComponent<Rigidbody>();
        }

        if (butcher != null)
        {
            butcherRigidbody = butcher.GetComponent<Rigidbody>();
            butcherAgent = butcher.GetComponent<NavMeshAgent>();
            butcherAnimator = butcher.GetComponent<Animator>();
            butcherAudioSource = butcher.GetComponent<AudioSource>();

            butcherAttackLogic = butcher.GetComponent<KillerAttackLogic>();
            butcherRoutineController = butcher.GetComponent<KillerRoutineController>();

            butcherScripts = butcher.GetComponents<MonoBehaviour>();
        }
    }

    public void ResetRoom()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxRigidbodies[i] != null)
            {
                boxRigidbodies[i].linearVelocity = Vector3.zero;
                boxRigidbodies[i].angularVelocity = Vector3.zero;
                boxRigidbodies[i].position = startPositions[i];
                boxRigidbodies[i].rotation = startRotations[i];
            }
            else
            {
                boxes[i].position = startPositions[i];
                boxes[i].rotation = startRotations[i];
            }
        }

        if (poisonGas != null)
            poisonGas.SetActive(false);

        if (door != null)
            door.lockGate();

        if (puzzle != null)
            puzzle.ResetPuzzle();

        ResetButcher();
    }

    void ResetButcher()
    {
        if (butcher == null || butcherStartPoint == null)
            return;

        // 1. Выключаем все скрипты мясника
        if (butcherScripts != null)
        {
            for (int i = 0; i < butcherScripts.Length; i++)
            {
                if (butcherScripts[i] != null)
                    butcherScripts[i].enabled = false;
            }
        }

        // 2. Останавливаем агент
        if (butcherAgent != null)
        {
            if (butcherAgent.enabled)
            {
                butcherAgent.isStopped = true;
                butcherAgent.ResetPath();
                butcherAgent.enabled = false;
            }
        }

        // 3. Останавливаем физику
        if (butcherRigidbody != null)
        {
            butcherRigidbody.linearVelocity = Vector3.zero;
            butcherRigidbody.angularVelocity = Vector3.zero;
        }

        // 4. Останавливаем звук
        if (butcherAudioSource != null)
            butcherAudioSource.Stop();

        // 5. Возвращаем на стартовую позицию
        butcher.position = butcherStartPoint.position;
        butcher.rotation = butcherStartPoint.rotation;

        // 6. Сбрасываем аниматор
        if (butcherAnimator != null)
        {
            butcherAnimator.Rebind();
            butcherAnimator.Update(0f);
        }

        // 7. Включаем агент обратно
        if (butcherAgent != null)
        {
            butcherAgent.enabled = true;
            butcherAgent.Warp(butcherStartPoint.position);
            butcherAgent.isStopped = true;
            butcherAgent.ResetPath();
        }

        // 8. Сбрасываем стартовый триггер комнаты
        if (room03StartTrigger != null)
            room03StartTrigger.ResetTrigger();

        // 9. Включаем все скрипты мясника обратно
        if (butcherScripts != null)
        {
            for (int i = 0; i < butcherScripts.Length; i++)
            {
                if (butcherScripts[i] != null)
                    butcherScripts[i].enabled = true;
            }
        }

        // 10. После сброса мясник не должен сразу бежать
        if (butcherAgent != null)
        {
            butcherAgent.isStopped = true;
            butcherAgent.ResetPath();
        }

        if (butcherAttackLogic != null)
            butcherAttackLogic.ResetAttack();

        if (butcherRoutineController != null)
            butcherRoutineController.ResetRoutine();
    }
}