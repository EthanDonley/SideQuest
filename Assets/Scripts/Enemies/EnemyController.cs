using Prime31;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}

public class EnemyController : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float detectionRange = 5f; // Horizontal range within which the enemy detects the player
    public float verticalDetectionRange = 2f; // Vertical range within which the enemy detects the player
    public float attackRange = 1f; // Range within which the enemy attacks the player
    public float chaseSpeed = 3.5f; // Speed of chasing
    public float attackCooldown = 1.5f; // Time between attacks
    public float flipThreshold = 0.1f; // Threshold to avoid constant flipping

    private float lastAttackTime = 0f;
    private Rigidbody2D rb;
    private CharacterController2D controller;
    private Vector3 originalScale;

    public EnemyState currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<CharacterController2D>();
        currentState = EnemyState.Idle;
        originalScale = transform.localScale;
    }

    void Update()
    {
        float horizontalDistanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float verticalDistanceToPlayer = Mathf.Abs(player.position.y - transform.position.y);

        switch (currentState)
        {
            case EnemyState.Idle:
                // Remain idle until player is detected
                if (horizontalDistanceToPlayer < detectionRange && verticalDistanceToPlayer < verticalDetectionRange && HasLineOfSight())
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                ChasePlayer();
                if (horizontalDistanceToPlayer < attackRange && verticalDistanceToPlayer < verticalDetectionRange)
                {
                    currentState = EnemyState.Attacking;
                }
                else if (horizontalDistanceToPlayer > detectionRange || verticalDistanceToPlayer > verticalDetectionRange || !HasLineOfSight())
                {
                    currentState = EnemyState.Idle;
                }
                break;

            case EnemyState.Attacking:
                AttackPlayer();
                if (horizontalDistanceToPlayer > attackRange || verticalDistanceToPlayer > verticalDetectionRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;
        }
    }

    void ChasePlayer()
    {
        // Move towards the player horizontally while maintaining the enemy's y position
        Vector2 targetPosition = new Vector2(player.position.x, rb.position.y);
        rb.position = Vector2.MoveTowards(rb.position, targetPosition, chaseSpeed * Time.deltaTime);

        // Flip the enemy based on the direction of movement only when necessary
        if (Mathf.Abs(targetPosition.x - transform.position.x) > flipThreshold)
        {
            if (targetPosition.x < transform.position.x && transform.localScale.x != -originalScale.x)
            {
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z); // Facing left
            }
            else if (targetPosition.x > transform.position.x && transform.localScale.x != originalScale.x)
            {
                transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z); // Facing right
            }
        }
    }

    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            Debug.Log("Enemy attacks the player!");
            lastAttackTime = Time.time;
            // Add your attack logic here (e.g., reducing player's health)
        }
    }

    bool HasLineOfSight()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange);
        if (hit.collider != null && hit.collider.CompareTag("Walls"))
        {
            return false;
        }

        // Ensure the player is within vertical detection range
        if (Mathf.Abs(player.position.y - transform.position.y) > verticalDetectionRange)
        {
            return false;
        }

        return true;
    }

    void FixedUpdate()
    {
        // Apply gravity and other physics-based movements
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionRange * 2, verticalDetectionRange * 2, 1));
    }
}