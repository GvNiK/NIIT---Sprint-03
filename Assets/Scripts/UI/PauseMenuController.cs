using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
	public Action OnRestartRequested = delegate { };
	public Action OnExitequested = delegate { };
	public Action OnClosed = delegate { };
	public CanvasGroup pauseMenuCanvasGroup;

	public void Show()
    {
        pauseMenuCanvasGroup.alpha = 1;
		pauseMenuCanvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        pauseMenuCanvasGroup.alpha = 0;
		pauseMenuCanvasGroup.blocksRaycasts = false;
		OnClosed();
	}

	public void Resume()
	{
		Hide();
	}

	public void Restart()
	{
		OnRestartRequested();
	}

	public void Exit()
	{
		OnExitequested();
	}
}
