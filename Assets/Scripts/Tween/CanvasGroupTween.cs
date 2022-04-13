using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupTween : Tween
{
	private CanvasGroup target;
	private float alpha;
	private float initialAlpha;
	private float targetTime;
	private float stateTime;

	public CanvasGroupTween(CanvasGroup target, float alpha,
		float time)
	{
		this.target = target;
		this.alpha = alpha;
		initialAlpha = target.alpha;
		this.targetTime = time;
	}

	public override void Start()
	{
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

			target.alpha = Mathf.Lerp(initialAlpha, alpha, percentComplete);

			if (stateTime >= targetTime)
			{
				OnComplete();
			}
		}
	}
}
