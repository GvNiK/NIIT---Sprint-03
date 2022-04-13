using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Explosion : IProjectile
{
	private const float MAX_SCALE = 10f;
	private const float ANIM_TIME = 0.3f;

	public Action<Collider, Vector3> onCollidedWithTarget = delegate { };
	public Action<Vector3> onCollidedWithEnvironment = delegate { };
	public Action onDestroyRequested = delegate { };

	private Transform owner;
	private float stateTime;
	private Transform transform;
	private CollisionCallbacks collisionCallbacks;
	private PlayerSettings playerSettings;

	public Explosion(Transform transform, PlayerSettings playerSettings)
	{
		this.transform = transform;
		this.playerSettings = playerSettings;
		collisionCallbacks = transform.GetComponentInChildren<CollisionCallbacks>();
		collisionCallbacks.OnTriggerEntered += OnCollision;
	}

	public void Fire(Transform owner, Vector3 position, Vector3 forward)
	{
		this.owner = owner;
		transform.position = position;
	}

	public void Update()
	{
		stateTime += Time.deltaTime;

		transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * MAX_SCALE, stateTime / ANIM_TIME);
		transform.Rotate(Vector3.up, 10);

		if(stateTime >= ANIM_TIME)
		{
			collisionCallbacks.OnTriggerEntered -= OnCollision;
			OnDestroyRequested();
		}
	}

	private void OnCollision(Collider other)
	{
		if (other.transform.tag.Equals("Enemy"))
		{
			Guard guard = other.transform.GetComponent<Guard>();
			guard.TakeDamage(playerSettings.ExplosiveAmmoDamage, owner);
		}
	}

	public Action<Transform> OnSubProjectileSpawned { get { return delegate { }; } set { } }
	public Action<Vector3> OnCollidedWithEnvironment { get { return onCollidedWithEnvironment; } set { onCollidedWithEnvironment = value; } }
	public Action<Collider, Vector3> OnCollidedWithTarget { get { return onCollidedWithTarget; } set { onCollidedWithTarget = value; } }
	public Action OnDestroyRequested { get { return onDestroyRequested; } set { onDestroyRequested = value; } }

	public Transform Transform { get { return transform; } }

	public ProjectileType Type { get { return ProjectileType.SubExplosion; } }
}
