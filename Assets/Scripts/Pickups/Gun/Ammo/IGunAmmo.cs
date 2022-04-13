using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGunAmmo : IEquippable 
{
	ProjectileType ProjectileType { get; }
}
