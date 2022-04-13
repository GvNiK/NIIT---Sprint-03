using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GlobalAudioController
{
	private SFXController sfxController;
	public GlobalAudioController()
	{
		GameObject audio = new GameObject("GlobalAudio");
		GameObject.DontDestroyOnLoad(audio);
		AudioMixer mixer = Resources.Load<AudioMixer>("Audio/MasterMixer");
		sfxController = new SFXController(audio, mixer, Resources.Load<SFXLibrary>("Audio/SFXLibrary"));
	}

	public void OnLevelSelectLoaded(Scene scene, MenuUIController menu)
	{
		menu.OnLoadComplete += () =>
		{
			OnSceneLoadComplete(scene);
		};
	}

	public void OnLevelLoaded(Scene scene, LevelController levelController)
	{
		levelController.OnLoadComplete += () =>
		{
			OnSceneLoadComplete(scene);
		};
	}

	private void OnSceneLoadComplete(Scene scene)
	{
		foreach (GameObject rootObj in scene.GetRootGameObjects())
		{
			ButtonAudio[] buttons = rootObj.GetComponentsInChildren<ButtonAudio>(true);
			foreach (ButtonAudio button in buttons)
			{
				button.OnClickAudioRequested += () => sfxController.Play("BUTTON_CLICK");
			}
		}
	}
}
