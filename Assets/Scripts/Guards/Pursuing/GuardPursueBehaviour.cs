using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPursueBehaviour : GuardBehaviour
{
	private NavMeshAgent meshAgent;
	private Transform targetObject;
	private Animator animator;
	private GuardAnimator animatorController;
	private AnimationListener animListener;
	private GameObject damageCollider;
	private GuardGunController guardGunController;
	private float predictDistanceThreshold = 6.0f;
	
	private float meleeRotateSpeed = 2.0f;
	private bool targetInRange = false;
	private Guard.VisionData visionData; 

	private Vector3 targetPreviousPosition;

	public GuardPursueBehaviour(NavMeshAgent meshAgent, Transform targetObject, Guard.VisionData visionData, GameObject damageCollider, GuardAnimator animatorController, GuardGunController guardGunController, float pursueMoveSpeed)
	{
		this.meshAgent = meshAgent;
		this.targetObject = targetObject;
		this.damageCollider = damageCollider;
		this.animatorController = animatorController;
		this.guardGunController = guardGunController;
		this.visionData = visionData;

		var collider = damageCollider.GetComponent<BoxCollider>();
		if (collider)
		{
			var bounds = Vector3.Scale(collider.size, collider.transform.lossyScale);
			visionData.upperRangeBound = Mathf.Max(bounds.x, Mathf.Max(bounds.y, bounds.z));
		}

		animator = meshAgent.gameObject.GetComponent<Animator>();
		animListener = meshAgent.gameObject.GetComponent<AnimationListener>();
		meshAgent.speed = pursueMoveSpeed;
		meshAgent.ResetPath();
	}

	public override void Begin()
	{
		animListener.OnAnimationEvent += OnAnimationEvent;
		targetPreviousPosition = targetObject.transform.position;
	}

	private void OnAnimationEvent(string parameter)
	{
		switch(parameter)
		{
			case "AttackDamageStart":
				AttackStart();
				break;
			case "AttackDamageEnd":
				AttackEnd();
				break;
		}
	}

	public override void End()
	{
		meshAgent.isStopped = false;
		meshAgent.updateRotation = true;
		animListener.OnAnimationEvent -= OnAnimationEvent;
		animator.SetBool("Attack", false);
	}

	public override void Update()
	{
		float distanceToTarget = Vector3.Distance(meshAgent.transform.position, targetObject.position);
		if (distanceToTarget <= visionData.lowerRangeBound || targetInRange)
		{
			targetInRange = true;
			meshAgent.isStopped = true;
			meshAgent.updateRotation = false;
			meshAgent.velocity = Vector3.zero;
			Move(Vector3.zero);

			RotateTowardsTarget();

			// Play attack animation if it isn't already playing
			if (!animator.GetBool("Attack"))
			{
				PlayAttackAnimation();
			}
		}
		else if (distanceToTarget < predictDistanceThreshold)
		{
			SetNewDestination(targetObject.position);
			Move(meshAgent.desiredVelocity);
		}
		else
		{
			SetNewDestination(PredictFuturePosition());
			Move(meshAgent.desiredVelocity);
		}

		if (distanceToTarget >= visionData.upperRangeBound)
		{
			targetInRange = false;
		}

		targetPreviousPosition = targetObject.transform.position;
	}

	private Vector3 PredictFuturePosition()
	{
		// If we're paused, return target's current position to prevent us dividing by 0
		if (Time.deltaTime == 0)
		{
			return targetObject.transform.position;
		}

		Vector3 targetVelocity = (targetObject.transform.position - targetPreviousPosition) / Time.deltaTime;
		var targetForward = Vector3.Scale(targetObject.transform.forward, targetVelocity);
		return targetObject.transform.position + targetForward;
	}

	private void SetNewDestination(Vector3 destination)
	{
		meshAgent.isStopped = false;
		meshAgent.updateRotation = true;
		meshAgent.SetDestination(destination);
	}

	private void RotateTowardsTarget()
	{
		Vector3 goalDirection = targetObject.position - meshAgent.transform.position;
		// Remove the y component so we don't pitch the gameobject, as it'll fight with navmeshagent
		goalDirection.y = 0;
		goalDirection = goalDirection.normalized;
		float stepAmount = meleeRotateSpeed * Time.deltaTime;
		Vector3 newDirection = Vector3.RotateTowards(meshAgent.transform.forward, goalDirection, stepAmount, 0.0f);
		meshAgent.transform.forward = newDirection;
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
		animatorController.UpdateMovement(turnAmount, forwardAmount);
	}

	private void PlayAttackAnimation()
	{
		animator.SetTrigger("Attack");
	}

	// MeleeDamage functions are called by an animation event in animation clip
	public void AttackStart()
	{
		guardGunController.FireBullet();
		damageCollider.SetActive(true);
	}

	public void AttackEnd()
	{
		damageCollider.SetActive(false);
	}
}
