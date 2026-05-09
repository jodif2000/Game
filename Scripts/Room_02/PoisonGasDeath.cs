using System.Collections;
using UnityEngine;

public class PoisonGasDeath : MonoBehaviour
{
    [SerializeField] float deathDelay = 2f;
    [SerializeField] PlayerDeathHandler playerDeathHandler;

    Coroutine deathRoutine;

    public void StartDeathTimer()
    {
        if (deathRoutine != null) return;

        deathRoutine = StartCoroutine(DeathCountdown());
    }

    public void StopDeathTimer()
    {
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }
    }

    IEnumerator DeathCountdown()
    {
        yield return new WaitForSeconds(deathDelay);

        deathRoutine = null;

        if (playerDeathHandler != null)
            playerDeathHandler.OnDeath();
    }
}