using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController
{
	private List<Pickup> requiredPickups;
	private PlayerController playerController;
	private PickupDropController dropController;
	private PickupEvents events;
	private List<Pickup> activePickups;
	private List<PickupSpawnerController> pickupSpawners;
	private float killZoneHeight = -1000.0f;
	private int totalRequiredPickups;
	private PickupPool pool;
	private PickupFactory factory;

	public PickupController(Transform levelObjects, PlayerController playerController,
		PickupEvents events, PickupLibrary pickupLibrary)
	{
		this.playerController = playerController;
		this.events = events;

		factory = new PickupFactory(pickupLibrary);

		pool = new PickupPool(factory);

		dropController = new PickupDropController(playerController, pool);
		dropController.OnPickupDropped += (pickup) =>
		{
			events.OnPickupDropped();
			Setup(pickup);
		};

		FindPickupsInLevel(levelObjects);
	}

    private void FindPickupsInLevel(Transform levelObjects)
	{
		requiredPickups = new List<Pickup>();
		activePickups = new List<Pickup>();
		pickupSpawners = new List<PickupSpawnerController>();

		Transform pickupRoot = levelObjects.Find("Pickups");

		if(pickupRoot == null)
		{
			return;
		}

		foreach(Transform pickupTrans in pickupRoot)
		{
			Pickup pickup = pickupTrans.GetComponent<Pickup>();

			if(pickup != null)
			{
				Setup(pickup);
			}
			else
			{
				PickupSpawner spawner = pickupTrans.GetComponent<PickupSpawner>();
				Setup(spawner);
			}
		}

		events.OnRemainingPickupCountUpdated(requiredPickups.Count);
	}

	private void Setup(Pickup pickup)
	{
		CollisionCallbacks collisionCallbacks = pickup.GetComponentInChildren<CollisionCallbacks>();
		activePickups.Add(pickup);

		Action<Collider> OnPickupCollision = null;
		OnPickupCollision = (other) =>
		{
			if (playerController.OwnsCollider(other) &&
				dropController.CanPickup(pickup))
			{
				collisionCallbacks.OnTriggerEntered -= OnPickupCollision;
				activePickups.Remove(pickup);
				if (requiredPickups.Contains(pickup))
				{
					requiredPickups.Remove(pickup);
					events.OnRemainingPickupCountUpdated(requiredPickups.Count);
				}
				events.OnPickupCollected(pickup);
				pool.Return(pickup);
			}
		};

		collisionCallbacks.OnTriggerEntered += OnPickupCollision;

		if (pickup.isRequiredToFinishLevel)
		{
			requiredPickups.Add(pickup);
			totalRequiredPickups++;
		}
	}

	private void Setup(PickupSpawner spawner)
	{
		PickupSpawnerController spawnerController = new PickupSpawnerController(spawner, events, pool);
		spawnerController.OnPickupSpawned += (pickup) =>
		{
			Setup(pickup);
		};
		pickupSpawners.Add(spawnerController);
	}

	public void Update()
	{
		dropController.Update();
		CheckPickupsBelowKillZ();
		foreach(PickupSpawnerController spawnerController in pickupSpawners)
		{
			spawnerController.Update();
		}
	}

	public void Drop(ItemType type, int count)
	{
		dropController.Drop(type, count);
	}

	public void PerformDrop()
	{
		dropController.DropAll();
	}

	private void CheckPickupsBelowKillZ()
	{
		for (int i = activePickups.Count - 1; i >= 0; i--)
		{
			Pickup pickup = activePickups[i];
			if (pickup.transform.position.y < killZoneHeight)
			{
				activePickups.Remove(pickup);
				pool.Return(pickup);
			}
		}
	}

	public int TotalRequiredPickups
	{
		get
		{
			return totalRequiredPickups;
		}
	}
}
