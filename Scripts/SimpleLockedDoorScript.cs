using UnityEngine;

public class SimpleLockedDoorScript : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip lockedSound;

    public void interactWithDoor()
    {
        if (audioSource != null && lockedSound != null)
            audioSource.PlayOneShot(lockedSound);
    }
}