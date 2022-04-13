using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolBehaviour : GuardBehaviour
{
	private List<WaypointInfo> waypoints;
	private float patrolMoveSpeed;
	private NavMeshAgent meshAgent;
	private WaypointInfo nextWaypoint;
	private PatrolCommand currentCommand;
	private GuardAnimator animationController;
	private int nextWaypointIndex = 0;

	public GuardPatrolBehaviour(NavMeshAgent meshAgent, List<WaypointInfo> waypoints, GuardAnimator animationController, float patrolMoveSpeed)
	{
		this.meshAgent = meshAgent;
		this.waypoints = waypoints;
		this.animationController = animationController;
		this.patrolMoveSpeed = patrolMoveSpeed;
	}

	public override void Begin()
	{
		nextWaypointIndex = GetClosestWaypoint();
		nextWaypoint = waypoints[nextWaypointIndex];
		animationController.LookAround();
		ExecuteWaypointInstructions();
	}

	public override void End()
	{
		meshAgent.isStopped = false;
		meshAgent.updateRotation = true;
	}

	public override void Update()
	{
		// Check with the animator to see if we're waiting on any spawn or hitstun animations
		if (animationController.isInLocomotion)
		{
			meshAgent.speed = patrolMoveSpeed;
			currentCommand.Update();
		}
		else
		{
			meshAgent.speed = 0.0f;
		}
	}

	private int GetClosestWaypoint()
	{
		int closestIndex = 0;
		float distanceOfIndex = float.MaxValue;
		for (int i = 0; i < waypoints.Count; i++)
		{
			if (waypoints[i].NodeType != WaypointType.MoveTo)
			{
				continue;
			}

			float distanceToPoint = Vector3.SqrMagnitude(meshAgent.transform.position - waypoints[i].TransformTarget.position);
			if (distanceToPoint < distanceOfIndex)
			{
				closestIndex = i;
				distanceOfIndex = distanceToPoint;
			}
		}
		return closestIndex;
	}

	private void ExecuteWaypointInstructions()
	{
		currentCommand = GenerateCommand(nextWaypoint);
		currentCommand.OnCommandComplete += CommandComplete;
		currentCommand.Begin();
	}

	private PatrolCommand GenerateCommand(WaypointInfo info)
	{
		switch (info.NodeType)
		{
			case WaypointType.None:
				{
					return null;
				}
			case WaypointType.MoveTo:
				{
					var command = new PatrolMoveTo(meshAgent, info.TransformTarget, animationController);
					return command;
				}
			case WaypointType.Rotate:
				{
					var command = new PatrolRotate(meshAgent, info.TargetRotation, info.RotationSpeed, animationController);
					return command;
				}
			case WaypointType.Wait:
				{
					var command = new PatrolWait(info.WaitTime);
					return command;
				}
			case WaypointType.Count:
				{
					return null;
				}
			default:
				return null;
		}
	}

	private void CommandComplete()
	{
		currentCommand.OnCommandComplete -= CommandComplete;
		currentCommand.End();
		currentCommand = null;
		nextWaypointIndex++;
		if (nextWaypointIndex >= waypoints.Count)
			nextWaypointIndex = 0;

		nextWaypoint = waypoints[nextWaypointIndex];
		ExecuteWaypointInstructions();
	}

}