using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [HideInInspector]
    public Transform target;

    private void Update() 
    {
        if(target != null)
        {
            transform.position = target.position;
        }    
    } 

}
