using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController 
{
	public Action<Levels.Data> OnLevelLoadRequest = delegate { };
	public Action OnExitRequested = delegate { };
	private HUDController hudController;
    private LevelEndMenuController levelEndMenu;
	private InventoryMenuController inventoryMenu;
	private TimeController timeController;
	private Player player;
	private PauseMenuController pauseMenuController;
	private LevelIntroUIController levelIntroController;
	private DoorUnlockedUIController doorUnlockedController;
	private int currentLevelID;

	public UIController(Player player,
		Transform cameraTransform, TimeController timeController, 
		int currentLevelID, InventoryController inventoryController,
		PickupController pickupController, PickupEvents pickupEvents,
		GuardEvents guardEvents, LevelStatsController levelStatsController,
		GameObject inventoryMenuPrefab, GameObject guardHealthPrefab)
    {
		this.player = player;
		this.timeController = timeController;
		this.currentLevelID = currentLevelID;

        hudController = cameraTransform.Find("HUDCanvas/HUD").GetComponent<HUDController>();
        hudController.SetupHUDController(player, 
			inventoryController, pickupEvents);

		pauseMenuController = hudController.GetComponentInChildren<PauseMenuController>(true);
		pauseMenuController.OnExitequested += () =>
		{
			OnExitRequested();
			OnScreenClosed();
		};
		pauseMenuController.OnRestartRequested += () =>
		{
			OnLevelLoadRequest(GetCurrentLevel());
			OnScreenClosed();
		};

		levelEndMenu = new LevelEndMenuController(hudController.GetComponentInChildren<LevelEndMenu>(true), currentLevelID, levelStatsController);
		levelEndMenu.OnLevelLoadRequested += (level) =>
		{
			OnLevelLoadRequest(level);
			OnScreenClosed();
		};
		levelEndMenu.OnRetryLevelRequested += () =>
		{
			OnLevelLoadRequest(GetCurrentLevel());
			OnScreenClosed();
		};
		levelEndMenu.OnExitRequested += () =>
		{
			OnExitRequested();
			OnScreenClosed();
		};

		inventoryMenu = new InventoryMenuController(inventoryMenuPrefab, hudController.transform, 
			inventoryController, player.Equipment, pickupController);

		_ = new GuardUIController(guardHealthPrefab, guardEvents);

		levelIntroController = new LevelIntroUIController(hudController.transform, player.Broadcaster, currentLevelID);

		doorUnlockedController = new DoorUnlockedUIController(hudController.transform);
		pickupEvents.OnRemainingPickupCountUpdated += (count) =>
		{
			if(count == 0)
			{
				doorUnlockedController.OnDoorUnlocked();
			}
		};

		player.Broadcaster.Callbacks.OnPlayerPauseRequested += () => ShowPause();
		hudController.OnPauseMenuRequested += () => ShowPause();
		pauseMenuController.OnClosed += () =>
		{
			player.Broadcaster.EnableActions(ControlType.Gameplay);
			hudController.ShowHUD();
			OnScreenClosed();
		};

		player.Broadcaster.Callbacks.OnPlayerInventoryOpenRequested += () => ShowInventory();
		hudController.OnInventoryRequested += () => ShowInventory();
		inventoryMenu.OnClosed += () =>
		{
			player.Broadcaster.EnableActions(ControlType.Gameplay);
			hudController.ShowHUD();
			OnScreenClosed();
		};
	}

	private Levels.Data GetCurrentLevel()
	{
		foreach (Levels.Data level in Levels.ALL)
		{
			if (level.ID == currentLevelID)
			{
				return level;
			}
		}

		return null;
	}

	public void ShowPause()
	{
		player.Broadcaster.EnableActions(ControlType.None);
		hudController.HideHUD();
		pauseMenuController.Show();
		OnScreenOpened();
	}

	public void ShowInventory()
	{
		player.Broadcaster.EnableActions(ControlType.None);
		hudController.HideHUD();
		inventoryMenu.Show();
		OnScreenOpened();
	}

	public void OnLevelFailed(string message)
	{
		levelEndMenu.Show(message, false);
		OnScreenOpened();
	}

	public void OnLevelComplete(string message)
	{
		levelEndMenu.Show(message, true);
		OnScreenOpened();
	}

	private void OnScreenOpened()
	{
		timeController.Pause();
	}

	private void OnScreenClosed()
	{
		timeController.Resume();
	}

	public void Update()
	{
		levelIntroController.Update();
		doorUnlockedController.Update();
	}
}
