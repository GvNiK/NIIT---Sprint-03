using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTween : Tween
{
	private Transform target;
	private Vector3 localPosition;
	private Quaternion localRotation;
	private Vector3 initialLocalPosition;
	private Quaternion intialLocalRotation;
	private float targetTime;
	private float stateTime;

	public TransformTween(Transform target, Vector3 localPosition,
		Quaternion localRotation, float time)
	{
		this.target = target;
		this.localPosition = localPosition;
		this.localRotation = localRotation;
		this.targetTime = time;
	}

	public override void Start()
	{
		initialLocalPosition = target.localPosition;
		intialLocalRotation = target.localRotation;

		if(targetTime <= 0f)
		{
			OnComplete();
		}
	}

	public override void Update()
	{
		if(stateTime < targetTime)
		{
			stateTime += Time.deltaTime;

			float percentComplete = stateTime / targetTime;
			target.localPosition = Vector3.Lerp(initialLocalPosition, localPosition, percentComplete);
			target.localRotation = Quaternion.Lerp(intialLocalRotation, localRotation, percentComplete);

			if(stateTime >= targetTime)
			{
				OnComplete();
			}
		}
	}
}
