// using UnityEngine;
//
// public class ThrowableObject : MonoBehaviour
// {
//     [Header("Точка возврата")]
//     [SerializeField] private Transform respawnPoint;
//
//     private Rigidbody rb;
//     private Collider[] colliders;
//
//     private bool canReturn = true;
//
//     private void Awake()
//     {
//         rb = GetComponent<Rigidbody>();
//         colliders = GetComponentsInChildren<Collider>();
//     }
//
//     public void DisableReturn()
//     {
//         canReturn = false;
//     }
//
//     public void EnableReturn()
//     {
//         canReturn = true;
//     }
//
//     public void ReturnToRespawnPoint()
//     {
//         if (!canReturn) return;
//         if (respawnPoint == null) return;
//
//         transform.SetParent(null);
//
//         if (rb != null)
//         {
//             rb.linearVelocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//
//             rb.useGravity = false;
//             rb.isKinematic = true;
//         }
//
//         foreach (Collider objectCollider in colliders)
//         {
//             objectCollider.enabled = true;
//         }
//
//         transform.position = respawnPoint.position;
//         transform.rotation = respawnPoint.rotation;
//
//         if (rb != null)
//         {
//             rb.linearVelocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//             rb.Sleep();
//         }
//     }
//     
//     public void HideAfterHit()
//     {
//         if (rb != null)
//         {
//             rb.linearVelocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//             rb.useGravity = false;
//             rb.isKinematic = true;
//         }
//
//         gameObject.SetActive(false);
//     }
//     
//     private bool canPlayFallSound = true;
//
//     private void OnCollisionEnter(Collision collision)
//     {
//         if (!canPlayFallSound) return;
//
//         if (collision.relativeVelocity.magnitude > 1.5f)
//         {
//             SoundManager.PlaySound(SoundType.Stone_Fall, 0.8f);
//             canPlayFallSound = false;
//             Invoke(nameof(ResetFallSound), 0.3f);
//         }
//     }
//
//     private void ResetFallSound()
//     {
//         canPlayFallSound = true;
//     }
// }

using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    [Header("Звук удара")]
    [SerializeField] private float minHitVelocity = 1.5f;
    [SerializeField] private float hitSoundCooldown = 0.3f;
    [SerializeField] private float hitSoundVolume = 0.8f;

    private Rigidbody rb;
    private bool canPlayHitSound = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canPlayHitSound) return;

        if (collision.relativeVelocity.magnitude < minHitVelocity) return;

        SoundManager.PlaySound(SoundType.Stone_Fall, hitSoundVolume);

        canPlayHitSound = false;
        Invoke(nameof(ResetHitSound), hitSoundCooldown);
    }

    private void ResetHitSound()
    {
        canPlayHitSound = true;
    }

    public void StopStone()
    {
        if (rb == null) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}