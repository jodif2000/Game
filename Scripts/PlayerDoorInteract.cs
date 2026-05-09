using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDoorInteract : MonoBehaviour
{
    [Header("Поиск двери")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] float interactDistance = 3f;
    [SerializeField] LayerMask doorLayerMask;

    [Header("Подсказка")]
    [SerializeField] GameObject interactPrompt;

    DoorAnimationScript currentDoor;
    SimpleLockedDoorScript currentLockedDoor;
    bool isLookingAtDoor;

    private void Update()
    {
        ScanForDoor();
    }

    private void ScanForDoor()
    {
        bool wasLookingAtDoor = isLookingAtDoor;

        isLookingAtDoor = false;
        currentDoor = null;
        currentLockedDoor = null;

        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, doorLayerMask, QueryTriggerInteraction.Ignore))
        {
            DoorAnimationScript animatedDoor = hit.collider.GetComponent<DoorAnimationScript>();

            if (animatedDoor == null)
                animatedDoor = hit.collider.GetComponentInParent<DoorAnimationScript>();

            if (animatedDoor != null)
            {
                isLookingAtDoor = true;
                currentDoor = animatedDoor;
                currentLockedDoor = null;
            }
            else
            {
                SimpleLockedDoorScript lockedDoor = hit.collider.GetComponent<SimpleLockedDoorScript>();

                if (lockedDoor == null)
                    lockedDoor = hit.collider.GetComponentInParent<SimpleLockedDoorScript>();

                if (lockedDoor != null)
                {
                    isLookingAtDoor = true;
                    currentLockedDoor = lockedDoor;
                    currentDoor = null;
                }
            }
        }

        if (!wasLookingAtDoor && isLookingAtDoor)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }

        if (wasLookingAtDoor && !isLookingAtDoor)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    private void OnInteract(InputValue _)
    {
        if (!isLookingAtDoor) return;

        if (currentDoor != null)
        {
            currentDoor.interactWithDoor();
            return;
        }

        if (currentLockedDoor != null)
        {
            currentLockedDoor.interactWithDoor();
        }
    }

    private void OnDisable()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}