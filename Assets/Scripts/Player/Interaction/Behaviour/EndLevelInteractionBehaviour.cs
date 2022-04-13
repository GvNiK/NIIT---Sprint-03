using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EndLevelInteractionBehaviour : InteractionBehaviour
{
	private bool canInteract;

	public EndLevelInteractionBehaviour(EndLevelInteraction data, PickupEvents pickupEvents)
	{
		pickupEvents.OnRemainingPickupCountUpdated += (pickupCount) =>
		{
			canInteract = pickupCount == 0;
			UpdateMonitorVisuals(data);
			UpdateVFX(data);
		};
	}

	private void UpdateMonitorVisuals(EndLevelInteraction data)
	{
		Material material = data.monitorRenderer.materials[data.materialIndex];
		if(canInteract)
		{
			material.SetTexture("_MainTex", data.unlocked);
			material.SetTexture("_EmissionMap", data.unlocked);
		}
		else
		{
			material.SetTexture("_MainTex", data.locked);
			material.SetTexture("_EmissionMap", data.locked);
		}
	}


	private void UpdateVFX(EndLevelInteraction data)
	{
		data.unlockedVFX.SetActive(canInteract);
	}

	public override bool CanInteract()
	{
		return canInteract;		
	}
}
