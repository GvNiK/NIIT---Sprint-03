using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySocialPlatform : SocialPlatform
{
    public override bool IsAuthenticated { get { return true; } }
    public override void Login(Action OnSuccess)
    {
        Debug.Log("Login Attempted !!!");
    }
    public override void UnlockAchievement(string name, string Id)
    {
        Debug.Log("Achievements Unlocked : " + name + " " + Id);
    }
}
