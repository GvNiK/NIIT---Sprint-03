using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardDeathBehaviour : GuardBehaviour
{
	public Action OnComplete = delegate { };
	private Animator animator;
	private NavMeshAgent meshAgent;
	private AnimationListener animListener;
	private GuardVision vision;
	private Guard.ModelData modelData;

	public GuardDeathBehaviour(NavMeshAgent meshAgent, Animator animator, 
		GuardVision vision, Guard.ModelData modelData)
	{
		this.animator = animator;
		this.meshAgent = meshAgent;
		this.vision = vision;
		this.modelData = modelData;
		animListener = meshAgent.GetComponent<AnimationListener>();
	}

	public override void Begin()
	{
		meshAgent.ResetPath();
		meshAgent.isStopped = true;
		meshAgent.velocity = Vector3.zero;
		animator.SetBool("IsDead", true);
		vision.Disable();
		foreach(ParticleSystem particles in modelData.aliveParticles)
		{
			particles.Stop();
		}
		animListener.OnAnimationEvent += OnAnimationEvent;
	}

	private void OnAnimationEvent(string parameter)
	{
		switch(parameter)
		{
			case "DeathComplete":
				OnComplete();
				break;
		}
	}

	public override void End()
	{
		meshAgent.isStopped = false;
		animator.SetBool("IsDead", false);
		animListener.OnAnimationEvent -= OnAnimationEvent;
	}

	public override void Update() { }
}
