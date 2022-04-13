using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class LevelIntroUIController
{
	private PlayerInputBroadcaster playerInput;
	private CanvasGroup canvasGroup;
	private CanvasGroupTween activeTween;

	public LevelIntroUIController(Transform hudTransform,
		PlayerInputBroadcaster playerInput, int levelID)
	{
		this.playerInput = playerInput;

		canvasGroup = hudTransform.Find("LevelName").GetComponent<CanvasGroup>();
		TextMeshProUGUI levelNumber = canvasGroup.transform.Find("LevelNumber").GetComponent<TextMeshProUGUI>();
		levelNumber.text = "Level" + (levelID + 1);
		TextMeshProUGUI levelName = canvasGroup.transform.Find("LevelName").GetComponent<TextMeshProUGUI>();
		levelName.text = Levels.ALL.Find(x => x.ID == levelID).name;
		canvasGroup.alpha = 0f;

		StartTween(new CanvasGroupTween(canvasGroup, 1f, 2f));

		playerInput.Callbacks.OnPlayerMoveFired += OnPlayerMoved;
	}

	private void StartTween(CanvasGroupTween tween)
	{
		activeTween = tween;
		activeTween.OnComplete += () => activeTween = null;
		activeTween.Start();
	}

	public void Update()
	{
		if(activeTween != null)
		{
			activeTween.Update();
		}
	}

	private void OnPlayerMoved(Vector2 direction)
	{
		playerInput.Callbacks.OnPlayerMoveFired -= OnPlayerMoved;
		Hide();
	}

	private void Hide()
	{
		StartTween(new CanvasGroupTween(canvasGroup, 0f, 2f));
	}
}
