using System;
using UnityEngine;

public class CollisionCallbacks : MonoBehaviour
{
	public Action<Collider> OnTriggerEntered = delegate { };
	public Action<Collision> OnCollisionEntered = delegate { };
	public Action<Collider> OnTriggerStayed = delegate { };
	public Action<Collider> OnTriggerExited = delegate { };

	private void OnTriggerEnter(Collider other)
	{
		OnTriggerEntered(other);
	}

	private void OnCollisionEnter(Collision collision)
	{
		OnCollisionEntered(collision);
	}

	private void OnTriggerStay(Collider other)
	{
		OnTriggerStayed(other);
	}

	private void OnTriggerExit(Collider other)
	{
		OnTriggerExited(other);
	}
}
