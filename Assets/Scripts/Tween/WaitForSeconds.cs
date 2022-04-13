using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForSeconds : Tween
{
    private float targetTime;
    private float stateTime;

    public WaitForSeconds(float targetTime)
    {
        this.targetTime = targetTime;
    }
    public override void Start()
    {
        if (targetTime <= 0f)
        {
            OnComplete();
        }
    }

    public override void Update()
    {
        stateTime += Time.deltaTime;
        if(stateTime >= targetTime)
        {
            OnComplete();
        }
    }
}
