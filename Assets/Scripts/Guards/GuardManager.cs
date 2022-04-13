using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardManager
{
	private Dictionary<Guard, GuardController> guards;
	private GuardEvents guardEvents;
	private ProjectilePool projectilePool;
	private PlayerObjectData playerObjectData;
	private GameObject player;
	private List<GuardSpawnerController> spawners;
	public Action<int> OnGuardsSetup = delegate { };	//S3 - Assignment 02

	public GuardManager(GuardEvents guardEvents, ProjectilePool projectilePool)
	{
		this.guardEvents = guardEvents;
		this.projectilePool = projectilePool;
		
		guards = new Dictionary<Guard, GuardController>();
		spawners = new List<GuardSpawnerController>();
	}

	public void OnLevelLoaded(Transform levelObjects)
	{
		player = levelObjects.Find("Player").gameObject;
		playerObjectData = player.GetComponent<PlayerObjectData>();

		Guard[] guards = levelObjects.GetComponentsInChildren<Guard>();		//S3 - Assignment 02
		GuardSpawner[] spawners = levelObjects.GetComponentsInChildren<GuardSpawner>();		//S3 - Assignment 02

		int totalGuards = guards.Length + spawners.Length;		//S3 - Assignment 02

		foreach (Guard guard in guards) //levelObjects.GetComponentsInChildren<Guard>())	//S3 - Assignment 02
		{
			SpawnGuard(guard);
		}

		foreach(GuardSpawner spawner in spawners) //levelObjects.GetComponentsInChildren<GuardSpawner>())	//S3 - Assignment 02
		{
			SetUpSpawner(spawner);
		}
	}

	private void SpawnGuard(Guard guard)
	{
		GuardController guardController = new GuardController(guard, player, 
			playerObjectData.Head, projectilePool);
		guardController.OnDamageTaken += (damagedGuard, health, maxHealth) => guardEvents.Hit(damagedGuard, health, maxHealth);
		guardController.OnDamageDealt += (damageAmount, damageLocation) => guardEvents.DealDamage(damageAmount, damageLocation);
		guardController.OnKilled += () => guardEvents.Kill(guard);
		guardController.OnDeathComplete += () => guardEvents.DeathComplete(guard);
		guardController.OnDespawned += () =>
		{
			guards.Remove(guard);
			GameObject.Destroy(guard.gameObject);
		};
		
		guards[guard] = guardController;

		guardEvents.Spawn(guard, guardController);
	}

	private void SetUpSpawner(GuardSpawner spawner)
	{
		GuardSpawnerController spawnerController = new GuardSpawnerController(spawner, guardEvents);
		spawnerController.OnGuardSpawned += (guard) => SpawnGuard(guard);
		spawnerController.OnGuardRespawned += (guard) => guardEvents.SpawnerHasRespawned(guard);

		spawnerController.OnGuardDespawnRequested += (guard) => guards[guard].StartDespawn();
		spawners.Add(spawnerController);
	}

	public void ForceIdle()
	{
		foreach (KeyValuePair<Guard, GuardController> guard in guards)
		{
			guard.Value.ForceIdle();
		}
	}

	public void Update()
	{
		foreach(KeyValuePair<Guard, GuardController> guard in guards)
		{
			guard.Value.Update();
		}

		foreach(GuardSpawnerController spawner in spawners)
		{
			spawner.Update();
		}
	}

	public IReadOnlyDictionary<Guard, GuardController> Guards
	{
		get
		{
			return guards;
		}
	}
}
