using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelVFXController
{
	private VFXPool pool;
	private List<ActiveVFX> activeVFX;
	private List<ActiveVFX> clearedCache;

	public LevelVFXController(VFXLibrary library, PlayerController player,
		GuardEvents guardEvents, GuardManager guardManager)
	{
		activeVFX = new List<ActiveVFX>();
		pool = new VFXPool(library);

		clearedCache = new List<ActiveVFX>();

		player.OnEquipmentCollidedWithEnvironment += (collisionPos, type) =>
		{
			switch (type)
			{
				case ItemType.DamageAmmo:
					SpawnVFX("IMPACT", collisionPos);
					break;
				case ItemType.Melee:
					SpawnVFX("IMPACT", collisionPos);
					break;
			}
		};

		player.OnEquipmentCollidedWithGuard += (guard, collisionPos, type) =>
		{
			bool isAlive = guardManager.Guards[guard].CanBeTargeted;

			if(isAlive == false)
			{
				return;
			}

			switch (type)
			{
				case ItemType.DamageAmmo:
					SpawnVFX("SHIELD", guard.transform.position);
					break;
				case ItemType.Melee:
					SpawnVFX("SHIELD", guard.transform.position);
					break;
			}
		};

		guardEvents.AddSpawnerHasRespawnedListener((guard) =>
		{
			SpawnVFX("RESPAWN", guard.transform.position);
		});
	}

	private void SpawnVFX(string ID, Vector3 position)
	{
		UpdatePool();

		ParticleSystem vfx = pool.Get(ID);
		activeVFX.Add(new ActiveVFX()
		{
			ID = ID,
			vfx = vfx
		});

		vfx.transform.position = position;
		vfx.Play(true);
	}

	private void UpdatePool()
	{
		foreach(ActiveVFX active in activeVFX)
		{
			if (HasVFXFinishedPlaying(active))
			{
				clearedCache.Add(active);
				pool.Return(active.ID, active.vfx);
			}
		}

		foreach (ActiveVFX toClear in clearedCache)
		{
			activeVFX.Remove(toClear);
		}
		clearedCache.Clear();
	}

	private bool HasVFXFinishedPlaying(ActiveVFX active)
	{
		return active.vfx.gameObject.activeSelf == false;
	}

	private class ActiveVFX
	{
		public string ID;
		public ParticleSystem vfx;
	}
}
