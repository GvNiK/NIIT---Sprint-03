using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAmmo : IGunAmmo
{
	public bool CanEquipAsMain(IEquippableMain currentMainEquipment)
	{
		return false;
	}

	public bool CanEquipAsSecondary(IEquippableMain currentMainEquipment, IEquippable currentSecondaryEquipment)
	{
		return (currentMainEquipment is Gun) &&
			(currentSecondaryEquipment is ExplosionAmmo) == false;
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

	public ItemType Type { get { return ItemType.ExplosiveAmmo; } }
	public ProjectileType ProjectileType { get { return ProjectileType.Explosive; } }
}
