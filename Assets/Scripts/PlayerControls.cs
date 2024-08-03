using Prime31;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerControls : MonoBehaviour
{
    enum PlayerDirection { Right, Left }
    private PlayerDirection direction = PlayerDirection.Right;

    [Header("Movement")]
    public float defaultGravity = -25;
    public float gravity = -25;
    public float runSpeed = 8;
    public float groundDamping = 20; // how fast do we change direction? higher means faster
    public float inAirDamping = 5;
    public float jumpHeight = 3;
    public float maxFallSpeed = -9;
    private float normalizedHorizontalSpeed = 0;
    public float fallSpeedModifier = 1;
    public float gravityFallModifier = 1;

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
    // private Animator animator;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    private Vector3 velocity;

    private bool wasGroundedLastFrame = false;

    void Awake()
    {
        // animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController2D>();

        // listen to some events for illustration purposes
        controller.onControllerCollidedEvent += OnControllerCollider;
        controller.onTriggerEnterEvent += OnTriggerEnterEvent;
        controller.onTriggerExitEvent += OnTriggerExitEvent;

        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    #region Event Listeners

    void OnControllerCollider(RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they aren't very interesting
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            // If the normal of the hit is horizontal, zero out the x velocity
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

        // animator.SetFloat("xVelocity", Mathf.Abs(velocity.x));
        // animator.SetFloat("yVelocity", velocity.y);
    }

    void FixedUpdate()
    {
        if (controller.isGrounded)
        {
            velocity.y = 0;
            coyoteTimeCounter = coyoteTime;
            // animator.SetBool("isJumping", false);
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
            // animator.SetBool("isJumping", true);
        }

        if (velocity.y < 0)
            velocity.y *= fallSpeedModifier;

        HandleHorizontal();
        HandleJump();
        ClampFallSpeed();

        controller.move(velocity * Time.deltaTime);

        // grab our current velocity to use as a base for all calculations
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

            // Adjust sprite facing based on direction
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

            /*
            if (controller.isGrounded)
            {
                SetAnimationState("Idle");
                PlayAnimation("Idle");
            }
            else if (velocity.y < 0)
            {
                SetAnimationState("Falling");
                PlayAnimation("Fall");
            }
            */
        }

        var smoothedMovementFactor = controller.isGrounded ? groundDamping : inAirDamping;
        velocity.x = Mathf.Lerp(velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
    }

    void HandleJump()
    {
        if (jumpPressed && coyoteTimeCounter > 0f)
        {
            Jump();
        }

        if (velocity.y < 0)
            velocity.y += gravity * gravityFallModifier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

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
        velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
        coyoteTimeCounter = 0;

        // animator.SetBool("isJumping", true);
    }

    private void ClampFallSpeed()
    {
        if (rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
        }
    }

    /*
    public void SetAnimationState(string state)
    {
        animator.SetInteger("state", Animator.StringToHash(state));
    }

    public void PlayAnimation(string name)
    {
        animator.Play(Animator.StringToHash(name));
    }
    */
}