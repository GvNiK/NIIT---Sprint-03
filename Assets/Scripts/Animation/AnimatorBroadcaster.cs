using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBroadcaster : MonoBehaviour
{
    public Action OnAnimatorMoveFired = delegate { };
    public Action<int> OnAnimatorIKFired = delegate { };

    public void OnAnimatorMove()
    {
        OnAnimatorMoveFired.Invoke();
    }

    public void OnAnimatorIK(int layerIndex)
    {
        OnAnimatorIKFired.Invoke(layerIndex);
    }
}
