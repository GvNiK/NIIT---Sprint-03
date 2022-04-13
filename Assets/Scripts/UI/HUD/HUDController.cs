using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
	public Action OnInventoryRequested = delegate { };
	public Action OnPauseMenuRequested = delegate { };

	public CanvasGroup gameplayHUDCanvasGroup;
    public JoystickController joystickController;
    
    public Image playerHealthBar;

	public CanvasGroup equipmentGroup;
	public Image equipmentIcon;
	public Image equipmentBorder;
	public CanvasGroup secondaryGroup;
	public Image secondaryIcon;
	public TextMeshProUGUI secondaryText;

	public ItemIcons icons;
	public Sprite interactIcon;

	public PickupHUDAnimation pickupAnimation;

	private Player player;
	private InventoryController inventory;

	public void SetupHUDController(Player player, InventoryController inventory, 
		PickupEvents pickupEvents)
    {
		this.player = player;
		this.inventory = inventory;
        joystickController.Setup(player.Broadcaster.Callbacks);

		player.Equipment.OnNewItemEqupped += () => UpdateEquipment();
		player.Equipment.OnItemConsumed += (itemType) => UpdateEquipment();
		pickupEvents.OnPickupDropped += () => UpdateEquipment();
		pickupEvents.OnPickupCollected += (pickup) =>
		{
			pickupAnimation.OnItemPickedUp(pickup.type);
			UpdateEquipment();
		};

		UpdateEquipment();
    }

	public void Update()
	{
		UpdateEquipmentEnabledState();
	}

	public void ShowHUD()
    {
        gameplayHUDCanvasGroup.alpha = 1f;
    }

    public void HideHUD()
    {
        gameplayHUDCanvasGroup.alpha = 0f;
    }

    public void UpdatePlayerHealth(float newValue)
    {
        playerHealthBar.fillAmount = newValue / player.Settings.PlayerMaxHP;
    }

    public void RequestPause()
    {
		OnPauseMenuRequested();
    }

	public void RequestInventory()
	{
		OnInventoryRequested();
	}

	private void UpdateEquipment()
	{
		if (player.Equipment.Secondary != null)
		{
			secondaryIcon.sprite = icons.Get(player.Equipment.Secondary.Type);
			secondaryText.text = inventory.GetCount(player.Equipment.Secondary.Type).ToString();
		}
	}

	private void UpdateEquipmentEnabledState()
	{
		if(player.Interaction.CanInteract())
		{
			equipmentIcon.sprite = interactIcon;
			equipmentBorder.enabled = true;
			equipmentGroup.alpha = 1f;
			secondaryGroup.alpha = 0f;
		}
		else
		{
			if (player.Equipment.Main == null)
			{
				equipmentGroup.alpha = 0f;
			}
			else
			{
				if (player.Equipment.Main.CanUse())
				{
					equipmentIcon.sprite = icons.Get(player.Equipment.Main.Type);
					equipmentBorder.enabled = true;
				}
				else
				{
					equipmentIcon.sprite = icons.GetInactive(player.Equipment.Main.Type);
					equipmentBorder.enabled = false;
				}
				equipmentGroup.alpha = 1f;
			}

			if (player.Equipment.Secondary == null)
			{
				secondaryGroup.alpha = 0f;
			}
			else
			{
				secondaryGroup.alpha = 1f;
			}
		}
		
	}
}
