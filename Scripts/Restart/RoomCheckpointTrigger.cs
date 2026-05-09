using UnityEngine;

public class RoomCheckpointTrigger : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] RoomResetController roomToReset;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Checkpoint enter: " + gameObject.name);

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.SetCheckpoint(respawnPoint, roomToReset);
    }
}