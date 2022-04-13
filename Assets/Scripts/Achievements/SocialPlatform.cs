using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SocialPlatform 
{
    public abstract void Login(Action OnSuccess);
    public abstract void UnlockAchievement(string name, string Id);
    public abstract bool IsAuthenticated { get; }
}
