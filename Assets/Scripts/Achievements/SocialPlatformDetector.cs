using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SocialPlatformDetector 
{
    public static SocialPlatform Get()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            return new AndroidSocialPlatform();
#elif UNITY_IOS && !UNITY_EDITOR
            return new IOsSocialPlatform();
#else
        return new DummySocialPlatform();
#endif
    }
}
