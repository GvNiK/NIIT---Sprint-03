using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GuardSpawnerController
{
	public Action<Guard> OnGuardSpawned = delegate { };
	public Action<Guard> OnGuardRespawned = delegate { };
	public Action<Guard> OnGuardDespawnRequested = delegate { };
	private GuardSpawner spawner;
	private float coolDown;
	private Guard currentGuard;
	private bool isRespawning;

	public GuardSpawnerController(GuardSpawner spawner, GuardEvents events)
	{
		this.spawner = spawner;

		events.AddDeathCompleteListener(GuardKilled);
	}

	private void GuardKilled(Guard guard)
	{
		if(currentGuard == guard)
		{
			currentGuard = null;
			isRespawning = true;
			OnGuardDespawnRequested(guard);
		}
	}

	public void Update()
	{
		if (currentGuard != null || spawner.respawnOnDeath == false)
		{
			return;
		}

		coolDown -= Time.deltaTime;

		if (coolDown <= 0)
		{
			SpawnGuard();
		}
	}

	private void SpawnGuard()
	{
		coolDown = spawner.cooldown;

		GameObject instance = GameObject.Instantiate(spawner.prefab, spawner.container.transform, true);
		currentGuard = instance.GetComponent<Guard>();
		OnGuardSpawned(currentGuard);

		if(isRespawning)
		{
			OnGuardRespawned(currentGuard);
		}
	}
}
