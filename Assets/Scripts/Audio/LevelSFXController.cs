using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelSFXController
{
	private AudioManager audioManager;

	public LevelSFXController(PlayerController player, PlayerInteractionController playerInteraction,
		AudioManager audioManager, PickupEvents pickupEvents,
		PlayerEquipmentController playerEquipment, GuardEvents guardEvents)
	{
		this.audioManager = audioManager;
		GameObject playerObj = player.Objects.PlayerAnimator.gameObject;

		playerEquipment.OnItemConsumed += (type) =>
		{
			switch (type)
			{
				case ItemType.DamageAmmo:
					audioManager.SFX.Play("SHOOT_DAMAGE", player.Objects.Blaster.gameObject);
					break;
				case ItemType.ExplosiveAmmo:
					audioManager.SFX.Play("SHOOT_EXPLOSIVE", player.Objects.Blaster.gameObject);
					break;
			}
		};

		playerEquipment.OnProjectileSpawned += (proj, type) =>
		{
			switch (type)
			{
				case ProjectileType.SubExplosion:
					audioManager.SFX.Play("PROJECTILE_EXPLOSION", proj.transform.position);
					break;
			}
		};

		player.OnEquipmentCollidedWithEnvironment += (collisionPos, type) =>
		{
			switch (type)
			{
				case ItemType.DamageAmmo:
					audioManager.SFX.Play("PROJECTILE_DAMAGE_ENV", collisionPos);
					break;
			}
		};

		player.OnEquipmentCollidedWithGuard += (guard, collisionPos, type) =>
		{
			switch (type)
			{
				case ItemType.DamageAmmo:
				case ItemType.ExplosiveAmmo:
					audioManager.SFX.Play("PROJECTILE_ENEMY", collisionPos);
					break;
				case ItemType.Melee:
					audioManager.SFX.Play("MELEE_ENEMY", collisionPos);
					break;
			}
		};

		player.OnEquipmentUsed += (type) =>
		{
			switch (type)
			{
				case ItemType.Melee:
					audioManager.SFX.Play("MELEE", player.Objects.Sword.gameObject);
					break;
			}
		};

		player.OnPlayerDamageTaken += (damage) =>
		{
			audioManager.SFX.Play("PLAYER_DAMAGE", playerObj);
		};

		player.OnDeathSequenceStarted += () =>
		{
			audioManager.SFX.Play("PLAYER_KILLED", playerObj);
		};

		playerInteraction.OnInteractionStarted += (interaction) =>
		{
			TimelineSignalListener signalListener = interaction.GetComponentInChildren<TimelineSignalListener>();
			signalListener.OnMessage += (message) =>
			{
				switch(message)
				{
					case "DoorOpen":
						audioManager.SFX.Play("DOOR_OPEN", interaction.gameObject);
						break;
				}
			};
		};

		pickupEvents.OnPickupCollected += (pickup) =>
		{
			switch (pickup.type)
			{
				case ItemType.Gun:
					audioManager.SFX.Play("PICKUP_GUN", pickup.transform.position);
					break;
				case ItemType.DamageAmmo:
				case ItemType.ExplosiveAmmo:
					audioManager.SFX.Play("PICKUP_AMMO", pickup.transform.position);
					break;
				case ItemType.Supplies:
					audioManager.SFX.Play("PICKUP_SUPPLIES", pickup.transform.position);
					break;
				case ItemType.Melee:
					audioManager.SFX.Play("PICKUP_MELEE", pickup.transform.position);
					break;
			}
		};

		pickupEvents.OnRemainingPickupCountUpdated += (pickupCount) =>
		{
			if(pickupCount == 0)
			{
				audioManager.SFX.Play("EXIT_UNLOCKED", playerObj);
			}
		};

		
		AnimationListener playerAnimationListener = playerObj.GetComponentInChildren<AnimationListener>();
		playerAnimationListener.OnWeightedAnimationEvent += (argument, weight) =>
		{
			switch (argument)
			{
				case "Footstep":
					if (weight > 0.5f)
					{
						audioManager.SFX.Play("FOOTSTEP", playerObj);
					}
					break;
				case "PushButton":
					audioManager.SFX.Play("PANEL_BUTTON_PRESS", playerObj);
					break;
			}
		};

		guardEvents.AddSpawnedListener((guard, controller) =>
		{
			AnimationListener animListener = guard.GetComponentInChildren<AnimationListener>();
			animListener.OnWeightedAnimationEvent += (argument, weight) =>
			{
				switch (argument)
				{
					case "Footstep":
						if (weight > 0.5f)
						{
							audioManager.SFX.Play("FOOTSTEP", guard.gameObject);
						}
						break;
				}
			};

			controller.OnSuspicionUpgraded += () =>
			{
				audioManager.SFX.Play("GUARD_MORE_ALERT", guard.gameObject);
			};
			controller.OnKilled += () =>
			{
				audioManager.SFX.Play("GUARD_KILLED", guard.gameObject);
			};
			controller.OnProjectileSpawned += (proj) =>
			{
				audioManager.SFX.Play("GUARD_SHOOT", guard.gameObject);
			};

			if (guard is GuardFlying guardFlying)
			{
				audioManager.SFX.PlayLooped("GUARD_JET_BOOST_LOOP", guard.gameObject);
			}
		});

		guardEvents.AddKilledListener((guard) =>
		{
			if (guard is GuardFlying guardFlying)
			{
				audioManager.SFX.StopLooped(guard.gameObject);
			}
		});

		guardEvents.AddSpawnerHasRespawnedListener((guard) =>
		{
			audioManager.SFX.Play("GUARD_CREATED_BY_SPAWNER", guard.gameObject);
		});
	}

	public void OnLevelComplete()
	{
		audioManager.SFX.Play("LEVEL_COMPLETE");
	}

	public void OnLevelFail()
	{
		audioManager.SFX.Play("LEVEL_FAIL");
	}
}
