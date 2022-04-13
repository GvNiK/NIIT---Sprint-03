using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PickupDropController
{
	private const float DROP_COOLDOWN = 2f;
	private const float DROP_FORCE = 3f;
	public Action<Pickup> OnPickupDropped = delegate { };
	private Dictionary<Pickup, int> droppedItems;

	private List<CoolDown> cooldowns;
	private PlayerController playerController;
	private PickupPool pool;

	public PickupDropController(PlayerController playerController, PickupPool pool)
	{
		this.playerController = playerController;
		this.pool = pool;
		cooldowns = new List<CoolDown>();
		droppedItems = new Dictionary<Pickup, int>();
	}

	public void Drop(ItemType type, int count)
	{
		Pickup pickup = pool.Get(type);
		pickup.quantity = count;
		pickup.gameObject.SetActive(true);
		droppedItems.Add(pickup, count);

		Rigidbody rigidbody = pickup.GetComponent<Rigidbody>();
		AnimatePickup animatePickup = pickup.GetComponentInChildren<AnimatePickup>();
		ParticleSystem.EmissionModule pickupEmission = pickup.GetComponentInChildren<ParticleSystem>().emission;
		cooldowns.Add(new CoolDown(pickup, () =>
		{
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
			animatePickup.ResetHeight();
			animatePickup.IsAnimating = true;
			pickupEmission.enabled = true;
		}));

		OnPickupDropped(pickup);
	}

	public void Update()
	{
		for(int i = cooldowns.Count - 1; i>= 0; i--)
		{
			cooldowns[i].time -= Time.deltaTime;
			if(cooldowns[i].time <= 0)
			{
				cooldowns[i].OnComplete();
				cooldowns.RemoveAt(i);
			}
		}
	}

	public void DropAll()
	{
		foreach(KeyValuePair<Pickup, int> item in droppedItems)
		{
			item.Key.transform.position = playerController.Position;

			AnimatePickup animatePickup = item.Key.GetComponentInChildren<AnimatePickup>();
			animatePickup.IsAnimating = false;

			ParticleSystem.EmissionModule pickupEmission = item.Key.GetComponentInChildren<ParticleSystem>().emission;
			pickupEmission.enabled = false;

			Rigidbody rigidBody = item.Key.GetComponent<Rigidbody>();
			rigidBody.useGravity = true;
			rigidBody.isKinematic = false;
			rigidBody.AddForce((playerController.Forward * DROP_FORCE) + Vector3.up, ForceMode.Impulse);
		}

		droppedItems.Clear();
	}

	public bool CanPickup(Pickup pickup)
	{
		foreach(CoolDown cooldown in cooldowns)
		{
			if(cooldown.pickup == pickup)
			{
				return false;
			}
		}

		return true;
	}

	private class CoolDown
	{
		public Pickup pickup;
		public float time;
		public Action OnComplete = delegate { };

		public CoolDown(Pickup pickup, Action onComplete)
		{
			OnComplete = onComplete;
			this.pickup = pickup;
			this.time = DROP_COOLDOWN;
		}
	}
}
