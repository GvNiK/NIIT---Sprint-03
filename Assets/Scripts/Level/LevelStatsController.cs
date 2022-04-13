using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatsController
{
    private IntStatistic enemiesKilled;
    private IntStatistic pickupsCollected;
    private FloatStatistic levelTime;

    public LevelStatsController(GuardManager guardManager, PickupController pickupController)
    {
        enemiesKilled = new IntStatistic("Enemies Killed", 0);
        pickupsCollected = new IntStatistic("Pickups Collected", 0, pickupController.TotalRequiredPickups);
        levelTime = new FloatStatistic("Level Time", 0f);
    }

    public void OnEnemyKilled()
    {
        enemiesKilled.AddToValue(1);
    }

    public void OnRemainingPickupsUpdated(int value)
    {
		pickupsCollected.Value = pickupsCollected.Total - value;
    }

    public void UpdateTime(float timeDelta)
    {
        this.levelTime.AddToValue(timeDelta);
    }

    public IntStatistic EnemiesKilled
    {
        get
        {
            return enemiesKilled;
        }
    }

    public IntStatistic PickupsCollected
    {
        get
        {
            return pickupsCollected;
        }
    }

    public FloatStatistic LevelTime
    {
        get
        {
            return levelTime;
        }
    }
}
