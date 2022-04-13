using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCollision
{
	private Transform transform;

	public PlayerCollision(Transform transform)
	{
		this.transform = transform;
	}

	public bool OwnsCollider(Collider collider)
	{
		return collider.attachedRigidbody != null &&
			collider.attachedRigidbody.transform == transform;
	}
}
