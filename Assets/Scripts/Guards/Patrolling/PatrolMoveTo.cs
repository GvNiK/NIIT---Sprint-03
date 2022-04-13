using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolMoveTo : PatrolCommand
{
	private NavMeshAgent meshAgent;
	private Transform goal;
	private GuardAnimator animationController;

	public PatrolMoveTo(NavMeshAgent meshAgent, Transform goal, GuardAnimator animationController)
	{
		this.meshAgent = meshAgent;
		this.goal = goal;
		this.animationController = animationController;
	}

	public override void Begin()
	{
		meshAgent.SetDestination(goal.position);
	}

	public override void End()
	{
		
	}

	public override void Update()
	{
		// Check to see if we've reached our destination
		float distanceToGoal = (meshAgent.destination - meshAgent.transform.position).magnitude;
		if (!meshAgent.hasPath ||
			distanceToGoal < 0.2f ||
			(meshAgent.velocity.sqrMagnitude == 0  && distanceToGoal < meshAgent.stoppingDistance))
		{
			meshAgent.ResetPath();
			CompleteCommand();
		}
		else
		{
			if (distanceToGoal < meshAgent.stoppingDistance)
			{
				Move(Vector3.zero);
			}
			else
			{
				Move(meshAgent.desiredVelocity);
			}
		}
	}

	private void Move(Vector3 move)
	{
		if (move.magnitude > 1f)
		{
			move.Normalize();
		}
		move = meshAgent.transform.InverseTransformDirection(move);
		move = Vector3.ProjectOnPlane(move, Vector3.up);
		float turnAmount = Mathf.Atan2(move.x, move.z);
		float forwardAmount = move.z;

		// Because our movement is driven by root motion, we have to tell our animator to help move our character
		animationController.UpdateMovement(turnAmount, forwardAmount);
	}
}
