using UnityEngine;
using UnityEngine.AI;

public class BossMelee : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int damage = 20;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    private float lastAttackTime;

    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isDead) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            StopAndAttack();
        }
        else if (dist <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        animator.SetBool("Walk", true);
        animator.SetBool("Attack", false);
    }

    void StopAndAttack()
    {
        agent.isStopped = true;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        animator.SetBool("Walk", false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // This method is called from an ANIMATION EVENT
  

    void Idle()
    {
        agent.isStopped = true;
        animator.SetBool("Walk", false);
        animator.SetBool("Attack", false);
    }

    public void Die()
    {
        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("Die");
    }
}
