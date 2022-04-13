using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndCutscene 
{
    private Player player; 
    private GuardManager guardManager;
    private EndLevelInteraction endLevelInteraction;
    private IKWeightTween iKWeightTween;

    public LevelEndCutscene(Player player, GuardManager guardManager)//, EndLevelInteraction endLevelInteraction)
    {
        this.player = player;
        this.guardManager = guardManager;
        //this.endLevelInteraction = endLevelInteraction;
    }

    public void PlayComplete(EndLevelInteraction endLevel, Action CompleteLevel)
    {
        player.Broadcaster.EnableActions(ControlType.None);
		guardManager.ForceIdle();

        //Step - 01
        float distFromOffset = Vector3.Distance(player.ObjectData.transform.position, endLevel.playerInteractionOffset.position);
        //Debug.Log(distFromOffset);
        if(distFromOffset <= 0.5f)
        {
            //Step - 02
            player.Controller.Face(endLevel.faceTarget.position, () =>
            {   
                //Step - 03
                player.ObjectData.setTargetController.target = endLevel.interactionTarget;
                Debug.Log(player.ObjectData.setTargetController.target);

                //Step - 04: Increase the Weight of the IKChain.
                iKWeightTween = new IKWeightTween(player.ObjectData.leftHandIK, endLevel);
                //Debug.Log(iKWeightTween);
                iKWeightTween.OnIKComplete += () =>
                {
                    iKWeightTween = null;
                    player.Animator.SetTrigger("ButtonPush");

                    CutsceneSignalListener cutsceneListener = endLevel.GetComponentInChildren<CutsceneSignalListener>();
                    cutsceneListener.OnCutsceneComplete += CompleteLevel;
                };
                    //endLevel.playableDirector.playableAsset = endLevel.playableAsset;
                    endLevel.playableDirector.Play();


            });
        }     
    }

    public void Update() 
    {
        if(iKWeightTween != null)
        {
            iKWeightTween.Update();
        }    
    }
}
