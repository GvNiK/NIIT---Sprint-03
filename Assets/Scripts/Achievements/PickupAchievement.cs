using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PickupAchievement
{
	private int pickupsCollected;
	private string name;
	private int target;
	private string achievementID;
	public PickupAchievement(string name, int target, string achievementID)
	{
		this.name = name;
		this.target = target;
		this.achievementID = achievementID;
	}
	public void OnPickupCollected(Action<string, string> OnSuccess)
	{
		pickupsCollected++;

		if (pickupsCollected >= target)
		{
			OnSuccess(name, achievementID);
		}
	}
	public void OnLevelComplete()
	{
		Reset();
	}
	private void Reset()
	{
		pickupsCollected = 0;
	}
}
