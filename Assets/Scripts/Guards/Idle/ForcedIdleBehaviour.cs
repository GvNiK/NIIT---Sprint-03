using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForcedIdleBehaviour : GuardBehaviour
{
	private Animator animator;
	private NavMeshAgent meshAgent;
	private GuardVision vision;

	public ForcedIdleBehaviour(NavMeshAgent meshAgent, Animator animator, GuardVision vision)
	{
		this.animator = animator;
		this.meshAgent = meshAgent;
		this.vision = vision;
	}

	public override void Begin()
	{
		vision.Disable();
		meshAgent.ResetPath();
		meshAgent.isStopped = true;
		meshAgent.velocity = Vector3.zero;
	}

	public override void End() 
	{
		vision.Enable();
	}

	public override void Update() { }
}
