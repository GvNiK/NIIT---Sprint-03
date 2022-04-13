using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MixingCamera : CineCamController
{
    private CinemachineMixingCamera mixCam;
    public Transform mixCam1;
    public Transform mixCam2;
    private Transform target;

    protected override void Awake()
    {
        base.Awake();

        mixCam = vcam as CinemachineMixingCamera;   //Casting
        if(mixCam == null)
        {
            Debug.LogError("MixingCamera not found! Please attach a mixing Camera Component.");
        }
    }

    public override void SetCameraTarget(Transform player)
    {
        base.SetCameraTarget(player);
        target = player;
    }

    //protected = Accesible only by Base Class. Abstraction. OOP.
    protected override void Update()
    {
        //Check Dist and Check Wieght
        float targetWeight1 = Vector3.Distance(target.position, mixCam1.transform.position);
        float targetWeight2 = Vector3.Distance(target.position, mixCam2.transform.position);

        mixCam.m_Weight0 = targetWeight1;
        mixCam.m_Weight1 = targetWeight2;

    }

}