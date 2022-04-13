using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneSignalListener : MonoBehaviour
{
    public Action OnCutsceneComplete = delegate {};

    public void CutsceneComplete()
    {
        OnCutsceneComplete();
    }
}
