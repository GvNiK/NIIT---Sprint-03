using UnityEngine;
using System;

public class ProjectileBehaviour
{
	private const float LIFESPAN = 3f;

	public Action<Collider, Vector3> OnCollisionWithTarget = delegate { };
	public Action<Collider, Vector3> OnCollisionWithEnvironment = delegate { };
	public Action<Collider, Vector3> OnCollision = delegate { };
	public Action OnDestroyRequested = delegate { };

	private Transform transform;
	private Rigidbody body;
	private float fireTime;
	private CollisionCallbacks collisionCallbacks;
	private string targetString;

	public ProjectileBehaviour(Transform transform, string targetString)
	{
		this.transform = transform;
		this.targetString = targetString;
		body = transform.GetComponent<Rigidbody>();
		collisionCallbacks = transform.GetComponentInChildren<CollisionCallbacks>();
	}

	public void Fire(Vector3 position, Vector3 forward, float force)
	{
		transform.position = position;
		transform.forward = forward;

		body.AddForce(transform.forward * force, ForceMode.Impulse);

		collisionCallbacks.OnTriggerEntered += OnCollided;

		fireTime = Time.time;
	}

	public void Update()
	{
		float lifeTime = Time.time - fireTime;

		if (lifeTime >= LIFESPAN)
		{
			StartDestroy();
		}
	}

	private void OnCollided(Collider collider)
	{
		Vector3 closestPoint = collider.ClosestPointOnBounds(transform.position);
		OnCollision(collider, closestPoint);
		if (collider.transform.tag.Equals(targetString))
		{
			OnCollisionWithTarget(collider, closestPoint);
		}
		else
		{
			OnCollisionWithEnvironment(collider, closestPoint);
		}
		StartDestroy();
	}

	private void StartDestroy()
	{
		collisionCallbacks.OnTriggerEntered -= OnCollided;
		OnDestroyRequested();
	}
}
