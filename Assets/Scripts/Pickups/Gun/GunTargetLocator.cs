using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GunTargetLocator
{
	private GuardManager guardManager;
	private PlayerSettings playerSettings;
	private PlayerObjectData playerObjectData;

	public GunTargetLocator(GuardManager guardManager, PlayerSettings playerSettings, PlayerObjectData playerObjectData)
	{
		this.guardManager = guardManager;
		this.playerSettings = playerSettings;
		this.playerObjectData = playerObjectData;
	}

	public void Locate(Action<Transform> OnSuccess)
	{
		Transform closestEnemy = null;
		foreach (KeyValuePair<Guard, GuardController> guard in guardManager.Guards)
		{
			if (guard.Value.CanBeTargeted == false)
			{
				continue;
			}

			Vector3 guardPosition = new Vector3(guard.Key.transform.position.x, guard.Key.visionData.eyeHeight, guard.Key.transform.position.z);
			Vector3 playerToEnemy = guardPosition - playerObjectData.Head.position;
			Ray rayToEnemy = new Ray(playerObjectData.Head.position, playerToEnemy);
			RaycastHit hit;

			if (Physics.Raycast(rayToEnemy, out hit, playerSettings.ShotCheckRadius))
			{
				if (hit.transform.tag.Equals("Enemy"))
				{
					Vector3 direction = guard.Key.target.position - playerObjectData.Head.position;
					float angle = Vector3.Angle(direction, playerObjectData.Head.forward);

					if (Mathf.Abs(angle) <= playerSettings.ShotCheckAngle)
					{
						if (closestEnemy == null)
						{
							closestEnemy = guard.Key.target;
						}
						else
						{
							Vector3 playerPositionFloor = playerObjectData.Head.position;
							playerPositionFloor.y = 0;
							Vector3 guardPositionFloor = guard.Key.transform.position;
							guardPositionFloor.y = 0;
							float checkingGuardDistance = Vector3.Distance(playerPositionFloor, guardPositionFloor);

							Vector3 closestGuardPositionFloor = closestEnemy.position;
							closestGuardPositionFloor.y = 0;
							float currentGuardDistance = Vector3.Distance(playerPositionFloor, closestGuardPositionFloor);

							if (checkingGuardDistance < currentGuardDistance)
							{
								closestEnemy = guard.Key.target;
							}
						}
					}
				}
			}
		}

		if(closestEnemy != null)
		{
			OnSuccess(closestEnemy);
		}
	}
}
