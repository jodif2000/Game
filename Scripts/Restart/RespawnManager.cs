using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    Transform currentRespawnPoint;
    RoomResetController currentRoom;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetCheckpoint(Transform respawnPoint, RoomResetController room)
    {
        currentRespawnPoint = respawnPoint;
        currentRoom = room;

        Debug.Log("NEW CHECKPOINT: " + respawnPoint.name + " | room: " + room.name);
    }

    public void RespawnPlayer(GameObject player, PlayerDeathHandler deathHandler)
    {
        Debug.Log("RESPAWN START");
        Debug.Log("currentRespawnPoint = " + (currentRespawnPoint != null ? currentRespawnPoint.name : "NULL"));
        Debug.Log("currentRoom = " + (currentRoom != null ? currentRoom.name : "NULL"));

        Time.timeScale = 1f;

        if (currentRoom != null)
            currentRoom.ResetRoom();

        if (currentRespawnPoint != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            player.transform.position = currentRespawnPoint.position + Vector3.up * 0.2f;
            player.transform.rotation = currentRespawnPoint.rotation;

            if (controller != null)
                controller.enabled = true;
        }

        PlayerDisableController disableController = player.GetComponent<PlayerDisableController>();
        if (disableController != null)
            disableController.EnablePlayer();

        deathHandler.ResetAfterDeath();
    }
}