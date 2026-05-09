using UnityEngine;
using UnityEngine.AI;

public class KillerAttackLogic : MonoBehaviour
{
    [SerializeField] float catchRadius = 1.8f;
    [SerializeField] float runSpeed = 3.0f;

    [SerializeField] PlayerDetector detector;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] PlayerDisableController playerDisableController;
    [SerializeField] PlayerDeathHandler playerDeathHandler;
    [SerializeField] AudioSource catchAudio;
    [SerializeField] KillerRoutineController routineController;

    bool started = false;

    void Update()
    {
        if (detector == null || agent == null) return;

        if (detector.PlayerDetected)
        {
            if (!started)
            {
                started = true;

                // отключаем патруль
                if (routineController != null)
                {
                    routineController.StopAllCoroutines();
                    routineController.enabled = false;
                }

                // блокируем игрока
                if (playerDisableController != null)
                    playerDisableController.DisablePlayer();

                // запускаем звук
                if (catchAudio != null)
                {
                    catchAudio.Play();

                    // завершение игры ровно после окончания звука
                    Invoke(nameof(FinishGame), catchAudio.clip.length);
                }
            }

            agent.isStopped = false;
            agent.speed = runSpeed;

            if (detector.DistanceToPlayer > catchRadius)
            {
                Vector3 playerPos = detector.LastPlayerPosition;
                Vector3 dir = (transform.position - playerPos).normalized;
                Vector3 targetPoint = playerPos + dir * catchRadius;

                agent.SetDestination(targetPoint);
            }
            else
            {
                agent.isStopped = true;

                if (playerDisableController != null)
                    playerDisableController.FaceKiller();
            }
        }
    }

    void FinishGame()
    {
        if (playerDeathHandler != null)
            playerDeathHandler.KilledByKiller();
    }

    public void ResetAttack()
    {
        started = false;
        CancelInvoke(nameof(FinishGame));

        if (detector != null) detector.ResetDetector();

        if (agent != null)
        {
            agent.speed = runSpeed;
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
}