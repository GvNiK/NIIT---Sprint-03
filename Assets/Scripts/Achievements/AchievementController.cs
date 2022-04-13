using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementController 
{
    //List of Achievments

    //Level Complete
    private List<LevelCompleteAchievement> levelCompleteAchievement;
    private List<ItemConsumedAchievement> itemConsumedAchievement;
    private List<EnemiesKilledAchievements> enemiesKilledAchievements;
    private List<PickupAchievement> pickupAchievements;
    private List<SpeedAchievement> speedAchievements;

    //Killing Achivement
    private SocialPlatform socialPlatform;
    //Platform we're on
    //saveData <- if we add 5 Kills to our total in a Level
    //private SaveData saveData;

    public AchievementController(SaveData saveData)
    {
        
        socialPlatform = SocialPlatformDetector.Get();

        levelCompleteAchievement = new List<LevelCompleteAchievement>()
		{
			new LevelCompleteAchievement("Into the Unknown", 0, "CgkIjbD2oasEEAIQAA"),
			new LevelCompleteAchievement("Starman", 2, "CgkIjbD2oasEEAIQCA"),
			new LevelCompleteAchievement("Tress-spacing", 3, "CgkIjbD2oasEEAIQCg"),
			new LevelCompleteAchievement("Explorer", 4, "CgkIjbD2oasEEAIQAg"),
		};

        itemConsumedAchievement = new List<ItemConsumedAchievement>()
		{
			new ItemConsumedAchievement("Blaster Master", ItemType.ExplosiveAmmo, "CgkIjbD2oasEEAIQCQ")
		};

        enemiesKilledAchievements = new List<EnemiesKilledAchievements>()
        {
            new EnemiesKilledAchievements("Mass murderer", 2, 2, "CgkIjbD2oasEEAIQAA"),
            new EnemiesKilledAchievements("Pacifest", 0, 2, "CgkIjbD2oasEEAIQAA")
        };

        pickupAchievements = new List<PickupAchievement>()
        {
            new PickupAchievement("Scavenger", 5, "CgkIjbD2oasEEAIQAA")
        };

        speedAchievements = new List<SpeedAchievement>()
        {
            new SpeedAchievement("Punch it, Hughie", 120, SpeedAchievement.SpeedAchievementType.All, "CgkIjbD2oasEEAIQAA"),
            new SpeedAchievement("Speedrunner", 30, SpeedAchievement.SpeedAchievementType.Any, "CgkIjbD2oasEEAIQAA")
        };

        foreach (SpeedAchievement speedAchievement in speedAchievements)
        {
            speedAchievement.Load(saveData);
        }
    }

    public void OnLevelComplete(int levelIndex, float score)
    {
        foreach(LevelCompleteAchievement levelCompleteAchievement in levelCompleteAchievement)
		{
			levelCompleteAchievement.TryUnlock(levelIndex, (name, achievementID) => socialPlatform.UnlockAchievement(name, achievementID));
            //{ Debug.Log(name + " " + achievementID); });
		}

        foreach (EnemiesKilledAchievements enemiesKilledAchievement in enemiesKilledAchievements)
        {
            enemiesKilledAchievement.TryUnlock( (name, achievementId) => socialPlatform.UnlockAchievement(name, achievementId));
            enemiesKilledAchievement.Reset();
        }

        foreach (PickupAchievement pickupAchievement in pickupAchievements)
        {
            pickupAchievement.OnLevelComplete();
        }

        foreach (SpeedAchievement speedAchievement in speedAchievements)
        {
            speedAchievement.TryUnlock(score, levelIndex, (name, achievementId) => socialPlatform.UnlockAchievement(name, achievementId));
        }
    }

    public void OnItemConsumed(ItemType type)
    {
        foreach (ItemConsumedAchievement itemConsumed in itemConsumedAchievement)
        {
            itemConsumed.TryUnlock((type), (name, achievementId) => socialPlatform.UnlockAchievement(name, achievementId));
        }
    }

    public void OnGuardKilled()
    {
        foreach (EnemiesKilledAchievements enemiesKilledAchievement in enemiesKilledAchievements)
        {
            enemiesKilledAchievement.OnGuardKilled();
        }
    }

    public void OnGuardsSetup(int totalGuards)
    {
        foreach (EnemiesKilledAchievements enemiesKilledAchievement in enemiesKilledAchievements)
        {
            enemiesKilledAchievement.OnGuardsSetup(totalGuards);
        }
    }

    public void OnPickupCollected()
    {
        foreach (PickupAchievement pickupAchievement in pickupAchievements)
        {
            pickupAchievement.OnPickupCollected((name, achievementID) => socialPlatform.UnlockAchievement(name, achievementID));
        }
    }

}
