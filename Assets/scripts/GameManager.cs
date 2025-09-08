using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D myRigidBody;
    public Transform visual;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public Vector2 friction = new Vector2(-1f, 0);

    [Header("Jump Settings")]
    public float jumpForce = 8f;

    [Header("Visual FX Settings")]
    public float scaleLerpSpeed = 10f;
    public float tiltAngle = 15f;
    public float maxVerticalSpeedForEffects = 10f;

    private float _currentSpeed;
    private Vector3 originalScale;

    void Start()
    {
        if (visual != null)
            originalScale = visual.localScale;
    }

    void Update()
    {
        HandleInput();
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
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            myRigidBody.linearVelocity = new Vector2(_currentSpeed, myRigidBody.linearVelocity.y);
        }

        if (myRigidBody.linearVelocity.x > 0)
        {
            myRigidBody.linearVelocity += friction;
        }
        else if (myRigidBody.linearVelocity.x < 0)
        {
            myRigidBody.linearVelocity -= friction;
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            myRigidBody.linearVelocity = Vector2.up * jumpForce;
        }
    }

    private void UpdateVisualEffects()
    {
        if (visual == null) return;

        float verticalSpeed = myRigidBody.linearVelocity.y;
        float horizontalSpeed = myRigidBody.linearVelocity.x;

        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(verticalSpeed) / maxVerticalSpeedForEffects);

        float scaleY = Mathf.Lerp(1.0f, 1.4f, normalizedSpeed);
        float scaleX = Mathf.Lerp(1.0f, 0.6f, normalizedSpeed);

        Vector3 targetScale = new Vector3(
            originalScale.x * scaleX,
            originalScale.y * scaleY,
            originalScale.z
        );

        visual.localScale = Vector3.Lerp(visual.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);

        float tiltDirection = Mathf.Sign(horizontalSpeed) != 0 ? Mathf.Sign(horizontalSpeed) : 1;
        float targetAngle = Mathf.Clamp(-verticalSpeed * 2f * tiltDirection, -tiltAngle, tiltAngle);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        visual.rotation = Quaternion.Lerp(visual.rotation, targetRotation, Time.deltaTime * scaleLerpSpeed);
    }
}