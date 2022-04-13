using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IProjectile
{
	void Fire(Transform owner, Vector3 position, Vector3 forward);
	void Update();
	Action<Transform> OnSubProjectileSpawned { get; set; }
	Action<Vector3> OnCollidedWithEnvironment { get; set; }
	Action<Collider, Vector3> OnCollidedWithTarget { get; set; }
	Action OnDestroyRequested { get; set; }
	Transform Transform { get; }
	ProjectileType Type { get; }
}
