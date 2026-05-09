using UnityEngine;

public class Room03StartTrigger : MonoBehaviour
{
    [SerializeField] KillerRoutineController killerRoutine;

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (killerRoutine != null)
            killerRoutine.StartRoutine();
    }

    public void ResetTrigger()
    {
        triggered = false;
    }
}