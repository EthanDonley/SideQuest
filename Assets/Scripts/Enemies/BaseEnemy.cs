using Prime31;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Attacking,
    Chasing
}

public abstract class BaseEnemy : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float detectionRange = 5f; // Horizontal range within which the enemy detects the player
    public float verticalDetectionRange = 2f; // Vertical range within which the enemy detects the player
    public float attackRange = 1f; // Range within which the enemy attacks the player
    public float attackCooldown = 1.5f; // Time between attacks
    public float flipThreshold = 0.1f; // Threshold to avoid constant flipping

    protected float lastAttackTime = 0f;
    protected Rigidbody2D rb;
    protected Vector3 originalScale;
    protected CharacterController2D controller;
    public EnemyState currentState;

    public float maxHealth = 100f;
    private float currentHealth;
    public float attackDamage = 10f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<CharacterController2D>();
        currentState = EnemyState.Idle;
        originalScale = transform.localScale;
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        float horizontalDistanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float verticalDistanceToPlayer = Mathf.Abs(player.position.y - transform.position.y);

        switch (currentState)
        {
            case EnemyState.Idle:
                if (horizontalDistanceToPlayer < detectionRange && verticalDistanceToPlayer < verticalDetectionRange && HasLineOfSight())
                {
                    OnPlayerDetected();
                }
                break;

            case EnemyState.Attacking:
                AttackPlayer();
                if (horizontalDistanceToPlayer > attackRange || verticalDistanceToPlayer > verticalDetectionRange)
                {
                    currentState = EnemyState.Idle;
                }
                break;
        }
    }

    protected virtual void OnPlayerDetected()
    {
        // Override this in derived classes to handle player detection
    }

    protected virtual void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            Debug.Log("Enemy attacks the player!");
            lastAttackTime = Time.time;
            PerformAttack(player.gameObject);
        }
    }

    protected bool HasLineOfSight()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange);
        if (hit.collider != null && hit.collider.CompareTag("Walls"))
        {
            return false;
        }

        if (Mathf.Abs(player.position.y - transform.position.y) > verticalDetectionRange)
        {
            return false;
        }

        return true;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    public void PerformAttack(GameObject target)
    {
        /*Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
        }*/
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionRange * 2, verticalDetectionRange * 2, 1));
    }
}
