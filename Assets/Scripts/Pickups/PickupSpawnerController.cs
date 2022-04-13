using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PickupSpawnerController
{
	public Action<Pickup> OnPickupSpawned = delegate { };
	private PickupSpawner spawner;
	private float coolDown;
	private Pickup heldPickup;
	private PickupPool pool;

	public PickupSpawnerController(PickupSpawner spawner, PickupEvents events,
		PickupPool pool)
	{
		this.spawner = spawner;
		this.pool = pool;
		events.OnPickupCollected += OnPlayerPickedUp;
	}

	public void Update()
	{
		if(heldPickup != null)
		{
			return;
		}

		coolDown -= Time.deltaTime;

		if(coolDown <= 0)
		{
			coolDown = spawner.cooldown;
			SpawnPickup();
		}
	}

	private void SpawnPickup()
	{
		heldPickup = pool.Get(spawner.type);
		heldPickup.transform.position = spawner.container.transform.position;
		OnPickupSpawned(heldPickup);
	}

	private void OnPlayerPickedUp(Pickup pickup)
	{
		if(pickup == heldPickup)
		{
			heldPickup = null;
		}
	}
}
