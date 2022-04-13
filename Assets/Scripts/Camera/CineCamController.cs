using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CineCamController : MonoBehaviour
{
    //private MainCamera camera;
    
    public CollisionCallbacks collisionCallbacks;
    //private CameraController cameraController;
    public CinemachineVirtualCameraBase vcam;
    public bool isFollow;
    public bool isLookAt = true;
    

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        collisionCallbacks.OnTriggerEntered += (collision) =>
        {
            if (collision.transform.tag.Equals("Player"))
                vcam.gameObject.SetActive(true);
        };
        collisionCallbacks.OnTriggerExited += (collision) =>
        {
            if (collision.transform.tag.Equals("Player"))
                vcam.gameObject.SetActive(false);
        };
    }

    protected virtual void Update()
    {
        
    }
   
    public virtual void SetCameraTarget(Transform player)
    {
        if(isLookAt)
        {
            vcam.LookAt = player;
        }

        if(isFollow)
        {
            vcam.Follow = player;
        }
    }
}
