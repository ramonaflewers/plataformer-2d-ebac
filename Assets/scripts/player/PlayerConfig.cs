using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
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

    [Header("Visual FX Settings")]
    public float scaleLerpSpeed = 10f;
    public float tiltAngle = 15f;
    public float maxVerticalSpeedForEffects = 10f;
}
