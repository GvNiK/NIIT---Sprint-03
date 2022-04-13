using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardDespawnBehaviour : GuardBehaviour
{
	public Action OnComplete = delegate { }; 
	private Animator animator;
	private NavMeshAgent meshAgent;

	public GuardDespawnBehaviour(NavMeshAgent meshAgent, Animator animator)
	{
		this.animator = animator;
		this.meshAgent = meshAgent;
	}

	public override void Begin()
	{
		meshAgent.ResetPath();
		meshAgent.isStopped = true;
		meshAgent.velocity = Vector3.zero;
		animator.SetBool("IsDespawning", true);

		AnimationListener animListener = meshAgent.GetComponent<AnimationListener>();
		animListener.OnAnimationEvent += OnAnimationEvent;
	}

	private void OnAnimationEvent(string param)
	{
		switch(param)
		{
			case "DespawnComplete":
				OnComplete();
				break;
		}
	}

	public override void End() { }

	public override void Update() { }
}
