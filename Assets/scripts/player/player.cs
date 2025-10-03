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

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public Vector2 friction = new Vector2(-1f, 0);

    [Header("Jump Settings")]
    public float jumpForce = 8f;

    [Header("Jump Buffer Settings")]
    public float jumpBufferTime = 0.1f;
    public float coyoteTime = 0.1f;

    [Header("Double Jump Settings")]
    public int maxJumpCount = 2;
    private int currentJumpCount;

    [Header("Visual FX Settings")]
    public float scaleLerpSpeed = 10f;
    public float tiltAngle = 15f;
    public float maxVerticalSpeedForEffects = 10f;

    private float _currentSpeed;
    private Vector3 originalScale;
    public int facingDirection = 1;
    private Animator _animator;
    private bool isGrounded;
    private bool isJumping;
    private bool isFalling;

    private float jumpBufferCounter;
    private float coyoteTimeCounter;

    private HealthBase _healthBase;

    void Start()
    {
        if (visual != null)
        {
            originalScale = visual.localScale;
            _animator = visual.GetComponent<Animator>();
        }
    }

    void Update()
    {
        isGrounded = CheckIfGrounded();

        if (isGrounded)
        {
            currentJumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

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
    _currentSpeed = Input.GetKey(KeyCode.X) ? runSpeed : walkSpeed;

    if (Input.GetKey(KeyCode.LeftArrow))
    {
        myRigidBody.linearVelocity = new Vector2(-_currentSpeed, myRigidBody.linearVelocity.y);
        facingDirection = -1;
    }
    else if (Input.GetKey(KeyCode.RightArrow))
    {
        myRigidBody.linearVelocity = new Vector2(_currentSpeed, myRigidBody.linearVelocity.y);
        facingDirection = 1;
    }
    else
    {
        float newX = Mathf.MoveTowards(myRigidBody.linearVelocity.x, 0, Mathf.Abs(friction.x));
        myRigidBody.linearVelocity = new Vector2(newX, myRigidBody.linearVelocity.y);
    }

    if (_animator != null)
    {
        float horizontalSpeed = Mathf.Abs(myRigidBody.linearVelocity.x);
        _animator.SetBool("isMoving", horizontalSpeed > 0.1f);

        _animator.speed = horizontalSpeed >= runSpeed - 0.1f ? 1.5f : 1f;
    }

    SpriteRenderer spriteRenderer = visual.GetComponent<SpriteRenderer>();
    if (spriteRenderer != null)
    {
        spriteRenderer.flipX = facingDirection == -1;
    }
}



    private void HandleJumpInput()
    {
        bool jumpPressed = jumpBufferCounter > 0f;

        if (jumpPressed && (isGrounded || coyoteTimeCounter > 0f) && currentJumpCount == 0)
        {
            Jump();
        }
        else if (jumpPressed && currentJumpCount < maxJumpCount && !isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        myRigidBody.linearVelocity = new Vector2(myRigidBody.linearVelocity.x, jumpForce);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
        currentJumpCount++;

        if (_animator != null)
        {
            _animator.SetBool("isJumping", true);
            _animator.SetBool("isFalling", false);
        }
    }

    private void UpdateAnimatorParams()
    {
        float verticalVelocity = myRigidBody.linearVelocity.y;

        if (!isGrounded)
        {
            if (verticalVelocity > 0.1f)
            {
                if (_animator != null)
                {
                    _animator.SetBool("isJumping", true);
                    _animator.SetBool("isFalling", false);
                }
            }
            else if (verticalVelocity < -0.1f)
            {
                if (_animator != null)
                {
                    _animator.SetBool("isJumping", false);
                    _animator.SetBool("isFalling", true);
                }
            }
        }
        else
        {
            if (_animator != null)
            {
                _animator.SetBool("isJumping", false);
                _animator.SetBool("isFalling", false);
            }
        }
    }

    private void UpdateVisualEffects()
    {
        if (visual == null) return;

        float verticalSpeed = myRigidBody.linearVelocity.y;

        if (isJumping || isFalling)
        {
            visual.localScale = new Vector3(
                Mathf.Abs(originalScale.x) * facingDirection,
                originalScale.y,
                originalScale.z
            );
        }
        else
        {
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(verticalSpeed) / maxVerticalSpeedForEffects);
            float scaleY = Mathf.Lerp(1.0f, 1.8f, normalizedSpeed);
            float scaleX = Mathf.Lerp(1.0f, 0.3f, normalizedSpeed);

            Vector3 targetScale = new Vector3(
                originalScale.x * scaleX * facingDirection,
                originalScale.y * scaleY,
                originalScale.z
            );

            visual.localScale = Vector3.Lerp(visual.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);
        }

        float tiltDirection = facingDirection;
        float targetAngle = Mathf.Clamp(-verticalSpeed * 2f * tiltDirection, -tiltAngle, tiltAngle);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        visual.rotation = Quaternion.Lerp(visual.rotation, targetRotation, Time.deltaTime * scaleLerpSpeed);

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
