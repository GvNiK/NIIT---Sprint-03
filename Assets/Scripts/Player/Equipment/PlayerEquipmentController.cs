using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerEquipmentController
{
	public Action<ItemType> OnItemConsumed = delegate { };
	public Action<ItemType> OnEquipmentUsed = delegate { };
	public Action<Vector3, ItemType> OnCollidedWithEnvironment = delegate { };
	public Action<Guard, Vector3, ItemType> OnCollidedWithGuard = delegate { };
	public Action<Transform, ProjectileType> OnProjectileSpawned = delegate { };
	public Action OnNewItemEqupped = delegate { };
	private IEquippableMain main;
	private IEquippable secondary; 
	private Transform equipmentHolder;
	private InventoryController inventoryController;
	private EquipmentFactory equipmentFactory;
	private Collider[] playerColliders;
	private PlayerObjectData playerObjectData;

	public PlayerEquipmentController(Transform playerTransform, InventoryController inventoryController, 
		PlayerObjectData playerObjectData, PlayerSettings playerSettings, 
		PlayerEvents playerEvents, GunTargetLocator gunTargetLocator,
		PlayerInputBroadcaster inputBroadcaster, ProjectilePool projectilePool)
	{
		this.inventoryController = inventoryController;
		this.playerObjectData = playerObjectData;
		equipmentHolder = playerTransform.Find("EquipmentHolderFPS");
		equipmentFactory = new EquipmentFactory(playerObjectData.Blaster.gameObject, playerObjectData.Sword.gameObject,
			playerSettings, playerObjectData, playerEvents, gunTargetLocator, inputBroadcaster,
			projectilePool);

		
		playerColliders = playerTransform.GetComponentsInChildren<Collider>();

		inventoryController.OnItemCountUpdated += (item, count) =>
		{
			bool hasSecondaryBeenFullyConsumed = secondary != null && item == secondary.Type && count == 0;

			if(hasSecondaryBeenFullyConsumed)
			{
				UnequipSecondary();
			}
		};
	}

	public void OnPickupCollected(Pickup pickup)
	{
		inventoryController.Add(pickup.type, pickup.quantity);

		if(pickup.isRequiredToFinishLevel)
		{
			inventoryController.SetAsUndroppable(pickup.type);
		}

		EquipIfEmptyHanded(pickup.type);
	}

	private void EquipIfEmptyHanded(ItemType type)
	{
		IEquippable equippablePickup = equipmentFactory.Create(type);
		bool isEquippable = equippablePickup != null;

		if(isEquippable == false)
		{
			return;
		}

		if((main == null && equippablePickup.CanEquipAsMain(main)) ||
			(secondary == null && equippablePickup.CanEquipAsSecondary(main, null)))
		{
			Equip(equippablePickup);
		}
		else
		{
			equippablePickup.Destroy();
		}
	}

	public void Equip(ItemType type)
	{
		IEquippable equippablePickup = equipmentFactory.Create(type);
		bool isEquippable = equippablePickup != null;

		if (isEquippable)
		{
			Equip(equippablePickup);
		}
	}

	private void Equip(IEquippable equippable)
	{
		if(equippable.CanEquipAsMain(main))
		{
			UnequipSecondary();

			UnequipMain();

			main = equippable as IEquippableMain;
			main.OnProjectileSpawned += (proj, type) =>
			{
				IgnoreCollisionsWith(proj);
				OnProjectileSpawned(proj, type);
			};
			main.OnItemConsumed += (item) =>
			{
				inventoryController.Remove(item);
				OnItemConsumed(item);
			};
			main.OnCollidedWithGuard += (guard, type, obj) =>
			{
				OnCollidedWithGuard(guard, obj, type);
			};
			main.OnCollidedWithEnvironment += (type, obj) => OnCollidedWithEnvironment(obj, type);
			main.OnUsed += () => OnEquipmentUsed(main.Type);

			inventoryController.EquipMain(equippable.Type);

			equippable.Equip(main, equipmentHolder);
			OnNewItemEqupped();
		}
		else if(equippable.CanEquipAsSecondary(main, secondary))
		{
			UnequipSecondary();

			secondary = equippable;

			inventoryController.EquipSecondary(equippable.Type);

			equippable.Equip(main, equipmentHolder);
			OnNewItemEqupped();
		}
		else
		{
			equippable.Destroy();
		}
	}

	public bool CanEquip(ItemType type)
	{
		IEquippable equippable = equipmentFactory.Create(type);
		bool isEquippable = equippable != null;

		if (isEquippable == false)
		{
			return false;
		}

		isEquippable = equippable.CanEquipAsMain(main) ||
			equippable.CanEquipAsSecondary(main, secondary);

		equippable.Destroy();

		return isEquippable;
	}

	private void UnequipMain()
	{
		if(main != null)
		{
			inventoryController.UnequipMain();
			main.Unequip(main);
			main.Destroy();
			main = null;
		}
	}

	private void UnequipSecondary()
	{
		if(secondary != null)
		{
			inventoryController.UnequipSecondary();
			secondary.Unequip(main);
			secondary.Destroy();
			secondary = null;
		}
	}

	private void IgnoreCollisionsWith(Transform other)
	{
		Collider[] otherColliders = other.GetComponentsInChildren<Collider>();

		foreach(Collider playerCollider in playerColliders)
		{
			foreach(Collider otherCollider in otherColliders)
			{
				Physics.IgnoreCollision(playerCollider, otherCollider);
			}
		}
	}

	public void StartUse()
	{
		if (main != null && main.CanUse())
		{
			main.StartUse();
		}
	}

	public void EndUse()
	{
		if (main != null)
		{
			main.EndUse();
		}
	}

	public void Update()
	{
		if(main != null)
		{
			main.Update();
		}
	}

	public IEquippableMain Main {  get { return main; } }

	public IEquippable Secondary { get { return secondary; } }
}
