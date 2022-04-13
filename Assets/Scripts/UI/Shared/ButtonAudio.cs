using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
	public UnityAction OnClickAudioRequested = delegate { };

	private void Start()
	{
		OnButtonsChanged();
	}

	public void OnButtonsChanged()
	{
		Button[] buttons = GetComponentsInChildren<Button>();
		foreach(Button button in buttons)
		{
			button.onClick.RemoveListener(OnClickAudioRequested);
			button.onClick.AddListener(OnClickAudioRequested);
		}
	}
}
