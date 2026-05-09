using UnityEngine;
using UnityEngine.AI;

public class KillerAnimations : MonoBehaviour
{
    [SerializeField] Animator animator;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        animator.SetFloat("speed", agent.velocity.magnitude);
    }
}