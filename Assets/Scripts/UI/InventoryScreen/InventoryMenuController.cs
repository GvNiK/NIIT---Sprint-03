using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InventoryMenuController
{
	public Action OnClosed = delegate { };
	private InventoryController inventory;
	private CanvasGroup canvasGroup;
	private InventoryItem[] inventoryItems;
	private List<InventoryItem> inUse;
	private InventoryMenu menu;
	private PlayerEquipmentController equipmentController;
	private InventorySubMenuController subMenu;
	private PickupController pickupController;
	private ButtonAudio buttonAudio;

	public InventoryMenuController(GameObject prefab,
		Transform hudTransform, InventoryController inventory,
		PlayerEquipmentController equipmentController, PickupController pickupController)
	{
		this.inventory = inventory;
		this.equipmentController = equipmentController;
		this.pickupController = pickupController;

		GameObject gameObject = GameObject.Instantiate(prefab, hudTransform);
		menu = gameObject.GetComponent<InventoryMenu>();

		canvasGroup = menu.GetComponent<CanvasGroup>();
		inventoryItems = menu.itemGrid.GetComponentsInChildren<InventoryItem>();
		inUse = new List<InventoryItem>();

		buttonAudio = menu.itemGrid.GetComponent<ButtonAudio>(); 

		subMenu = new InventorySubMenuController(menu.subMenuPrefab, menu.subMenuItemPrefab);

		menu.close.onClick.AddListener(() => Hide());
	}

	public void Show()
	{
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
		Refresh();
	}

	private void Refresh()
	{
		Clear();

		UpdateItems();
		UpdateEquipment();
	}

	private void UpdateItems()
	{
		Dictionary<ItemType, InventoryItem> filledSlots = new Dictionary<ItemType, InventoryItem>();

		foreach (KeyValuePair<ItemType, InventoryController.Item> item in inventory.Contents)
		{
			InventoryItem slot = StartUsingNextSlot();
			filledSlots[item.Key] = slot;

			slot.icon.sprite = menu.icons.Get(item.Key);
			slot.icon.gameObject.SetActive(true);
			slot.count.text = item.Value.count.ToString();
			slot.count.gameObject.SetActive(true);
			slot.button.onClick.AddListener(() =>
			{
				UpdateSubMenuIcons(filledSlots);
				slot.subMenuIcon.SetActive(false);

				List<InventorySubMenuController.Entry> subMenuEntries = new List<InventorySubMenuController.Entry>();
				
				if(equipmentController.CanEquip(item.Key))
				{
					subMenuEntries.Add(new InventorySubMenuController.Entry(() =>
					{
						equipmentController.Equip(item.Key);
						Refresh();
					}, "Equip"));
				}
				if(item.Value.canDrop)
				{
					subMenuEntries.Add(new InventorySubMenuController.Entry(() =>
					{
						inventory.RemoveAll(item.Key);
						pickupController.Drop(item.Key, item.Value.count);
						Refresh();
					}, "Drop"));
				}

				subMenu.Show(slot.subMenuContainer, subMenuEntries);

				buttonAudio.OnButtonsChanged();
			});
		}

		UpdateSubMenuIcons(filledSlots);

		buttonAudio.OnButtonsChanged();
	}

	private void UpdateSubMenuIcons(Dictionary<ItemType, InventoryItem> filledSlots)
	{
		foreach (KeyValuePair<ItemType, InventoryItem> slot in filledSlots)
		{
			InventoryController.Item item = inventory.Contents.First(x => x.Key == slot.Key).Value;
			slot.Value.subMenuIcon.SetActive(CanClick(slot.Key, item));
		}
	}

	private bool CanClick(ItemType itemType, InventoryController.Item item)
	{
		return equipmentController.CanEquip(itemType) || item.canDrop;
	}

	private void UpdateEquipment()
	{
		if(equipmentController.Main == null)
		{
			menu.mainEquipmentIcon.gameObject.SetActive(false);
		}
		else
		{
			menu.mainEquipmentIcon.sprite = menu.icons.Get(equipmentController.Main.Type);
			menu.mainEquipmentIcon.gameObject.SetActive(true);
		}

		if(equipmentController.Secondary == null)
		{
			menu.secondaryEquipmentIcon.gameObject.SetActive(false);
			menu.secondaryEquipmentCount.gameObject.SetActive(false);
		}
		else
		{
			menu.secondaryEquipmentIcon.sprite = menu.icons.Get(equipmentController.Secondary.Type);
			menu.secondaryEquipmentIcon.gameObject.SetActive(true);
			menu.secondaryEquipmentCount.text = inventory.GetCount(equipmentController.Secondary.Type).ToString();
			menu.secondaryEquipmentCount.gameObject.SetActive(true);
		}
	}

	private InventoryItem StartUsingNextSlot()
	{
		foreach (InventoryItem inventoryItem in inventoryItems)
		{
			if(inUse.Contains(inventoryItem) == false)
			{
				inUse.Add(inventoryItem);
				return inventoryItem;
			}
		}

		return null;
	}

	private void Clear()
	{
		foreach (InventoryItem inventoryItem in inventoryItems)
		{
			inventoryItem.button.onClick.RemoveAllListeners();
			inventoryItem.icon.gameObject.SetActive(false);
			inventoryItem.count.gameObject.SetActive(false);
			inventoryItem.subMenuIcon.SetActive(false);
		}

		inUse.Clear();
	}

	private void Hide()
	{
		canvasGroup.alpha = 0f;
		canvasGroup.blocksRaycasts = false;
		pickupController.PerformDrop();
		OnClosed();
	}
}
