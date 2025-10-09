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

    [Header("VFX")]
    public ParticleSystem moveVFX;
    public ParticleSystem jumpVFX;

    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float currentSpeedMultiplier = 0f;

    [Header("Audio Sources & Clips")]
    public AudioSource jumpAudioSource;
    public AudioSource moveAudioSource;
    public AudioClip jumpClip;
    public AudioClip slideClip;
    public AudioClip footstepClip;

    [Header("Sliding Settings")]
    public float slideMinSpeed = 1.5f;

    private bool isSliding = false;
    private bool hasPlayedSlideSound = false;
    private float slideAnimBufferTimer = 0f;
    public float slideAnimDuration = 0.25f;

    private float footstepTimer = 0f;

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

        if (moveAudioSource != null)
        {
            moveAudioSource.clip = footstepClip;
            moveAudioSource.loop = true;
            moveAudioSource.volume = 0f;
            moveAudioSource.Play();
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

            if (myRigidBody.linearVelocity.y <= 0f)
            {
                Vector2 vel = myRigidBody.linearVelocity;
                vel.y = 0f;
                myRigidBody.linearVelocity = vel;
                SnapPlayerToGround();
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
        UpdateMoveVFX();
        CheckSliding();
        UpdateFootstepSounds();
    }

    private void SnapPlayerToGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius * 2f, groundLayer);
        if (hit.collider != null)
        {
            Vector3 pos = transform.position;
            float playerBottomY = groundCheck.position.y - groundCheckRadius;
            float distanceToGround = playerBottomY - hit.point.y;
            if (distanceToGround > 0f)
            {
                pos.y -= distanceToGround;
                transform.position = pos;
            }
        }
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

            bool isRunningAnim = isGrounded && horizontalSpeed >= (config.runSpeed - config.runAnimationThreshold);
            _animator.SetBool("IsRunning", isRunningAnim);
        }

        var spriteRenderer = visual.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && isGrounded)
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
            PlayJumpSound();
            JumpVFX();
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

    private void JumpVFX()
    {
        if (jumpVFX != null) jumpVFX.Play();
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

    private void UpdateMoveVFX()
    {
        if (moveVFX == null) return;

        bool shouldPlay = isGrounded && Mathf.Abs(myRigidBody.linearVelocity.x) > 0.1f;

        if (shouldPlay && !moveVFX.isPlaying)
        {
            moveVFX.Play();
        }
        else if (!shouldPlay && moveVFX.isPlaying)
        {
            moveVFX.Stop();
        }
    }

    private void CheckSliding()
    {
        float horizontalSpeed = myRigidBody.linearVelocity.x;

        bool holdingRight = Input.GetKey(KeyCode.RightArrow);
        bool holdingLeft = Input.GetKey(KeyCode.LeftArrow);

        bool slidingNow = false;

        if (holdingRight && horizontalSpeed < -slideMinSpeed)
            slidingNow = true;
        else if (holdingLeft && horizontalSpeed > slideMinSpeed)
            slidingNow = true;

        if (slidingNow)
        {
            slideAnimBufferTimer = slideAnimDuration;
            _animator?.SetBool("IsSliding", true);

            if (!hasPlayedSlideSound && jumpAudioSource != null && slideClip != null)
            {
                jumpAudioSource.PlayOneShot(slideClip);
                hasPlayedSlideSound = true;
            }
        }
        else
        {
            if (slideAnimBufferTimer > 0f)
            {
                slideAnimBufferTimer -= Time.deltaTime;
                _animator?.SetBool("IsSliding", true);
            }
            else
            {
                _animator?.SetBool("IsSliding", false);
                hasPlayedSlideSound = false;
            }
        }

        isSliding = slidingNow;
    }

    private void UpdateFootstepSounds()
    {
        if (moveAudioSource == null || footstepClip == null) return;

        bool isMoving = isGrounded && Mathf.Abs(myRigidBody.linearVelocity.x) > 0.1f;

        if (isMoving)
        {
            float moveSpeed = Mathf.Abs(myRigidBody.linearVelocity.x);
            float interval = Mathf.Lerp(0.5f, 0.1f, Mathf.InverseLerp(config.walkSpeed, config.runSpeed, moveSpeed));

            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                moveAudioSource.pitch = 1.0f;
                moveAudioSource.PlayOneShot(footstepClip, 0.7f);
                footstepTimer = interval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void PlayJumpSound()
    {
        if (jumpAudioSource != null && jumpClip != null)
        {
            jumpAudioSource.PlayOneShot(jumpClip);
        }
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
