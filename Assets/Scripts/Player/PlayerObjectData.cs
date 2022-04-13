using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerObjectData : MonoBehaviour
{
    public Transform Blaster;
    public Transform Sword;
    public Transform Head;
    public Animator PlayerAnimator;

    //S3 - Assignment 05
    public ChainIKConstraint leftHandIK;
    public TargetController setTargetController;
}
