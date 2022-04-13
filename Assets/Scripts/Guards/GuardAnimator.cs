using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardAnimator
{
	public bool isInLocomotion = true;
	private bool canCheckLocomotionState = false;
	private string locomotionStateName = "Locomotion BlendTree";
	private bool canLookaround = false;

	private NavMeshAgent meshAgent;
	private Animator animator;

	private float turnAmount;
	private float forwardAmount;

	private bool updatedTurnThisFrame;
	private bool updatedMoveThisFrame;



	public GuardAnimator(NavMeshAgent meshAgent, Animator animator, Guard guard)
	{
		this.meshAgent = meshAgent;
		this.animator = animator;

		guard.OnDamageTaken += TakeHit;

		canCheckLocomotionState = animator.HasState(0, Animator.StringToHash(locomotionStateName));
		if (!canCheckLocomotionState)
		{
			Debug.LogWarning($"isInLocomotion will always be true for {animator.gameObject.name} because we can't find a state named {locomotionStateName} in the animator");
		}
		canLookaround = HasParameter("LookAround");
	}

	public void Update()
	{
		DecayTurnRateToZero();
		DecayMovementToZero();

		if (!meshAgent.hasPath)
		{
			return;
		}

		float distanceToGoal = (meshAgent.destination - meshAgent.transform.position).magnitude;
		if (distanceToGoal < meshAgent.stoppingDistance)
		{
			Move(Vector3.zero);
		}
		else
		{
			Move(meshAgent.desiredVelocity);
		}

		if (canCheckLocomotionState)
		{
			isInLocomotion = animator.GetCurrentAnimatorStateInfo(0).IsName(locomotionStateName);
		}
	}

	private void DecayTurnRateToZero()
	{
		// If we've set a turn rate this frame, switch the flag to false so we don't adjust turn rate
		if (updatedTurnThisFrame)
		{
			updatedTurnThisFrame = false;
			return;
		}

		animator?.SetFloat("TurnRate", 0.0f, 0.15f, Time.deltaTime);
	}

	private void DecayMovementToZero()
	{
		// If our pathing's set a move speed this frame, switch the flag to false so we don't adjust it
		if (updatedMoveThisFrame)
		{
			updatedMoveThisFrame = false;
			return;
		}

		animator?.SetFloat("MoveSpeed", 0.0f, 0.15f, Time.deltaTime);
	}

	public void TurnOnSpot(float turnRate)
	{
		animator.SetFloat("TurnRate", turnRate, 0.15f, Time.deltaTime);
		animator.SetFloat("MoveSpeed", 0.0f, 0.5f, Time.deltaTime);
		updatedTurnThisFrame = true;
	}

	private void Move(Vector3 move)
	{
		updatedMoveThisFrame = true;
		if (move.magnitude > 1f)
		{
			move.Normalize();
		}
		move = meshAgent.transform.InverseTransformDirection(move);
		move = Vector3.ProjectOnPlane(move, Vector3.up);
		turnAmount = Mathf.Atan2(move.x, move.z);
		forwardAmount = move.z;
	}

	public void UpdateMovement(float turnAmount, float forwardAmount)
	{
		turnAmount = Mathf.Clamp(turnAmount, -1.0f, 1.0f);
		forwardAmount = Mathf.Clamp01(forwardAmount);

		animator.SetFloat("TurnRate", turnAmount, 0.05f, Time.deltaTime);
		animator.SetFloat("MoveSpeed", forwardAmount, 0.35f, Time.deltaTime);
	}

	private void TakeHit(float amount, Transform instigator)
	{
		// Figure out if the instigator is in front or behind us
		Vector3 forwardNoY = new Vector3(meshAgent.transform.forward.x, 0.0f, meshAgent.transform.forward.z);
		Vector3 insitgatorNoY = new Vector3(instigator.transform.position.x, 0.0f, instigator.transform.position.z);
		Vector3 directionToInstigator = insitgatorNoY - forwardNoY;

		if (Vector3.Dot(directionToInstigator, insitgatorNoY) >= 0.0f)
		{
			animator.SetTrigger("TakeHitFront");
		}
		else
		{
			animator.SetTrigger("TakeHitRear");
		}
	}

	public void LookAround()
	{
		if (canLookaround && animator)
		{
			animator.SetTrigger("LookAround");
		}
	}

	private bool HasParameter(string paramName)
	{
		foreach (AnimatorControllerParameter param in animator.parameters)
		{
			if (param.name == paramName)
				return true;
		}
		return false;
	}
}
