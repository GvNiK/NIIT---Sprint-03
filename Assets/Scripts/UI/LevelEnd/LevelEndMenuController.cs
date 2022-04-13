using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndMenuController
{
	public Action<Levels.Data> OnLevelLoadRequested = delegate { };
	public Action OnRetryLevelRequested = delegate { };
	public Action OnExitRequested = delegate { };
	private LevelEndMenu data;
	private LevelStatsController levelStatsController;

	public LevelEndMenuController(LevelEndMenu data, int currentLevelID, LevelStatsController levelStatsController)
	{
		this.data = data;
		this.levelStatsController = levelStatsController;

		GetNextLevel(currentLevelID, (nextLevel) =>
		{
			data.nextLevelButton.gameObject.SetActive(true);

			data.nextLevelButton.onClick.AddListener(() =>
			{
				OnLevelLoadRequested(nextLevel);
			});

		}, () => data.nextLevelButton.gameObject.SetActive(false));

		data.retryButton.onClick.AddListener(() =>
		{
			OnRetryLevelRequested();
		});

		data.exitToLevelSelectButton.onClick.AddListener(() =>
		{
			OnExitRequested();
		});

		Hide();
	}

	private void GetNextLevel(int currentLevelID, 
		Action<Levels.Data> OnNextLevelFound, Action OnNoNextLevel)
	{
		for(int i = 0; i < Levels.ALL.Count; i++)
		{
			if(Levels.ALL[i].ID == currentLevelID)
			{
				if((i + 1) < Levels.ALL.Count)
				{
					OnNextLevelFound(Levels.ALL[i + 1]);
					return;
				}
			}
		}

		OnNoNextLevel();
	}

	public void Show(string message, bool levelComplete)
	{
		data.gameObject.SetActive(true);

		data.message.text = message;
		data.score.text = levelStatsController.LevelTime.Value.ToString("0.00");
		data.enemiesKilled.text = levelStatsController.EnemiesKilled.Value.ToString();
		data.pickupsCollected.text = levelStatsController.PickupsCollected.Value + "/" + levelStatsController.PickupsCollected.Total;

		if (!levelComplete)
		{
			data.scoreContainer.gameObject.SetActive(false);
			data.nextLevelButton.gameObject.SetActive(false);
		}
	}

	public void Hide()
	{
		data.gameObject.SetActive(false);
	}
}
