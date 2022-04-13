using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaypointInfo
{
	public WaypointType NodeType;
	public Transform TransformTarget;
	public float WaitTime;
	public Vector3 TargetRotation;
	public float RotationSpeed;
}


public enum WaypointType
{
	None,
	MoveTo,
	Rotate,
	Wait,
	Count
}
