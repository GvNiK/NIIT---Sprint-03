using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardEmoteController
{
	private GuardEmote emotes;

	public GuardEmoteController(GuardSuspicion guardSuspicion, Transform emotes)
	{
		guardSuspicion.OnSuspicionStateUpdated += SwitchEmote;
		this.emotes = emotes.GetComponent<GuardEmote>();
	}

	private void SwitchEmote(SuspicionState newState, SuspicionState oldState, Transform alertingObject)
	{
		switch (newState)
		{
			case SuspicionState.Patrolling:
				if (oldState != SuspicionState.Patrolling)
				{
					// Guard has lost the player
					emotes.ShowLostPlayerEmote();
				}
				break;
			case SuspicionState.Investigating:
				if (oldState == SuspicionState.Patrolling)
				{
					// Guard is suspicious of something
					emotes.ShowSuspiciousEmote();
				}
				break;
			case SuspicionState.Pursuing:
				if (oldState == SuspicionState.Patrolling)
				{
					// Guard is suspicious of something
					emotes.ShowAlertedEmote();
				}
				break;
			default:
				break;
		}
	}

	public void HideEmotes()
	{
		emotes.HideEmote();
	}
}