using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GuardEvents
{
	private Action<Guard, float, float> onHit = delegate { };
	private Action<float, Vector3> onDamageDealt = delegate { };
	private Action<Guard> onKilled = delegate { };
	private Action<Guard> onDeathComplete = delegate { };
	private Action<Guard, GuardController> onSpawned = delegate { };
	private Action<Guard> onSpawnerHasRespawned = delegate { };

	public void AddHitListener(Action<Guard, float, float> listener)
	{
		onHit += listener;
	}

	public void Hit(Guard guard, float health, float maxHealth)
	{
		onHit(guard, health, maxHealth);
	}

	public void AddDamageDealtListener(Action<float, Vector3> listener)
	{
		onDamageDealt += listener;
	}

	public void DealDamage(float amount, Vector3 damageLocation)
	{
		onDamageDealt(amount, damageLocation);
	}

	public void AddKilledListener(Action<Guard> listener)
	{
		onKilled += listener;
	}

	public void Kill(Guard guard)
	{
		onKilled(guard);
	}

	public void AddDeathCompleteListener(Action<Guard> listener)
	{
		onDeathComplete += listener;
	}

	public void DeathComplete(Guard guard)
	{
		onDeathComplete(guard);
	}

	public void AddSpawnedListener(Action<Guard, GuardController> listener)
	{
		onSpawned += listener;
	}

	public void Spawn(Guard guard, GuardController controller)
	{
		onSpawned(guard, controller);
	}

	public void AddSpawnerHasRespawnedListener(Action<Guard> listener)
	{
		onSpawnerHasRespawned += listener;
	}

	public void SpawnerHasRespawned(Guard guard)
	{
		onSpawnerHasRespawned(guard);
	}
}
