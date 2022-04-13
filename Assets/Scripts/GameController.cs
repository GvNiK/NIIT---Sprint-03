using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void CreateGameController()
	{
		GameObject gameController = new GameObject("GameController");
		GameObject.DontDestroyOnLoad(gameController);
		gameController.AddComponent<GameController>();
	}

	private SaveDataController saveDataController;
	private SaveData saveData;
	private LoadingScreenController loadingScreen;
	private GlobalAudioController globalAudio;
	private AchievementController achievementController;	//S3 - Assignment 02

	private void Awake()
	{
		saveDataController = new SaveDataController();
		saveData = saveDataController.Load();
		loadingScreen = new LoadingScreenController();
		globalAudio = new GlobalAudioController();
		achievementController = new AchievementController(saveData);	//S3 - Assignment 02

		SceneManager.sceneLoaded += (scene, loadType) => OnNewSceneLoaded(scene);
	}

	private void OnNewSceneLoaded(Scene scene)
	{
		if (scene.name == "Menu")
		{
			OnLevelSelectLoaded(scene);
			return;
		}

		foreach (GameObject rootObj in scene.GetRootGameObjects())
		{
			LevelController levelController = rootObj.GetComponent<LevelController>();

			if (levelController != null)
			{
				OnLevelLoaded(scene);
				return;
			}
		}
	}

	private void OnLevelSelectLoaded(Scene scene)
	{
		MenuUIController menus = FindObjectOfType<MenuUIController>();

		LevelSelectController levelSelect = menus.levelSelectUI;
		levelSelect.UpdateButtons(CreateLevelData());

		globalAudio.OnLevelSelectLoaded(scene, menus);
	}

	private LevelSelectData CreateLevelData()
	{
		LevelSelectData levelSelectData = new LevelSelectData();
		bool previousLevelCompleted = true;
		foreach (Levels.Data level in Levels.ALL)
		{
			LevelSelectButtonData buttonData = new LevelSelectButtonData();

			buttonData.name = level.name;

			SaveData.Level levelData = saveData.levels.FirstOrDefault(x => x.ID == level.ID);

			if (levelData != null)
			{
				buttonData.score = levelData.score;
			}
			else if (previousLevelCompleted)
			{
				previousLevelCompleted = false;
			}
			else
			{
				buttonData.locked = true;
			}

			if(buttonData.locked == false)
			{
				buttonData.OnClicked += () => OnSceneLoadRequested(level);
			}

			levelSelectData.buttons.Add(buttonData);
		}

		return levelSelectData;
	}

	private void OnSceneLoadRequested(Levels.Data level)
	{
		loadingScreen.Show(() =>
		{
			SceneLoader sceneLoader = new SceneLoader();
			sceneLoader.Load(level.scenePath, () => loadingScreen.Hide(() => { }));
		});
	}

	private void OnLevelSelectLoadRequested()
	{
		loadingScreen.Show(() =>
		{
			SceneLoader sceneLoader = new SceneLoader();
			sceneLoader.LoadMainMenu(() => loadingScreen.Hide(() => { }));
		});

	}

	private void OnLevelLoaded(Scene scene)
	{
		LevelController levelController = FindObjectOfType<LevelController>();
		if (levelController != null)
		{
			levelController.OnLevelComplete += (score) =>
			{
				saveDataController.UpdateScore(saveData, levelController.levelID, score);
				saveDataController.Save(saveData);
				achievementController.OnLevelComplete(levelController.levelID, score);		//S3 - Assignment 02
			};

			//S3 - Assignment 02
			levelController.OnItemConsumed += (item) => achievementController.OnItemConsumed(item);	
			levelController.OnGuardKilled += () => achievementController.OnGuardKilled();		
			levelController.OnGuardsSetup += achievementController.OnGuardsSetup;
			levelController.OnPickupCollected += () => achievementController.OnPickupCollected();

			levelController.OnLevelLoadRequest += (levelID) => OnSceneLoadRequested(levelID);
			levelController.OnExitRequest += () => OnLevelSelectLoadRequested();

			globalAudio.OnLevelLoaded(scene, levelController);
		}
	}
}
