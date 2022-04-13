using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardGunController
{
	public Action<Transform> OnProjectileSpawned = delegate { };
	private IGunAmmo ammo;
	private Transform bulletStart;
	private Transform guardGunTransform;
	private Transform player;

	public Action<Vector3> OnCollidedWithPlayer = delegate { };
	private List<Collider> colliders;
	private ProjectilePool projectilePool;

	public GuardGunController(Transform guardGunTransform,
		Transform guard, Transform player, 
		Transform bulletStart, ProjectilePool projectilePool)
    {
		this.guardGunTransform = guardGunTransform;
		this.player = player;
		this.bulletStart = bulletStart;
		this.projectilePool = projectilePool;

		ammo = new GuardAmmo();
		colliders = GetAllGuardColliders(guard);
	}

	private List<Collider> GetAllGuardColliders(Transform root)
	{
		List<Collider> allColliders = new List<Collider>();
		allColliders.AddRange(root.GetComponentsInChildren<Collider>());
		allColliders.Add(root.GetComponent<Collider>());

		Transform damageArea = root.Find("DamageArea");
		if(damageArea != null)
		{
			damageArea.gameObject.SetActive(true);
			allColliders.Add(damageArea.GetComponent<Collider>());
			damageArea.gameObject.SetActive(false);
		}
		return allColliders;
	}

	public void FireBullet()
	{
		if (ammo != null)
		{
			IProjectile projectile = projectilePool.Get(ammo.ProjectileType);
			projectile.OnCollidedWithTarget += (other, collisionPoint) => OnPlayerHit(collisionPoint);
			ProjectileSpawned(projectile.Transform);

			Vector3 directionToEnemy = player.position - guardGunTransform.position;
			projectile.Fire(bulletStart, guardGunTransform.position, directionToEnemy);
		}
	}
	private void ProjectileSpawned(Transform projectile)
	{
		IgnoreCollisionsWith(projectile);
		OnProjectileSpawned(projectile);
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

	private void OnPlayerHit(Vector3 collisionPoint)
	{
		OnCollidedWithPlayer.Invoke(collisionPoint);
	}
}
