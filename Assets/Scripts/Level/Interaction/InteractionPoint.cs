using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    public Transform interactionTarget;
    public Transform faceTarget;
    public Transform playerInteractionOffset;
    public CollisionCallbacks collisionCallbacks;
}
