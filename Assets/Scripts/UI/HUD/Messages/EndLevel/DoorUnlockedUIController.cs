using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DoorUnlockedUIController
{
	private CanvasGroup canvasGroup;
	private Tween activeTween;

	public DoorUnlockedUIController(Transform hudTransform)
	{
		canvasGroup = hudTransform.Find("DoorUnlockedMessage").GetComponent<CanvasGroup>();
	}

	public void OnDoorUnlocked()
	{
		activeTween = new CanvasGroupTween(canvasGroup, 1f, 1f);
		activeTween.OnComplete += () =>
		{
			activeTween = new WaitForSeconds(2f);
			activeTween.OnComplete += () =>
			{
				activeTween = new CanvasGroupTween(canvasGroup, 0f, 1f);
				activeTween.OnComplete += () => activeTween = null;
				activeTween.Start();
			};
			activeTween.Start();
		};
		activeTween.Start();
	}

	public void Update()
	{
		if (activeTween != null)
		{
			activeTween.Update();
		}
	}
}

