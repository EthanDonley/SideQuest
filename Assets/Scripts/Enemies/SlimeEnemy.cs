using UnityEngine;

public class SlimeEnemy : BaseEnemy
{
    public float chaseSpeed = 3.5f; // Speed of chasing

    protected override void Awake()
    {
        base.Awake();
        // Additional slime-specific setup
    }

    protected override void Update()
    {
        base.Update();
        // Additional slime-specific behavior
    }

    protected override void OnPlayerDetected()
    {
        currentState = EnemyState.Attacking;
        // Additional logic for when the player is detected
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (currentState == EnemyState.Attacking)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Vector2 targetPosition = new Vector2(player.position.x, rb.position.y);
        rb.position = Vector2.MoveTowards(rb.position, targetPosition, chaseSpeed * Time.deltaTime);

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
}