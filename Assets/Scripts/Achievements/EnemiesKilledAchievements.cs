using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesKilledAchievements
{
    private string achivementId;
    private string name;
    private int guardsKilled;
    private int killTarget;
    private int pacifistMinimum;
    private int guardsInLevel;
    public void Reset()
    {
        guardsKilled = 0;
    }
    public EnemiesKilledAchievements(string name, int target, int pacifistMinimum, string achivementId)
    {
        this.name = name;
        this.killTarget = target;
        this.achivementId = achivementId;
        this.pacifistMinimum = pacifistMinimum;
    }
    public void OnGuardKilled()
    {
        guardsKilled++;
    }
    public void OnGuardsSetup(int totalGuards)
    {
        guardsInLevel = totalGuards;
    }
    public void TryUnlock(Action<string, string> OnSuccess)
    {
        if(guardsKilled == 0 && killTarget == 0 && guardsInLevel >= pacifistMinimum)
        {
            OnSuccess(name, achivementId);
        }
        else if (guardsKilled >= killTarget)
        {
            OnSuccess(name, achivementId);
        }
    }
}
