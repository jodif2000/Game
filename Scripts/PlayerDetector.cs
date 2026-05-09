using System.Collections;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] [Range(1.0f, 30.0f)] float detectionRadius = 5f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask obstacleLayer;

    Vector3 lastPlayerPosition;
    bool detected = false;
    float distanceToPlayer;

    public Vector3 LastPlayerPosition { get { return lastPlayerPosition; } }
    public bool PlayerDetected { get { return detected; } }
    public float DistanceToPlayer { get { return distanceToPlayer; } }

    void Start()
    {
        StartCoroutine(detectPlayer());
    }

    IEnumerator detectPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

            if (detected == true)
            {
                if (Vector3.Distance(lastPlayerPosition, transform.position) < 1.0f)
                    detected = false;
            }

            if (colliders.Length > 0)
            {
                distanceToPlayer = Vector3.Distance(transform.position, colliders[0].transform.position);
                Vector3 direction = colliders[0].transform.position - transform.position;

                if (Physics.Raycast(transform.position, direction, distanceToPlayer, obstacleLayer) == false)
                {
                    detected = true;
                    lastPlayerPosition = colliders[0].transform.position;
                }
            }
        }
    }

    public void ResetDetector()
    {
        detected = false;
    }

    public GameObject proximityCheck(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, playerLayer);

        if (colliders.Length > 0)
            return colliders[0].gameObject;

        return null;
    }
}