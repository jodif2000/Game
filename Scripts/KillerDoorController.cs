using UnityEngine;

public class KillerDoorController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip unlockSound;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip closeSound;
    [SerializeField] AudioClip lockSound;

    void Start()
    {
        if (animator != null)
            animator.SetBool("isOpen", false);
    }
    
    public void PlayUnlockSound()
    {
        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);
    }

    public void OpenDoor()
    {
        if (animator != null)
            animator.SetBool("isOpen", true);

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);
    }

    public void CloseDoor()
    {
        if (animator != null)
            animator.SetBool("isOpen", false);

        if (audioSource != null && closeSound != null)
            audioSource.PlayOneShot(closeSound);
    }

    public void PlayLockSound()
    {
        if (audioSource != null && lockSound != null)
            audioSource.PlayOneShot(lockSound);
    }
}