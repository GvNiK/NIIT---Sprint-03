using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMovementController
{
	private Transform transform;
	private Animator animator;
	private float turnAmount;
	private float forwardAmount;
	private PlayerSettings settings;
	private Target target;

	public PlayerMovementController(Transform transform, Animator animator,
		PlayerSettings settings)
	{
		this.transform = transform;
		this.animator = animator;
		this.settings = settings;
	}

	public void MoveTo(Vector3 movement)
	{
		if(IsFaceInProgress())
		{
			return;
		}
		SwitchTarget(new MoveTarget(transform, movement, settings), () => { });
	}

	private bool IsFaceInProgress()
	{
		return target is FaceTarget;
	}

	private void ApplyRotation(float extraRotation)
	{
		transform.Rotate(0, extraRotation, 0);
	}

	public void Face(Vector3 targetDirection, Action OnComplete)	//This Function Moves & Rotates the Player, towards the Target.
	{
		targetDirection.y = 0f;
		SwitchTarget(new FaceTarget(transform, targetDirection), () => OnComplete());
	}

	private void UpdateAnimator()
	{
		float animForward = animator.GetFloat("Forward");
		float transitionSpeed = animForward > forwardAmount ? settings.RunToStationaryTransitionSpeed : settings.StationaryToRunTransitionSpeed;
		animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime * transitionSpeed);
		animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
	}

	private void SwitchTarget(Target newTarget, Action OnComplete)
	{
		target = newTarget;
		target.OnAnimatorPropertiesUpdated += (turn, forward, turnSpeed) =>
		{
			turnAmount = turn;
			forwardAmount = forward;
			ApplyRotation(turnSpeed);
			UpdateAnimator();
		};
		target.OnComplete += () =>
		{
			target = null;
			OnComplete();
		};
		target.Start();
	}

	public void Update()
	{
		if(target != null)
		{
			target.Update();
		}
		else
		{
			turnAmount = 0f;
			forwardAmount = 0f;
			animator.SetFloat("Forward", 0f);
			animator.SetFloat("Turn", 0f);
		}
	}

	private abstract class Target
	{
		public Action OnComplete = delegate { };
		public Action<float, float, float> OnAnimatorPropertiesUpdated = delegate { };
		public abstract void Start();
		public abstract void Update();
	}

	private class FaceTarget : Target
	{
		private Transform transform;
		private Vector3 targetDirection;

		public FaceTarget(Transform transform, Vector3 targetDirection)
		{
			this.transform = transform;
			this.targetDirection = targetDirection;
		}

		public override void Start()
		{
			Update();
		}

		public override void Update()
		{
			Vector3 faceDirection = targetDirection;
			if (faceDirection.magnitude > 1f)
			{
				faceDirection.Normalize();
			}

			faceDirection = transform.InverseTransformDirection(faceDirection);
			faceDirection = Vector3.ProjectOnPlane(faceDirection, Vector3.zero);
			float turnAmount = Mathf.Atan2(faceDirection.x, faceDirection.z);

			float angleToDirection = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);
			float maxTurn = Math.Sign(angleToDirection) * 360f * Time.deltaTime;
			float turnSpeed = -Mathf.Min(maxTurn, angleToDirection);

			OnAnimatorPropertiesUpdated(turnAmount, 0f, turnSpeed);

			if (Vector3.Angle(transform.forward, targetDirection) < 1f)
			{
				OnComplete();
			}
		}
	}


	private class MoveTarget : Target
	{
		private Transform transform;
		private Vector3 movement;
		private PlayerSettings settings;

		public MoveTarget(Transform transform, Vector3 movement,
			PlayerSettings settings)
		{
			this.transform = transform;
			this.movement = movement;
			this.settings = settings;
		}

		public override void Start()
		{
			if (movement.magnitude > 1f)
			{
				movement.Normalize();
			}

			movement = transform.InverseTransformDirection(movement);
			movement = Vector3.ProjectOnPlane(movement, Vector3.up);
			float turnAmount = Mathf.Atan2(movement.x, movement.z);

			float forwardAmount = 0f;
			float movementTarget = movement.z;

			if (movementTarget != 0)
			{
				forwardAmount = movement.z;
			}
			else
			{
				forwardAmount = Mathf.Lerp(forwardAmount, movementTarget, Time.deltaTime);
			}

			float turnSpeed = Mathf.Lerp(settings.StationaryTurnSpeed, settings.MovingTurnSpeed, forwardAmount);
			float extraRotation = turnAmount * turnSpeed * Time.deltaTime;

			OnAnimatorPropertiesUpdated(turnAmount, forwardAmount, extraRotation);
		}

		public override void Update() 
		{
			OnComplete();
		}
	}

}
