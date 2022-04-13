using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAchievement
{
    private int targetTime;
    private string achivementId;
    private string name;
    private SpeedAchievementType achievementType;
    private List<SaveData.Level> levels;
    public SpeedAchievement(string name, int targetTime, SpeedAchievementType achievementType, string achivementId)
    {
        this.name = name;
        this.targetTime = targetTime;
        this.achivementId = achivementId;
        this.achievementType = achievementType;
    }

    public void TryUnlock(float completedTime, int completedLevelId, Action<string, string> OnSuccess)
    {
        if (achievementType == SpeedAchievementType.Any && completedTime <= targetTime)
        {
            OnSuccess(name, achivementId);
        }
        else if (achievementType == SpeedAchievementType.All )
        {
            if(levels.Count > 1)
            {
                return; 
            }
            if(levels[0].ID == completedLevelId)
            {
                if(completedTime<targetTime)
                {
                    OnSuccess(name, achivementId);
                }
            }
        }
    }

    public enum SpeedAchievementType
    {
        All,
        Any
    }

    public void Load(SaveData saveData)
    {
        int totalLevels = Levels.ALL.Count;
        levels = saveData.levels;
        for (int i = levels.Count - 1; i >= 0; i--)
        {
            if(levels[i].score <= targetTime)
            {
                levels.Remove(levels[i]);
            }            
        }
        //Debug.Log("Levels Setup !");
    }
}
