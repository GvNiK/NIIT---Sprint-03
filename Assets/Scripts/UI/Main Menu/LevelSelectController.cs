using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelSelectController : MonoBehaviour
{
	public Transform buttonContainer;
	public GameObject buttonPrefab;
	public Action OnBackButtonClicked = delegate { };

	public void UpdateButtons(LevelSelectData levelSelectData)
	{
		ClearList();

		foreach(LevelSelectButtonData buttonData in levelSelectData.buttons)
		{
			GameObject buttonInstance = GameObject.Instantiate(buttonPrefab, buttonContainer, false);

			LevelButton levelButton = buttonInstance.GetComponent<LevelButton>();
			levelButton.text.text = buttonData.name;
			levelButton.button.onClick.AddListener(() => buttonData.OnClicked());

			if(buttonData.locked)
			{
				levelButton.Lock();
			}
			else if(buttonData.score > 0)
			{
				levelButton.Unlock(buttonData.score);
			}
			else
			{
				levelButton.Unlock();
			}
		}
	}

	private void ClearList()
	{
		foreach(Transform child in buttonContainer)
		{
			GameObject.Destroy(child.gameObject);
		}
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Back()
	{
		OnBackButtonClicked();
	}
}
