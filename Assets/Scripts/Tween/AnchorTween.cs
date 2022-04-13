using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorTween : Tween
{
	private RectTransform target;
	private Vector2 anchors;
	private Vector3 initialAnchors;
	private float targetTime;
	private float stateTime;
	private Func<float, float, float, float> interpolationXFunc;
	private Func<float, float, float, float> interpolationYFunc;

	public AnchorTween(RectTransform target,
		Vector2 anchors, float time) : this(target,
			anchors, time, Linear, Linear) { }

	public AnchorTween(RectTransform target,
		Vector2 anchors, float time,
		Func<float, float, float, float> interpolationXFunc,
		Func<float, float, float, float> interpolationYFunc)
	{
		this.target = target;
		this.anchors = anchors;
		this.targetTime = time;
		this.interpolationXFunc = interpolationXFunc;
		this.interpolationYFunc = interpolationYFunc;
	}

	public override void Start()
	{
		initialAnchors = target.anchorMin;

		if (targetTime <= 0f)
		{
			OnComplete();
		}
	}

	public override void Update()
	{
		if (stateTime < targetTime)
		{
			stateTime += Time.deltaTime;

			float percentComplete = stateTime / targetTime;
			float x = interpolationXFunc(initialAnchors.x, anchors.x, percentComplete);
			float y = interpolationYFunc(initialAnchors.y, anchors.y, percentComplete);

			target.anchorMin = target.anchorMax = new Vector2(x, y);

			if (stateTime >= targetTime)
			{
				OnComplete();
			}
		}
	}

	public static float SmoothStep(float a, float b, float progress)
	{
		return Mathf.SmoothStep(a, b, progress);
	}

	public static float Linear(float a, float b, float progress)
	{
		return Mathf.Lerp(a, b, progress);
	}

	public static float CubicIn(float a, float b, float progress)
	{
		float spread = b - a;
		return a + (progress * progress * progress * spread);
	}
}
