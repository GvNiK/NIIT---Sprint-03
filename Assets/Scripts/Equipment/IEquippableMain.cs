using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IEquippableMain : IEquippable
{
	bool CanUse();
	void StartUse();
	void EndUse();
	void Update();
	Action<ItemType, Vector3> OnCollidedWithEnvironment { get; set; }
	Action<Guard, ItemType, Vector3> OnCollidedWithGuard { get; set; }
	Action<Transform, ProjectileType> OnProjectileSpawned { get; set; }
	Action<ItemType> OnItemConsumed { get; set; }
	Action OnUsed { get; set; }
}
