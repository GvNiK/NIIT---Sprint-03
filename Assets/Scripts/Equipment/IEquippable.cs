using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquippable
{
	bool CanEquipAsMain(IEquippableMain currentMainEquipment);
	bool CanEquipAsSecondary(IEquippableMain currentMainEquipment, IEquippable currentSecondaryEquipment);
	void Equip(IEquippable currentMainEquipment, Transform equipmentHolder);
	void Unequip(IEquippable currentMainEquipment);
	void Destroy();
	ItemType Type { get; }
}
