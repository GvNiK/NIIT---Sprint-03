using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EquipmentFactory
{
	private GameObject gun;
	private GameObject sword;
	private ControlType controlType;
	private PlayerObjectData playerObjectData;
	private PlayerSettings playerSettings;
	private PlayerEvents playerEvents;
	private GunTargetLocator gunTargetLocator;
	private PlayerInputBroadcaster inputBroadcaster;
	private ProjectilePool projectilePool;

	public EquipmentFactory(GameObject gun, GameObject sword, 
		PlayerSettings playerSettings, PlayerObjectData playerObjectData,
		PlayerEvents playerEvents, GunTargetLocator gunTargetLocator,
		PlayerInputBroadcaster inputBroadcaster, ProjectilePool projectilePool)
	{
		this.gun = gun;
		this.sword = sword;
		this.playerObjectData = playerObjectData;
		this.playerSettings = playerSettings;
		this.playerEvents = playerEvents;
		this.gunTargetLocator = gunTargetLocator;
		this.inputBroadcaster = inputBroadcaster;
		this.projectilePool = projectilePool;
	}

	public IEquippable Create(ItemType type)
	{
		switch(type)
		{
			case ItemType.Melee:
				return new Sword(sword, playerSettings, playerObjectData);
			case ItemType.Gun:
				return new Gun(gun, playerObjectData, gunTargetLocator, 
					playerEvents, inputBroadcaster, projectilePool);
			case ItemType.DamageAmmo:
				return new DamageAmmo();
			case ItemType.ExplosiveAmmo:
				return new ExplosionAmmo();
		}

		return null;
	}
}
