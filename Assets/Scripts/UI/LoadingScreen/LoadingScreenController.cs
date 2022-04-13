using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingScreenController
{
	private GameObject gameObject;
	private Animator animator;
	private AnimationListener animationListener;
	public LoadingScreenController()
	{
		GameObject prefab = Resources.Load<GameObject>("UI/LoadingScreen");
		gameObject = GameObject.Instantiate(prefab);
		GameObject.DontDestroyOnLoad(gameObject);
		animator = gameObject.GetComponent<Animator>();
		animationListener = gameObject.GetComponent<AnimationListener>();
	}

	public void Show(Action OnCanLoad)
	{
		animationListener.OnAnimationEvent = (param) =>
		{
			if (param == "animInComplete")
			{
				OnCanLoad();
			}
		};
		animator.SetBool("show", true);
	}

	public void Hide(Action OnCanContinue)
	{
		animationListener.OnAnimationEvent = (param) =>
		{
			if (param == "animOutComplete")
			{
				OnCanContinue();
			}
		};
		animator.SetBool("show", false);
	}
}
