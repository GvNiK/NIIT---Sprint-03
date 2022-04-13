using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAmmo : IGunAmmo
{
	public bool CanEquipAsMain(IEquippableMain currentMainEquipment)
	{
		return false;
	}

	public bool CanEquipAsSecondary(IEquippableMain currentMainEquipment, IEquippable currentSecondaryEquipment)
	{
		return false;
	}

	public void Equip(IEquippable currentMainEquipment, Transform equipmentHolder){ }

	public void Destroy() { }

	public void Unequip(IEquippable currentMainEquipment){ }

	public ItemType Type { get { return ItemType.GuardAmmo; } }
	public ProjectileType ProjectileType { get { return ProjectileType.Guard; } }
}
