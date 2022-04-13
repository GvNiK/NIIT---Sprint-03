using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController
{
	public Action OnPaused = delegate { };
	public Action OnResumed = delegate { };

	public void Pause()
	{
		Time.timeScale = 0f;
		OnPaused();
	}

	public void Resume()
	{
		Time.timeScale = 1f;
		OnResumed();
	}
}
