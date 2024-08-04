using Prime31;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    enum PlayerDirection { Right, Left }
    private PlayerDirection direction = PlayerDirection.Right;

    [Header("Jump")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    [Header("Colliders")]
    public BoxCollider2D physicsCollider;
    public Vector2 defaultColliderSize;
    public Vector2 defaultColliderOffset;

    [Header("Audio")]
    public AudioClip jumpSound;
    private AudioSource audioSource;

    [Header("Inputs")]
    private bool jumpPressed = false;
    private bool jumpReleased = false;
    private bool lockInput = false;

    private CharacterController2D controller;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    private Vector3 velocity;
    private float normalizedHorizontalSpeed = 0;
    private bool wasGroundedLastFrame = false;

    void Awake()
    {
        controller = GetComponent<CharacterController2D>();

        controller.onControllerCollidedEvent += OnControllerCollider;
        controller.onTriggerEnterEvent += OnTriggerEnterEvent;
        controller.onTriggerExitEvent += OnTriggerExitEvent;

        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponentInChildren<AudioSource>();

        PlayerStats.Instance.ResetHealth(); // Ensure health is at max at the start
    }

    #region Event Listeners

    void OnControllerCollider(RaycastHit2D hit)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            if (Mathf.Abs(hit.normal.x) > 0)
            {
                velocity.x = 0;
            }
        }
    }

    void OnTriggerEnterEvent(Collider2D collision)
    {
        Debug.Log("Trigger enter: " + collision.gameObject.name);
    }

    void OnTriggerExitEvent(Collider2D col)
    {
        Debug.Log("Trigger exit: " + col.gameObject.name);
    }

    #endregion

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpReleased = true;
        }
    }

    void FixedUpdate()
    {
        if (controller.isGrounded)
        {
            velocity.y = 0;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        if (velocity.y < 0)
            velocity.y *= PlayerStats.Instance.fallSpeedModifier;

        HandleHorizontal();
        HandleJump();
        ClampFallSpeed();

        controller.move(velocity * Time.deltaTime);

        velocity = controller.velocity;
        jumpPressed = false;
        jumpReleased = false;

        wasGroundedLastFrame = controller.isGrounded;
    }

    void HandleHorizontal()
    {
        float input = Input.GetAxis("Horizontal");

        if (Mathf.Abs(input) > 0.1f)
        {
            direction = input > 0 ? PlayerDirection.Right : PlayerDirection.Left;
            normalizedHorizontalSpeed = Mathf.Sign(input);

            if ((direction == PlayerDirection.Right && transform.localScale.x < 0) ||
                (direction == PlayerDirection.Left && transform.localScale.x > 0))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            normalizedHorizontalSpeed = 0;
            velocity.x = 0;
        }

        var smoothedMovementFactor = controller.isGrounded ? PlayerStats.Instance.groundDamping : PlayerStats.Instance.inAirDamping;
        velocity.x = Mathf.Lerp(velocity.x, normalizedHorizontalSpeed * PlayerStats.Instance.runSpeed, Time.deltaTime * smoothedMovementFactor);
    }

    void HandleJump()
    {
        if (jumpPressed && coyoteTimeCounter > 0f)
        {
            Jump();
        }

        if (velocity.y < 0)
            velocity.y += PlayerStats.Instance.gravity * PlayerStats.Instance.gravityFallModifier * Time.deltaTime;
        else
            velocity.y += PlayerStats.Instance.gravity * Time.deltaTime;

        if (jumpReleased && velocity.y > 0)
        {
            velocity = new Vector2(velocity.x, velocity.y * 0.5f);
        }
    }

    public void Jump(bool force = false, AudioClip soundOverride = null)
    {
        AudioClip sfx = soundOverride == null ? jumpSound : soundOverride;
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(sfx);
        }
        else
        {
            Debug.LogError("Jump sound or AudioSource is not set properly");
        }
        if (force)
        {
            jumpPressed = true;
        }
        velocity.y = Mathf.Sqrt(2f * PlayerStats.Instance.jumpHeight * -PlayerStats.Instance.gravity);
        coyoteTimeCounter = 0;
    }

    private void ClampFallSpeed()
    {
        if (rb.velocity.y < PlayerStats.Instance.maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, PlayerStats.Instance.maxFallSpeed);
        }
    }
}