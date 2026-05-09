using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDisableController : MonoBehaviour
{
    [SerializeField] MonoBehaviour movementScript;
    [SerializeField] MonoBehaviour rotationScript;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Transform playerBody;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform killer;

    bool disabled = false;

    public void DisablePlayer()
    {
        if (disabled) return;

        disabled = true;

        if (movementScript != null)
            movementScript.enabled = false;

        if (rotationScript != null)
            rotationScript.enabled = false;

        if (playerInput != null)
            playerInput.enabled = false;
    }

    public void EnablePlayer()
    {
        disabled = false;

        if (movementScript != null)
            movementScript.enabled = true;

        if (rotationScript != null)
            rotationScript.enabled = true;

        if (playerInput != null)
            playerInput.enabled = true;
    }

    public void FaceKiller()
    {
        if (killer == null || playerBody == null) return;

        Vector3 direction = killer.position - playerBody.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
            playerBody.rotation = Quaternion.LookRotation(direction);

        if (playerCamera != null)
            playerCamera.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}