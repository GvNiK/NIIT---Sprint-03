using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InventoryController
{
	public const int MAX_NUMBER_PER_CATEGORY = 99;

	public Action<ItemType, int> OnItemCountUpdated = delegate { };
	public Action OnMainUnequipped = delegate { };
	public Action OnSecondaryUnequipped = delegate { };

	private Dictionary<ItemType, Item> heldItems;
	private Item main;
	private Item secondary;

	public InventoryController()
	{
		heldItems = new Dictionary<ItemType, Item>();

		foreach(ItemType pickupType in Enum.GetValues(typeof(ItemType)))
		{
			heldItems[pickupType] = new Item();
		}
	}

	public bool CanAdd(ItemType type)
	{
		return heldItems[type].count < MAX_NUMBER_PER_CATEGORY;
	}

	public void Add(ItemType type)
	{
		if (CanAdd(type))
		{
			heldItems[type].count++;

			OnItemCountUpdated(type, heldItems[type].count);
		}
	}

	public void Add(ItemType type, int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (CanAdd(type))
			{
				heldItems[type].count++;
			}
		}

		OnItemCountUpdated(type, heldItems[type].count);
	}

	public void SetAsUndroppable(ItemType type)
	{
		heldItems[type].canDrop = false;
	}

	public bool CanRemove(ItemType type)
	{
		return heldItems[type].count > 0 &&
			heldItems[type].canDrop;
	}

	public void Remove(ItemType type)
	{
		if(CanRemove(type))
		{
			heldItems[type].count--;

			CheckForUnequip(type);

			OnItemCountUpdated(type, heldItems[type].count);
		}
	}

	public void Remove(ItemType type, int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (CanRemove(type))
			{
				heldItems[type].count--;
			}

			CheckForUnequip(type);
		}

		OnItemCountUpdated(type, heldItems[type].count);
	}

	private void CheckForUnequip(ItemType type)
	{
		if(heldItems[type].count > 0)
		{
			return;
		}

		if (heldItems[type] == main)
		{
			UnequipMain();
		}
		else if (heldItems[type] == secondary)
		{
			UnequipSecondary();
		}
	}

	public int RemoveAll(ItemType type)
	{
		int itemCount = heldItems[type].count;
		heldItems[type].count = 0;
		CheckForUnequip(type);

		return itemCount;
	}

	public void EquipMain(ItemType type)
	{
		main = heldItems[type];
	}

	public void EquipSecondary(ItemType type)
	{
		secondary = heldItems[type];
	}

	public void UnequipMain()
	{
		main = null;
		OnMainUnequipped();
	}

	public void UnequipSecondary()
	{
		secondary = null;
		OnSecondaryUnequipped();
	}

	public int GetCount(ItemType type)
	{
		return heldItems[type].count;
	}

	public Dictionary<ItemType, Item> Contents
	{
		get
		{
			Dictionary<ItemType, Item> toReturn = new Dictionary<ItemType, Item>();

			foreach(KeyValuePair<ItemType, Item> heldItem in heldItems)
			{
				if(heldItem.Value != main && 
					heldItem.Value != secondary &&
					heldItem.Value.count > 0)
				{
					toReturn[heldItem.Key] = heldItem.Value.Copy();
				}
			}

			return toReturn;
		}
	}

	public class Item
	{
		public int count;
		public bool canDrop;

		public Item()
		{
			canDrop = true;
		}

		public Item Copy()
		{
			return new Item()
			{
				count = count,
				canDrop = canDrop
			};
		}
	}
}
