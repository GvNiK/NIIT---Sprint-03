using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
	private const string MAIN_MENU = "Scenes/Menu";

	public void Load(string scenePath, Action OnComplete)
	{
		AsyncOperation loadOp = SceneManager.LoadSceneAsync(scenePath);
		loadOp.completed += (op) => OnComplete();
	}

	public void LoadMainMenu(Action OnComplete)
	{
		AsyncOperation loadOp = SceneManager.LoadSceneAsync(MAIN_MENU);
		loadOp.completed += (op) => OnComplete();
	}
}
