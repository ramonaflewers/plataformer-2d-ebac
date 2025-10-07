using UnityEngine;
using DG.Tweening;

public class player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D myRigidBody;
    public Transform visual;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;

    [Header("Config")]
    public PlayerConfig config;

    private float _currentSpeed;
    private Vector3 originalScale;
    public int facingDirection = 1;
    private Animator _animator;
    private bool isGrounded;

    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float currentSpeedMultiplier = 0f;

    private void Start()
    {
        if (visual != null)
        {
            originalScale = visual.localScale;
            _animator = visual.GetComponent<Animator>();
        }

        if (config == null)
        {
            Debug.LogError("PlayerConfig não atribuído no inspetor!", this);
            enabled = false;
        }
    }

    private void Update()
    {
        isGrounded = CheckIfGrounded();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            jumpBufferCounter = config.jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (isGrounded)
        {
            coyoteTimeCounter = config.coyoteTime;

            // Evita "travamento" zerando a velocidade vertical só quando estiver caindo ou quase parando
            if (myRigidBody.linearVelocity.y < 0.1f)
            {
                Vector2 vel = myRigidBody.linearVelocity;
                vel.y = 0f;
                myRigidBody.linearVelocity = vel;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        ApplyCustomGravity();

        HandleInput();
        UpdateAnimatorParams();
        UpdateVisualEffects();
    }

    private void HandleInput()
    {
        HandleMovementInput();
        HandleJumpInput();
    }

    private void HandleMovementInput()
    {
        bool isRunning = Input.GetKey(KeyCode.X);
        float baseSpeed = config.walkSpeed;
        float maxSpeed = config.runSpeed;

        if (isRunning)
            currentSpeedMultiplier += Time.deltaTime / config.runAccelerationTime;
        else
            currentSpeedMultiplier -= Time.deltaTime / config.runDecelerationTime;

        currentSpeedMultiplier = Mathf.Clamp01(currentSpeedMultiplier);
        float desiredSpeed = Mathf.Lerp(baseSpeed, maxSpeed, currentSpeedMultiplier);

        float targetSpeed = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetSpeed = -desiredSpeed;
            facingDirection = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            targetSpeed = desiredSpeed;
            facingDirection = 1;
        }

        float acceleration = isGrounded ? config.groundAcceleration : config.airAcceleration;
        float newX = Mathf.MoveTowards(myRigidBody.linearVelocity.x, targetSpeed, acceleration * Time.deltaTime);
        myRigidBody.linearVelocity = new Vector2(newX, myRigidBody.linearVelocity.y);

        if (_animator != null)
        {
            float horizontalSpeed = Mathf.Abs(myRigidBody.linearVelocity.x);
            _animator.SetBool("isMoving", horizontalSpeed > 0.1f);
            _animator.speed = currentSpeedMultiplier > 0.9f ? 1.5f : 1f;
        }

        var spriteRenderer = visual.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && isGrounded == true)
        {
            spriteRenderer.flipX = facingDirection == -1;
        }
    }

    private void HandleJumpInput()
    {
        bool jumpPressed = jumpBufferCounter > 0f;

        if (jumpPressed && (isGrounded || coyoteTimeCounter > 0f))
        {
            Jump();
        }
    }

    private void Jump()
    {
        myRigidBody.linearVelocity = new Vector2(myRigidBody.linearVelocity.x, config.jumpForce);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;

        if (_animator != null)
        {
            _animator.SetBool("isJumping", true);
            _animator.SetBool("isFalling", false);
        }
    }

    private void ApplyCustomGravity()
    {
        if (!isGrounded)
        {
            if (myRigidBody.linearVelocity.y < 0)
            {
                myRigidBody.linearVelocity += Vector2.up * Physics2D.gravity.y * (config.fallGravityMultiplier - 1f) * Time.deltaTime;
            }
            else if (!Input.GetKey(KeyCode.Z))
            {
                myRigidBody.linearVelocity += Vector2.up * Physics2D.gravity.y * (config.lowJumpGravityMultiplier - 1f) * Time.deltaTime;
            }
        }
    }

    private void UpdateAnimatorParams()
    {
        float verticalVelocity = myRigidBody.linearVelocity.y;

        if (!isGrounded)
        {
            if (verticalVelocity > 0.1f)
            {
                _animator?.SetBool("isJumping", true);
                _animator?.SetBool("isFalling", false);
            }
            else if (verticalVelocity < -0.1f)
            {
                _animator?.SetBool("isJumping", false);
                _animator?.SetBool("isFalling", true);
            }
        }
        else
        {
            _animator?.SetBool("isJumping", false);
            _animator?.SetBool("isFalling", false);
        }
    }

    private void UpdateVisualEffects()
    {
        if (visual == null) return;

        float verticalSpeed = myRigidBody.linearVelocity.y;
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(verticalSpeed) / config.maxVerticalSpeedForEffects);

        float scaleY = Mathf.Lerp(1.0f, 1.8f, normalizedSpeed);
        float scaleX = Mathf.Lerp(1.0f, 0.3f, normalizedSpeed);

        Vector3 targetScale = new Vector3(
            originalScale.x * scaleX * facingDirection,
            originalScale.y * scaleY,
            originalScale.z
        );

        visual.localScale = Vector3.Lerp(visual.localScale, targetScale, Time.deltaTime * config.scaleLerpSpeed);

        float targetAngle = Mathf.Clamp(-verticalSpeed * 2f * facingDirection, -config.tiltAngle, config.tiltAngle);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        visual.rotation = Quaternion.Lerp(visual.rotation, targetRotation, Time.deltaTime * config.scaleLerpSpeed);

        Vector3 fixedScale = visual.localScale;
        fixedScale.x = Mathf.Abs(fixedScale.x) * facingDirection;
        visual.localScale = fixedScale;
    }

    private bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
