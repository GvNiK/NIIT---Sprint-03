using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
	public TextMeshProUGUI text;
	public Button button;
	public TextMeshProUGUI score;
	public GameObject lockView;
	public GameObject unlockView;

	public void Lock()
	{
		button.interactable = false;
		unlockView.SetActive(false);
		lockView.SetActive(true);
		text.color = Color.grey;
	}

	public void Unlock()
	{
		SetUnlocked();
	}

	public void Unlock(float score)
	{
		SetUnlocked();

		float minutes = (int)score / 60;
		float seconds = score % 60;

		this.score.text = minutes.ToString("00") + ":" + seconds.ToString("0.0");
	}

	private void SetUnlocked()
	{
		score.text = "--:--";
		text.color = Color.white;
		button.interactable = true;
		lockView.SetActive(false);
		unlockView.SetActive(true);
	}
}
