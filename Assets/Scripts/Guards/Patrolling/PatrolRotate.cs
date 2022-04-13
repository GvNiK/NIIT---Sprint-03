using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolRotate : PatrolCommand
{
	private NavMeshAgent meshAgent;
	private Vector3 rotateGoal;
	private float rotateSpeed;
	GuardAnimator animationController;

	public PatrolRotate(NavMeshAgent meshAgent, Vector3 rotateGoal, float rotateSpeed, GuardAnimator animationController)
	{
		this.meshAgent = meshAgent;
		this.rotateGoal = Quaternion.Euler(rotateGoal) * Vector3.forward;
		this.rotateSpeed = rotateSpeed;
		this.animationController = animationController;
	}

	public override void Begin()
	{
		// Take control of the rotation
		meshAgent.updateRotation = false;
	}

	public override void Update()
	{
		if (rotateSpeed == 0.0f)
		{
			Debug.Log("STEP SKIPPED - Attempting to rotate with a speed of 0, command will never complete");
			CompleteCommand();
		}
		float stepAmount = rotateSpeed * Time.deltaTime;
		Vector3 newDirection = Vector3.RotateTowards(meshAgent.transform.forward, rotateGoal, stepAmount, 0.0f);
		meshAgent.transform.forward = newDirection;

		var signedAngle = Vector3.SignedAngle(rotateGoal, meshAgent.transform.forward, Vector3.up);

		// Set a slower speed as we get closer to our rotational goal so the animation doesn't abruptly stop
		var rotateSpeedScale = Mathf.Clamp(signedAngle / 45.0f, -1.0f, 1.0f);
		animationController.TurnOnSpot(rotateSpeed * rotateSpeedScale);

		if (Vector3.Dot(meshAgent.transform.forward, rotateGoal) > 0.99f)
		{
			CompleteCommand();
			return;
		}
	}

	public override void End()
	{
		// Take control of the rotation
		animationController.TurnOnSpot(0.0f);
		meshAgent.updateRotation = true;
	}
}
