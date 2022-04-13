using UnityEngine;
using System;

public class DamageAmmoProjectile : IProjectile
{
	private Transform owner;
	private const float FIRE_FORCE = 15f;

	public Action<Collider, Vector3> onCollidedWithTarget = delegate { };
	public Action<Vector3> onCollidedWithEnvironment = delegate { };
	public Action onDestroyRequested = delegate { };

	private ProjectileBehaviour projectileBehaviour;
	private ParticleSystem[] particles;
	private Transform transform;
	private PlayerSettings playerSettings;

	public DamageAmmoProjectile(Transform transform, PlayerSettings playerSettings)
	{
		this.transform = transform;
		this.playerSettings = playerSettings;
		projectileBehaviour = new ProjectileBehaviour(transform, "Enemy");
		particles = transform.GetComponentsInChildren<ParticleSystem>();
		projectileBehaviour.OnCollisionWithTarget += (other, collisionPos) => CollisionWithTarget(other, collisionPos);
		projectileBehaviour.OnCollisionWithEnvironment += (other, collisionPos) => CollisionWithEnvironment(other, collisionPos);
		projectileBehaviour.OnDestroyRequested += () => OnDestroyRequested();
	}

	public void Fire(Transform owner, Vector3 position, Vector3 forward)
	{
		this.owner = owner;
		projectileBehaviour.Fire(position, forward, FIRE_FORCE);

		foreach(ParticleSystem particle in particles)
		{
			particle.Play();
		}
	}

	private void CollisionWithTarget(Collider other, Vector3 collisionPos)
	{
		Guard guard = other.GetComponent<Guard>();
		guard.TakeDamage(playerSettings.DamageAmmoDamage, owner);
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

	public bool IsRequiredToCompleteLevel { get { return false; } }

	public ProjectileType Type { get { return ProjectileType.Damage; } }

	public Action<Transform> OnSubProjectileSpawned { get { return delegate{}; } set {} }
	public Action<Vector3> OnCollidedWithEnvironment { get { return onCollidedWithEnvironment; } set { onCollidedWithEnvironment = value; } }
	public Action<Collider, Vector3> OnCollidedWithTarget { get { return onCollidedWithTarget; } set { onCollidedWithTarget = value; } }

	public Transform Transform { get { return transform; } }

	public Action OnDestroyRequested { get { return onDestroyRequested; } set { onDestroyRequested = value; } }
}
