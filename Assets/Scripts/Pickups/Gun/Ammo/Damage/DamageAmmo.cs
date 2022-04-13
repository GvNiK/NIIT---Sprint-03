using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAmmo : IGunAmmo
{
	public bool CanEquipAsMain(IEquippableMain currentMainEquipment)
	{
		return false;
	}

	public bool CanEquipAsSecondary(IEquippableMain currentMainEquipment, IEquippable currentSecondaryEquipment)
	{
		return (currentMainEquipment is Gun) && 
			(currentSecondaryEquipment is DamageAmmo) == false;
	}

	public void Equip(IEquippable currentMainEquipment, Transform equipmentHolder)
	{
		(currentMainEquipment as Gun).ChangeAmmo(this);
	}

	public void Destroy() { }

	public void Unequip(IEquippable currentMainEquipment)
	{
		(currentMainEquipment as Gun).ChangeAmmo(null);
	}

	public ItemType Type { get { return ItemType.DamageAmmo; } }

	public ProjectileType ProjectileType { get { return ProjectileType.Damage; } }
}
