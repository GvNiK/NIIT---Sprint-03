using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerSettings", order = 1)]
public class PlayerSettings : ScriptableObject
{
    [Header("FPS Settings")]
    [Range(0f, 15f)]
    public float MoveSpeed = 5f;

    [Range(0.5f, 3f)]
    public float PlayerTurnSpeed = 1.5f;

    [Range(0f, 1f)]
    public float FPSVerticalSensitivity = 1f;

    [Range(0f, 1f)]
    public float FPSHorizontalSensitivity = 1f;

    [Range(45f, 90f)]
    public float FPSMaxVerticalRotation = 85f;

    [Header("Third Person Settings")]
    public float MovingTurnSpeed = 360f;
    public float StationaryTurnSpeed = 180f;
    public float MoveSpeedMultiplier = 1.5f;
    public float GroundCheckDistance = 0.2f;
	public float StationaryToRunTransitionSpeed = 3f;
	public float RunToStationaryTransitionSpeed = 0.5f;

	[Header("General Settings")]
    public float PlayerMaxHP = 100;
    public float LastHitUIShowTime = 3f;

    [Header("Combat Settings")]
	public float SwordDamage = 10f;
	public float DamageAmmoDamage = 20f;
	public float ExplosiveAmmoDamage = 50f;
	public float SwipeCooldown = 0.8f;
    public int FPSSwipeAnimCount = 3;
    public float ShotCheckRadius = 15f;
    public float ShotCheckAngle = 60f;
}
