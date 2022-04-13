using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTests 
{
	private InventoryController inventory;

	[SetUp]
	public void Setup()
	{
		inventory = new InventoryController();
	}

	[Test]
	public void AddingUpdatesCount()
	{
		bool updated = false;
		inventory.OnItemCountUpdated += (pickupType, count) =>
		{
			if(pickupType == ItemType.Gun)
			{
				Assert.AreEqual(1, count);
				updated = true;
			}
		};

		inventory.Add(ItemType.Gun);

		Assert.IsTrue(updated);
    }

	[Test]
	public void CanAddReturnsFalseAtMax()
	{
		for(int i = 0; i< InventoryController.MAX_NUMBER_PER_CATEGORY; i++)
		{
			if(inventory.CanAdd(ItemType.Gun) == false)
			{
				Assert.Fail();
			}

			inventory.Add(ItemType.Gun);
		}

		Assert.IsFalse(inventory.CanAdd(ItemType.Gun));
	}

	[Test]
	public void RemovingUpdatesCount()
	{
		inventory.Add(ItemType.Gun);

		bool updated = false;
		inventory.OnItemCountUpdated += (pickupType, count) =>
		{
			if (pickupType == ItemType.Gun)
			{
				Assert.AreEqual(0, count);
				updated = true;
			}
		};

		inventory.Remove(ItemType.Gun);

		Assert.IsTrue(updated);
	}

	[Test]
	public void CannotRemoveWithNoContents()
	{
		Assert.IsFalse(inventory.CanRemove(ItemType.Gun));
	}

	[Test]
	public void RemoveAllReturnsCorrectCount()
	{
		inventory.Add(ItemType.DamageAmmo);
		inventory.Add(ItemType.DamageAmmo);
		inventory.Add(ItemType.DamageAmmo);

		Assert.AreEqual(3, inventory.RemoveAll(ItemType.DamageAmmo));
	}

	[Test]
	public void NoContentsInitially()
	{
		Assert.AreEqual(0, inventory.Contents.Count);
	}

	[Test]
	public void EquippedItemsNotIncludedInContents()
	{
		inventory.Add(ItemType.DamageAmmo);
		inventory.Add(ItemType.ExplosiveAmmo);

		Dictionary<ItemType, InventoryController.Item> contents = inventory.Contents;

		Assert.AreEqual(1, contents[ItemType.DamageAmmo].count);
		Assert.AreEqual(1, contents[ItemType.ExplosiveAmmo].count);

		inventory.EquipMain(ItemType.DamageAmmo);
		inventory.EquipSecondary(ItemType.ExplosiveAmmo);

		contents = inventory.Contents;

		Assert.IsFalse(contents.ContainsKey(ItemType.DamageAmmo));
		Assert.IsFalse(contents.ContainsKey(ItemType.ExplosiveAmmo));
	}

	[Test]
	public void EquippedItemIsUnequippedWhenRemoved()
	{
		inventory.Add(ItemType.DamageAmmo);
		inventory.Add(ItemType.ExplosiveAmmo);

		Dictionary<ItemType, InventoryController.Item> contents = inventory.Contents;

		Assert.AreEqual(1, contents[ItemType.DamageAmmo].count);
		Assert.AreEqual(1, contents[ItemType.ExplosiveAmmo].count);

		inventory.EquipMain(ItemType.DamageAmmo);
		inventory.EquipSecondary(ItemType.ExplosiveAmmo);

		inventory.Remove(ItemType.DamageAmmo);
		inventory.Remove(ItemType.ExplosiveAmmo);

		inventory.Add(ItemType.DamageAmmo);
		inventory.Add(ItemType.ExplosiveAmmo);

		Assert.AreEqual(1, contents[ItemType.DamageAmmo].count);
		Assert.AreEqual(1, contents[ItemType.ExplosiveAmmo].count);
	}

	[Test]
	public void ItemOnlyUnequippedWithEmptyContents()
	{
		inventory.OnSecondaryUnequipped += () => Assert.Fail();

		inventory.Add(ItemType.DamageAmmo, 2);

		inventory.EquipSecondary(ItemType.DamageAmmo);

		inventory.Remove(ItemType.DamageAmmo);
	}


	[Test]
	public void OnlyOneCallbackIsRecievedWhenBulkAdding()
	{
		int callbackCount = 0;
		inventory.OnItemCountUpdated += (item, count) => callbackCount++;

		inventory.Add(ItemType.DamageAmmo, 2);

		Assert.AreEqual(1, callbackCount);
	}

	[Test]
	public void OnlyOneCallbackIsRecievedWhenBulkRemoving()
	{
		inventory.Add(ItemType.DamageAmmo, 2);

		int callbackCount = 0;
		inventory.OnItemCountUpdated += (item, count) => callbackCount++;

		inventory.Remove(ItemType.DamageAmmo, 2);

		Assert.AreEqual(1, callbackCount);
	}

	[Test]
	public void UndroppableItemsCantBeRemoved()
	{
		inventory.Add(ItemType.DamageAmmo);
		inventory.SetAsUndroppable(ItemType.DamageAmmo);

		Assert.IsFalse(inventory.CanRemove(ItemType.DamageAmmo));
	}

	[Test]
	public void UndroppableItemsAreListedCorrectlyInContents()
	{
		inventory.Add(ItemType.DamageAmmo);
		inventory.Add(ItemType.ExplosiveAmmo);

		inventory.SetAsUndroppable(ItemType.DamageAmmo);

		Dictionary<ItemType, InventoryController.Item> contents = inventory.Contents;

		Assert.IsFalse(contents[ItemType.DamageAmmo].canDrop);
		Assert.IsTrue(contents[ItemType.ExplosiveAmmo].canDrop);
	}
}