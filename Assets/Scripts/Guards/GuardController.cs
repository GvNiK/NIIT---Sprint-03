using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController
{
	public Action<float, Vector3> OnDamageDealt = delegate { };
	public Action<Guard, float, float> OnDamageTaken = delegate { };
	public Action OnSuspicionUpgraded = delegate { };
	public Action OnKilled = delegate { };
	public Action OnDeathComplete = delegate { };
	public Action OnDespawned = delegate { };
	public Action<Transform> OnProjectileSpawned = delegate { };

	private Guard guard;
	private GuardAnimator animatorController;
	private GuardVision visionController;
	private GuardSuspicion suspicionController;
	private GuardHealth healthController;
	private GuardBehaviour currentBehaviour;
	private GuardEmoteController emoteController;
	private GuardGunController guardGunController;
	private GameObject damageCollider;
	private NavMeshAgent meshAgent;
	private Animator animator;

	public GuardController(Guard guard, GameObject player, 
		Transform playerTarget, ProjectilePool projectilePool)
	{
		this.guard = guard;
		Transform transform = guard.transform;
		List<GameObject> objectsToCheckAgainst = new List<GameObject>();
		objectsToCheckAgainst.Add(player);

		animator = guard.GetComponent<Animator>();
		meshAgent = guard.GetComponent<NavMeshAgent>();
		damageCollider = guard.transform.Find("DamageArea").gameObject;

		animatorController = new GuardAnimator(meshAgent, animator, guard);

		visionController = new GuardVision(guard, guard.visionData, objectsToCheckAgainst);

		healthController = new GuardHealth(guard.generalData);
		healthController.OnDamageTaken += (currentHealth, maxHealth) => OnDamageTaken(guard, currentHealth, maxHealth);
		healthController.OnKilled += GuardKilled;

		guard.OnDamageTaken += (damageAmount, damageInstigator) => healthController.TakeDamage(damageAmount);

		suspicionController = new GuardSuspicion(visionController, guard.suspicionData, guard, healthController);
		suspicionController.OnSuspicionStateUpdated += UpdateSuspicionState;

		emoteController = new GuardEmoteController(suspicionController, guard.transform.Find("Emotes"));

		guardGunController = new GuardGunController(guard.blaster, guard.transform,
			playerTarget, guard.blaster.Find("BulletStart"), projectilePool);
		guardGunController.OnCollidedWithPlayer += (guardPosition) => OnDamageDealt.Invoke(guard.generalData.attackDamage, guardPosition);
		guardGunController.OnProjectileSpawned += (proj) => OnProjectileSpawned(proj);

		currentBehaviour = new GuardPatrolBehaviour(meshAgent, guard.patrolData.waypoints, animatorController, guard.generalData.patrolMoveSpeed);
		currentBehaviour.Begin();
	}

	private void UpdateSuspicionState(SuspicionState newState, SuspicionState oldState, Transform alertingObject)
	{
		currentBehaviour.End();
		if(healthController.IsAlive)
		{
			switch (newState)
			{
				case SuspicionState.Patrolling:
					currentBehaviour = new GuardPatrolBehaviour(meshAgent, guard.patrolData.waypoints, animatorController, guard.generalData.patrolMoveSpeed);
					break;
				case SuspicionState.Investigating:
					break;
				case SuspicionState.Pursuing:
					currentBehaviour = new GuardPursueBehaviour(meshAgent, alertingObject, guard.visionData, damageCollider, animatorController, guardGunController, guard.generalData.pursueMoveSpeed);
					break;
				default:
					return;
			}
			currentBehaviour.Begin();

			if (newState > oldState)
			{
				OnSuspicionUpgraded();
			}
		}
	}

	private void GuardKilled()
	{
		suspicionController.OnSuspicionStateUpdated -= UpdateSuspicionState;
		GuardDeathBehaviour death = new GuardDeathBehaviour(meshAgent, 
			animator, visionController, guard.modelData);
		death.OnComplete += () =>
		{
			OnDeathComplete();
		}; 
		emoteController.HideEmotes();
		currentBehaviour = death;
		currentBehaviour.Begin();
		emoteController.HideEmotes();
		OnKilled();
	}

	public void StartDespawn()
	{
		suspicionController.OnSuspicionStateUpdated -= UpdateSuspicionState;
		GuardDespawnBehaviour despawn = new GuardDespawnBehaviour(meshAgent, animator);
		despawn.OnComplete += () => OnDespawned();
		currentBehaviour = despawn;
		currentBehaviour.Begin();
	}

	public void ForceIdle()
	{
		if(healthController.IsAlive)
		{
			currentBehaviour.End();
			suspicionController.OnSuspicionStateUpdated -= UpdateSuspicionState;
			currentBehaviour = new ForcedIdleBehaviour(meshAgent, animator, visionController);
			currentBehaviour.Begin();
		}
	}

	public void Update()
	{
		animatorController.Update();
		visionController.Update();
		currentBehaviour.Update();
	}

	public bool CanBeTargeted
	{
		get
		{
			return (currentBehaviour is GuardDeathBehaviour) == false &&
				(currentBehaviour is GuardDespawnBehaviour) == false;
		}
	}
}