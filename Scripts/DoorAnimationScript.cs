using UnityEngine;

public class DoorAnimationScript : MonoBehaviour
{
    [SerializeField] GameObject lampRed;      // красная лампа
    [SerializeField] GameObject lampGreen;    // зеленая лампа

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip lockedSound;   // звук запертой двери
    [SerializeField] AudioClip unlockSound;   // звук открытия замка
    [SerializeField] AudioClip openSound;     // звук открытия двери
    [SerializeField] AudioClip closeSound;    // звук закрытия двери

    Animator animator;

    bool isOpen = false;
    bool isUnlocked = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        isOpen = false;
        isUnlocked = false;

        animator.SetBool("isOpen", false);

        if (lampRed != null) lampRed.SetActive(true);
        if (lampGreen != null) lampGreen.SetActive(false);
    }

    // ящик поставили на плиту
    public void unlockGate()
    {
        if (isUnlocked) return;

        isUnlocked = true;

        if (lampRed != null) lampRed.SetActive(false);
        if (lampGreen != null) lampGreen.SetActive(true);

        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);
    }

    // ящик убрали с плиты
    public void lockGate()
    {
        isUnlocked = false;

        closeGate();

        if (lampRed != null) lampRed.SetActive(true);
        if (lampGreen != null) lampGreen.SetActive(false);
    }

    // взаимодействие игрока
    public void interactWithDoor()
    {
        if (!isUnlocked)
        {
            if (audioSource != null && lockedSound != null)
                audioSource.PlayOneShot(lockedSound);

            return;
        }

        openGate();
    }

    public void openGate()
    {
        if (isOpen) return;

        isOpen = true;
        animator.SetBool("isOpen", true);

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);
    }

    public void closeGate()
    {
        if (!isOpen) return;

        isOpen = false;
        animator.SetBool("isOpen", false);

        if (audioSource != null && closeSound != null)
            audioSource.PlayOneShot(closeSound);
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}