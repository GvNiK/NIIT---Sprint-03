using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConsumedAchievement
{
    private ItemType targetItem;
    private string achivementId;
    private string name;
    public ItemConsumedAchievement(string name, ItemType targetItem, string achivementId)
    {
        this.name = name;
        this.targetItem = targetItem;
        this.achivementId = achivementId;
    }
    public void TryUnlock(ItemType type, Action<string, string> OnSuccess)
    {
        if (type != ItemType.DamageAmmo)
        {
            OnSuccess(name, achivementId);
        }
    }
}
