using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKWeightTween 
{
    private ChainIKConstraint IKChain;
    private EndLevelInteraction endLevel;
    public Action OnIKComplete;
    private float distFromButton;
    
    public IKWeightTween(ChainIKConstraint IKChain, EndLevelInteraction endLevel)
    {
        this.IKChain = IKChain;
        this.endLevel = endLevel;
    }

    private void Start() 
    {
        if(IKChain.weight == 1.0f)
        {
            IKChain.weight = 0.0f;
        }
    }

    public void Update() 
    {
        distFromButton = Vector3.Distance(endLevel.interactionTarget.position, IKChain.data.target.position);
        distFromButton = Mathf.Clamp01(distFromButton);

        IKChain.weight = distFromButton;
        OnIKComplete();
        Debug.Log("IKWeight: " + IKChain.weight);
        Debug.Log("DistanceFromButton: " + distFromButton);

        // if(distFromButton == 0f)
        // {
        //     IKChain.weight = 1.0f;
        //     OnIKComplete();
        //     //Debug.Log("This is called!");
        // }

    }
   
}
