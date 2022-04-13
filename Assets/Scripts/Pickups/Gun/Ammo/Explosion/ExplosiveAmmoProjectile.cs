using UnityEngine;
using System;

public class ExplosiveAmmoProjectile : IProjectile
{
	private const float FIRE_FORCE = 10f;

	public Action<Transform> onSubProjectileSpawned = delegate { };
	public Action<Collider, Vector3> onCollidedWithTarget = delegate { };
	public Action<Vector3> onCollidedWithEnvironment = delegate { };
	public Action onDestroyRequested = delegate { };

	private Transform owner;
	private ProjectileBehaviour projectileBehaviour;
	private Transform transform;
	private ProjectilePool projectilePool;

	public ExplosiveAmmoProjectile(Transform transform, ProjectilePool projectilePool)
	{
		this.transform = transform;
		this.projectilePool = projectilePool;
		projectileBehaviour = new ProjectileBehaviour(transform, "Enemy");
		projectileBehaviour.OnCollision += Collision;
		projectileBehaviour.OnCollisionWithTarget += (other, collisionPos) => CollisionWithTarget(other, collisionPos);
		projectileBehaviour.OnCollisionWithEnvironment += (other, collisionPos) => CollisionWithEnvironment(other, collisionPos);
		projectileBehaviour.OnDestroyRequested += () => OnDestroyRequested();
	}

	private void StartExplosion(Vector3 collisionPos)
	{
		IProjectile explosion = projectilePool.Get(ProjectileType.SubExplosion);
		OnSubProjectileSpawned(explosion.Transform);
		explosion.Fire(owner, collisionPos, Vector3.up);
	}

	public void Fire(Transform owner, Vector3 position, Vector3 forward)
	{
		this.owner = owner;
		projectileBehaviour.Fire(position, forward, FIRE_FORCE);
	}

	private void Collision(Collider other, Vector3 collisionPos)
	{
		StartExplosion(collisionPos);
	}

	private void CollisionWithTarget(Collider other, Vector3 collisionPos)
	{
		Guard guard = other.GetComponent<Guard>();
		OnCollidedWithTarget(other, collisionPos);
	}

	private void CollisionWithEnvironment(Collider other, Vector3 collisionPos)
	{
		OnCollidedWithEnvironment(collisionPos);
	}

	public void Update()
	{
		projectileBehaviour.Update();
	}

	public Action<Transform> OnSubProjectileSpawned { get { return onSubProjectileSpawned; } set { onSubProjectileSpawned = value; } }
	public Action<Vector3> OnCollidedWithEnvironment { get { return onCollidedWithEnvironment; } set { onCollidedWithEnvironment = value; } }
	public Action<Collider, Vector3> OnCollidedWithTarget { get { return onCollidedWithTarget; } set { onCollidedWithTarget = value; } }
	public Action OnDestroyRequested { get { return onDestroyRequested; } set { onDestroyRequested = value; } }

	public Transform Transform {  get { return transform; } }

	public ProjectileType Type { get { return ProjectileType.Explosive; } }
}
