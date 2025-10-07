using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 4.5f;
    public Vector2 friction = new Vector2(10f, 0);

    [Header("Acceleration")]
    public float groundAcceleration = 60f;
    public float airAcceleration = 30f;

    [Header("Running")]
    public float runAccelerationTime = 0.4f;
    public float runDecelerationTime = 0.3f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float fallGravityMultiplier = 2.5f;       // Gravidade extra quando está caindo
    public float lowJumpGravityMultiplier = 2f;      // Gravidade extra se soltar o pulo cedo

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.1f;
    public float coyoteTime = 0.1f;

    [Header("Visual FX")]
    public float scaleLerpSpeed = 10f;
    public float tiltAngle = 15f;
    public float maxVerticalSpeedForEffects = 10f;
}
