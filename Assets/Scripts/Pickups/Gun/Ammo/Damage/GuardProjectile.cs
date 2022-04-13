using UnityEngine;
using System;

public class GuardProjectile : IProjectile
{
	private Transform owner;
	private const float FIRE_FORCE = 15f;
	private const float DAMAGE = 10f;

	public Action<Collider, Vector3> onCollidedWithTarget = delegate { };
	public Action<Vector3> onCollidedWithEnvironment = delegate { };
	public Action onDestroyRequested = delegate { };

	private ProjectileBehaviour projectileBehaviour;
	private ParticleSystem[] particles;
	private Transform transform;

	public GuardProjectile(Transform transform)
	{
		this.transform = transform;
		projectileBehaviour = new ProjectileBehaviour(transform, "Player");
		particles = transform.GetComponentsInChildren<ParticleSystem>();
		projectileBehaviour.OnCollisionWithTarget += (other, collisionPos) => OnCollidedWithTarget(other, collisionPos);
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

	public void Update()
	{
		projectileBehaviour.Update();
	}

	private void CollisionWithEnvironment(Collider other, Vector3 collisionPos)
	{
		OnCollidedWithEnvironment(collisionPos);
	}

	public bool IsRequiredToCompleteLevel { get { return false; } }

	public ProjectileType Type { get { return ProjectileType.Guard; } }

	public Action<Transform> OnSubProjectileSpawned { get { return delegate{}; } set {} }
	public Action<Vector3> OnCollidedWithEnvironment { get { return onCollidedWithEnvironment; } set { onCollidedWithEnvironment = value; } }
	public Action<Collider, Vector3> OnCollidedWithTarget { get { return onCollidedWithTarget; } set { onCollidedWithTarget = value; } }
	public Action OnDestroyRequested { get { return onDestroyRequested; } set { onDestroyRequested = value; } }

	public Transform Transform { get { return transform; } }
}
