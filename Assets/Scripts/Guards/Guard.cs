using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class Guard : MonoBehaviour
{
	public Action<float, Transform> OnDamageTaken = delegate { };
	public PatrolData patrolData;
	public VisionData visionData;
	public SuspicionData suspicionData;
	public ModelData modelData;
	public GeneralData generalData;
	public Transform target;
	public Transform blaster;

	public void TakeDamage(float damageAmount, Transform instigator)
	{
		OnDamageTaken.Invoke(damageAmount, instigator);
	}

	[Serializable]
	public class VisionData
	{
		public LayerMask raycastMask;

		public float eyeHeight = 1.5f;

		public float detectionRadius = 8.0f;

		[Tooltip("Degrees. Angle is half Field of View, so 30 is 60 degree arc total")]
		[Range(0.0f, 180.0f)]
		public float detectionAngle = 30.0f;

		public float lowerRangeBound = 5.0f;

		public float upperRangeBound = 8.0f;

		public bool visualise = false;

		public float awarenessZone = 2.0f;
	}

	[Serializable]
	public class ModelData
	{
		public List<ParticleSystem> aliveParticles;
	}

	[Serializable]
	public class PatrolData
	{
		public List<WaypointInfo> waypoints;
	}

	[Serializable]
	public class SuspicionData
	{
		public float currentSuspicion;

		[Range(0.0f, 1.0f)]
		public float investigatingThreshold = 0.1f;

		[Range(0.0f, 1.0f)]
		public float pursuingThreshold = 0.6f;

		public float suspicionBuildRate = 0.66f;
		public float suspicionDecayRate = 0.2f;
	}

	[Serializable]
	public class GeneralData
	{
		public float patrolMoveSpeed = 0.5f;
		public float pursueMoveSpeed = 1.0f;
		public float maxHealth = 100f;
		public string name = "Blobby Charlton";
		public float attackDamage = 25f;
		public Transform infoContainer;
	}
}