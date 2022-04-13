using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteAchievement
{
    private int targetLevel;
    private string achivementId;
    private string name;
    public LevelCompleteAchievement(string name, int targetLevel, string achivementId)
    {
        this.name = name;
        this.targetLevel = targetLevel;
        this.achivementId = achivementId;
    }
    public void TryUnlock(int completedLevelId, Action<string, string> OnSuccess)
    {
        if(completedLevelId == targetLevel)
        {
            OnSuccess(name, achivementId);
        }
    }
}
