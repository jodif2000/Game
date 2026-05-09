using UnityEngine;
using UnityEngine.Events;

public class LeverHit : MonoBehaviour
{
    [Header("Аниматор рычага")]
    [SerializeField] private Animator leverAnimator;

    [Header("Параметр активации")]
    [SerializeField] private string activatedParameter = "isActivated";

    [Header("Событие при активации")]
    [SerializeField] private UnityEvent onActivated;

    private bool isActivated;
    private Collider leverCollider;

    private void Awake()
    {
        if (leverAnimator == null)
        {
            leverAnimator = GetComponentInParent<Animator>();
        }

        leverCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryActivate(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryActivate(other);
    }

    private void TryActivate(Collider other)
    {
        if (isActivated) return;

        ThrowableObject stone = other.GetComponentInParent<ThrowableObject>();

        if (stone == null) return;

        isActivated = true;
        SoundManager.PlaySound(SoundType.Lever_Activated, 1f);

        if (leverAnimator != null)
        {
            leverAnimator.SetBool(activatedParameter, true);
        }

        onActivated.Invoke();

        // stone.DisableReturn();
        // stone.HideAfterHit();

        if (leverCollider != null)
        {
            leverCollider.enabled = false;
        }
    }
}