using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimations : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float speed = agent.velocity.magnitude;

        // если враг движется — включается ходьба
        animator.SetBool("isWalking", speed > 0.1f);
    }

    public void attack()
    {
        animator.SetTrigger("attack");
    }
}