using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : IEquippableMain
{
	private Animator animator;
	private GameObject gameObject;
	private CollisionCallbacks collisionCallbacks;
	private PlayerSettings playerSettings;
	private PlayerObjectData playerObjectData;
    private bool chainAttacks;
    private TrailRenderer swordTrailRenderer;
	private AnimationListener animationListener;
	private Action<ItemType> onItemConsumed = delegate { };
	private Action<ItemType, Vector3> onCollidedWithEnvironment = delegate { };
	private Action<Guard, ItemType, Vector3> onCollidedWithGuard = delegate { };
	private Action onUsed = delegate { };

	public Sword(GameObject gameObject, PlayerSettings playerSettings, PlayerObjectData playerObjectData)
    {
        this.playerObjectData = playerObjectData;
        this.gameObject = gameObject;
        this.playerSettings = playerSettings;

        animator = playerObjectData.PlayerAnimator;
        collisionCallbacks = gameObject.GetComponentInChildren<CollisionCallbacks>(true);
		collisionCallbacks.gameObject.SetActive(false);
        swordTrailRenderer = gameObject.GetComponentInChildren<TrailRenderer>();
		animationListener = animator.gameObject.GetComponent<AnimationListener>();
	}


    public void Equip(IEquippable currentMainEquipment, Transform equipmentHolder)
    {
        gameObject.SetActive(true);
        collisionCallbacks.OnTriggerEntered += SwordCollision;
		animationListener.OnAnimationEvent += OnAnimationEvent;
    }

    public void Unequip(IEquippable currentMainEquipment)
    {
        gameObject.SetActive(false);
        collisionCallbacks.OnTriggerEntered -= SwordCollision;
		animationListener.OnAnimationEvent -= OnAnimationEvent;
	}

	private void OnAnimationEvent(string param)
	{
		switch (param)
		{
			case "SwipeOneStart":
			case "SwipeTwoStart":
				AttackStart();
				break;
			case "SwipeOneEnd":
			case "SwipeTwoEnd":
				AttackEnd();
				break;
		}
	}

	public void StartUse()
	{
		if (animator.GetBool("Swipe"))
		{
			chainAttacks = true;
			return;
		}

		animator.SetBool("Swipe", true);
	}
	public void EndUse() { }

	public void Update() { }

	private void AttackStart()
	{
		swordTrailRenderer.emitting = true;
		collisionCallbacks.gameObject.SetActive(true);
		OnUsed();
	}

	private void AttackEnd()
	{
		swordTrailRenderer.emitting = false;
		collisionCallbacks.gameObject.SetActive(false);

		if (chainAttacks)
		{
			chainAttacks = false;
		}
		else
		{
			animator.SetBool("Swipe", false);
		}
	}

    private void SwordCollision(Collider collider)
	{
		Vector3 closestPoint = collider.ClosestPointOnBounds(gameObject.transform.position);
		if (collider.transform.tag.Equals("Enemy"))
        {
			Guard guard = collider.transform.GetComponent<Guard>();
			guard.TakeDamage(playerSettings.SwordDamage, playerObjectData.transform);
			OnCollidedWithGuard(guard, Type, closestPoint);
			AttackEnd();
        }
		else
		{
			OnCollidedWithEnvironment(ItemType.Melee, closestPoint);
		}
    }

	public bool CanUse()
	{
		return animator.GetBool("Swipe") == false || chainAttacks == false;
	}

	public bool CanEquipAsMain(IEquippableMain currentMainEquipment)
	{
		return true;
	}

	public bool CanEquipAsSecondary(IEquippableMain currentMainEquipment, IEquippable currentSecondaryEquipment)
	{
		return false;
	}

	public void Destroy() { }

	public Action<Transform, ProjectileType> OnProjectileSpawned { get { return delegate { }; } set { } }
	public Action<ItemType> OnItemConsumed { get { return onItemConsumed; } set { onItemConsumed = value; } }

	public ItemType Type { get { return ItemType.Melee; } }

	public Action<ItemType, Vector3> OnCollidedWithEnvironment { get { return onCollidedWithEnvironment; } set { onCollidedWithEnvironment = value; } }
	public Action<Guard, ItemType, Vector3> OnCollidedWithGuard { get { return onCollidedWithGuard; } set { onCollidedWithGuard = value; } }
	public Action OnUsed { get { return onUsed; } set { onUsed = value; } }
}
