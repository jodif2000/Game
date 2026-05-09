using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KillerRoutineController : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] KillerDoorController doorController;

    [Header("Стартовая точка")]
    [SerializeField] Transform startPoint;

    [Header("Точки маршрута")]
    [SerializeField] Transform idlePoint;
    [SerializeField] Transform doorPoint;
    [SerializeField] Transform inspectPoint;
    [SerializeField] Transform returnPoint;

    [Header("Тайминги")]
    [SerializeField] float idleTime = 15f;
    [SerializeField] float unlockTime = 5f;
    [SerializeField] float inspectTime = 5f;
    [SerializeField] float lockTime = 5f;

    bool routineStarted = false;
    Coroutine routineCoroutine;

    void Start()
    {
        SetWalking(false);

        if (agent != null)
            agent.isStopped = true;
    }

    public void StartRoutine()
    {
        if (routineStarted) return;

        routineStarted = true;

        if (agent != null)
            agent.isStopped = false;

        routineCoroutine = StartCoroutine(RoutineLoop());
    }

    public void ResetRoutine()
    {
        routineStarted = false;

        if (routineCoroutine != null)
        {
            StopCoroutine(routineCoroutine);
            routineCoroutine = null;
        }

        StopAllCoroutines();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        // Телепорт назад за дверь
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;

            if (agent != null)
                agent.Warp(startPoint.position);
        }

        SetWalking(false);

        if (doorController != null)
            doorController.CloseDoor();
    }

    IEnumerator RoutineLoop()
    {
        while (true)
        {
            SetWalking(false);
            yield return new WaitForSeconds(idleTime);

            yield return StartCoroutine(MoveToPoint(doorPoint.position));

            SetWalking(false);
            doorController.PlayUnlockSound();
            yield return new WaitForSeconds(unlockTime);

            doorController.OpenDoor();
            yield return new WaitForSeconds(0.5f);

            yield return StartCoroutine(MoveToPoint(inspectPoint.position));

            SetWalking(false);
            yield return new WaitForSeconds(inspectTime);

            yield return StartCoroutine(MoveToPoint(returnPoint.position));

            doorController.CloseDoor();
            yield return new WaitForSeconds(0.5f);

            SetWalking(false);
            doorController.PlayLockSound();
            yield return new WaitForSeconds(lockTime);

            yield return StartCoroutine(MoveToPoint(idlePoint.position));
        }
    }

    IEnumerator MoveToPoint(Vector3 target)
    {
        if (agent == null) yield break;

        agent.isStopped = false;
        agent.SetDestination(target);
        SetWalking(true);

        while (agent.pathPending)
            yield return null;

        while (agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        agent.isStopped = true;
        SetWalking(false);
    }

    void SetWalking(bool value)
    {
        if (animator != null)
            animator.SetBool("isWalking", value);
    }
}