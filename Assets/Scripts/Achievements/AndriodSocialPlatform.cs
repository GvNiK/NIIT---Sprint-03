#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayServices;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

public class AndroidSocialPlatform : SocialPlatform
{
    public override bool IsAuthenticated
    {
        get { return Social.localUser.authenticated; }
    }

    public override void Login(Action OnSuccess)
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((success) =>
        {
            if(success)
            {
                OnSuccess();
            }
        });
    }

    public override void UnlockAchievement(string name, string Id)
    {
        if (IsAuthenticated)
        {
            Social.ReportProgress(Id, 100, (success) => { });
        }
    }
}
#endif
