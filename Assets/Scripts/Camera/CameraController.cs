using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;


public class CameraController
{
    private GameObject cameraObject;
    public GameObject mainCameraTransform;
    private Transform cameraPosition;
    private Vector3 cameraOffset;
    private ViewOcclusionManager playerViewOcclusionManager;
    public Transform player;

    public CameraController(GameObject cameraObjectPrefab,
		Transform parent, Transform player)
    {
        this.player = player;
        cameraObject = GameObject.Instantiate(cameraObjectPrefab, null);
        cameraObject.name = "Camera";

        mainCameraTransform = cameraObject.transform.Find("MainCamera").gameObject;
        playerViewOcclusionManager = new ViewOcclusionManager(this);
        cameraOffset = mainCameraTransform.transform.position - player.position;
        
        //S3 - Assignment 04
        SetPlayerCamera(cameraObject.transform);
        CineVirtualCameras();

    }

    private void CineVirtualCameras()   //S3 - Assignment 04
    {
        foreach(CineCamController cams in GameObject.FindObjectsOfType<CineCamController>())
        {
            cams.SetCameraTarget(player);
        }
    }

    private void SetPlayerCamera(Transform cameraObject)    //S3 - Assignment 04
    {
        GameObject defaultCamera = cameraObject.transform.Find("VirtualCamera").gameObject;
        CinemachineVirtualCameraBase vcam = defaultCamera.GetComponent<CinemachineVirtualCameraBase>();
        vcam.LookAt = player.transform;
        vcam.Follow = player.transform;
    }

    private void UpdateCameraFollow()
    {
        mainCameraTransform.transform.position = player.position + cameraOffset;
    }
    public void Update()
    {
        playerViewOcclusionManager.Update();
        UpdateCameraFollow();
    }

    public Transform Target
    {
        get
        {
            return player;
        }
        set
        {
            player = value;
            player = value;
        }
    }

    public Transform CameraTransform
    {
        get
        {
            return cameraObject.transform;
        }
    }

    public Transform MainCameraTransform
    {
        get
        {
            return mainCameraTransform.transform;
        }
    }
}
