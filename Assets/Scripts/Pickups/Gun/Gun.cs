using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : IEquippableMain
{
	private GameObject gameObject;
	private Transform transform;
	private IGunAmmo ammo;
	private Action<Transform, ProjectileType> onProjectileSpawned = delegate { };
	private Action<Guard, ItemType, Vector3> onCollidedWithGuard = delegate { };
	private Action<ItemType, Vector3> onCollidedWithEnvironment = delegate { };
	private Action<ItemType> onItemConsumed = delegate { };
	private Action onUsed = delegate { };
	private Collider[] colliders;
	private Animator animator;

	private PlayerObjectData playerObjectData;
    private bool attackActive;
	private GunTargetLocator gunTargetLocator;
	private PlayerEvents playerEvents;
	private PlayerInputBroadcaster inputBroadcaster;
    private Transform bulletStart;
    private Target currentTarget;
	private ProjectilePool projectilePool;
	private AnimationListener animationListener;

	public Gun(GameObject gameObject, PlayerObjectData playerObjectData, 
		GunTargetLocator gunTargetLocator, PlayerEvents playerEvents,
		PlayerInputBroadcaster inputBroadcaster, ProjectilePool projectilePool)
    {
        this.gameObject = gameObject;
		this.transform = gameObject.transform;
		this.playerObjectData = playerObjectData;
		this.gunTargetLocator = gunTargetLocator;
		this.playerEvents = playerEvents;
		this.inputBroadcaster = inputBroadcaster;
		this.projectilePool = projectilePool;

        animator = playerObjectData.PlayerAnimator;
        bulletStart = playerObjectData.Blaster.Find("BulletStart");

		colliders = gameObject.GetComponentsInChildren<Collider>();

		animationListener = playerObjectData.PlayerAnimator.gameObject.GetComponent<AnimationListener>();
	}

    public void Update() { }

    public void Destroy()
	{
		gameObject.SetActive(false);
    }

    public void StartUse()
    {
		AttackStart();

		Target target = FindTarget();
		playerEvents.OnShotTargetSet.Invoke(target.Position, () =>
		{
			animator.SetTrigger("Shoot");
		});

		currentTarget = target;
	}

    private void FireBullet()
    {
		if (ammo != null)
		{
			ItemType ammoType = ammo.Type;
			ProjectileType projectileType = ammo.ProjectileType;
			IProjectile projectile = projectilePool.Get(ammo.ProjectileType);
			projectile.OnSubProjectileSpawned += (subProj) => ProjectileSpawned(subProj, projectileType);
			projectile.OnCollidedWithTarget += (other, collisionPoint) => OnCollidedWithGuard(other.GetComponent<Guard>(), ammoType, collisionPoint);
			projectile.OnCollidedWithEnvironment += (collisionPoint) => OnCollidedWithEnvironment(ammoType, collisionPoint);
			ProjectileSpawned(projectile.Transform, ammo.ProjectileType);

			Vector3 directionToEnemy = currentTarget.Position - transform.position;
			projectile.Fire(bulletStart, transform.position, directionToEnemy);

			OnUsed();
			OnItemConsumed(ammo.Type);
		}
	}

    private void AttackStart()
    {
        attackActive = true;
		inputBroadcaster.EnableActions(ControlType.None);
    }

    private void AttackEnd()
    {
        attackActive = false;
		inputBroadcaster.EnableActions(ControlType.Gameplay);
    }

	public void EndUse() { }

    private Target FindTarget()
    {
        Transform closestEnemy = null;
		gunTargetLocator.Locate((closest) => closestEnemy = closest);

		if(closestEnemy != null)
		{
			return new TransformTarget(closestEnemy);
		}
		else
		{
			return new VectorTarget(transform.position + transform.forward);
		}
    }

    public void Equip(IEquippable currentMainEquipment, Transform equipmentHolder)
	{
		animationListener.OnAnimationEvent += OnAnimationEvent;
		gameObject.SetActive(true);
    }

    public void Unequip(IEquippable currentMainEquipment)
	{
		animationListener.OnAnimationEvent -= OnAnimationEvent;
		gameObject.SetActive(false);
    }

	private void OnAnimationEvent(string param)
	{
		switch (param)
		{
			case "AttackEnd":
				AttackEnd();
				break;
			case "Fire":
				FireBullet();
				break;
		}
	}

	private void ProjectileSpawned(Transform projectile, ProjectileType type)
	{
		IgnoreCollisionsWith(projectile);
		OnProjectileSpawned(projectile, type);
	}

	private void IgnoreCollisionsWith(Transform other)
	{
		Collider[] projectileColliders = other.GetComponentsInChildren<Collider>();

		foreach (Collider collider in colliders)
		{
			foreach (Collider projectileCollider in projectileColliders)
			{
				Physics.IgnoreCollision(collider, projectileCollider);
			}
		}
	}


	public bool CanEquipAsMain(IEquippableMain currentMainEquipment)
	{
		return true;
	}

	public bool CanEquipAsSecondary(IEquippableMain currentMainEquipment, IEquippable currentSecondaryEquipment)
	{
		return false;
	}

	public void ChangeAmmo(IGunAmmo ammo)
	{
		this.ammo = ammo;
	}

	public bool CanUse()
	{
		return ammo != null && attackActive == false;
	}

	public bool IsRequiredToCompleteLevel { get { return false; } }

	public ItemType Type { get { return ItemType.Gun; } }

	public Action<Transform, ProjectileType> OnProjectileSpawned { get { return onProjectileSpawned; } set { onProjectileSpawned = value; } }
	public Action<ItemType, Vector3> OnCollidedWithEnvironment { get { return onCollidedWithEnvironment; } set { onCollidedWithEnvironment = value; } }
	public Action<Guard, ItemType, Vector3> OnCollidedWithGuard { get { return onCollidedWithGuard; } set { onCollidedWithGuard = value; } }
	public Action<ItemType> OnItemConsumed { get { return onItemConsumed; } set { onItemConsumed = value; } }
	public Action OnUsed { get { return onUsed; } set { onUsed = value; } }

	private abstract class Target
	{
		public abstract Vector3 Position { get; }
	}

	private class VectorTarget : Target
	{
		private Vector3 value;

		public VectorTarget(Vector3 value)
		{
			this.value = value;
		}
		public override Vector3 Position { get { return value; } }
	}
	

	private class TransformTarget : Target
	{
		private Transform value;

		public TransformTarget(Transform value)
		{
			this.value = value;
		}

		public override Vector3 Position { get { return value.position; } }
	}
}

