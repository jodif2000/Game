using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] PlayerWinHandler playerWinHandler;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerWinHandler != null)
            playerWinHandler.OnWin();
    }
}