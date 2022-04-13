using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MenuUIController : MonoBehaviour
{
	public Action OnLoadComplete = delegate { };
	public Action OnMovingToMainMenu = delegate { };
	public Action OnAchievementsScreenRequested = delegate { };
	public GameObject TitleScreenUI;
    public GameObject MainMenuUI;
	public LevelSelectController levelSelectUI;

	// The button on the title screen
	public Button TitleScreenContinueButton;

    // The buttons on the main menu
    public Button LevelSelectButton;
	public Button AchievementsButton;
	public Button QuitButton;

	public void Awake()
    {
        SetupButtonListeners();
        InitialiseTitleScreen();
    }

	public void Start()
	{
		OnLoadComplete();	
	}

	private void InitialiseTitleScreen()
    {
		// Should be enabled by default but we will ensure so here
		TitleScreenUI.SetActive(true);
    }

    private void SetupButtonListeners()
    {
        // Add listeners to each buttons onClick functionality
        TitleScreenContinueButton.onClick.AddListener(() =>
        {
			TitleScreenUI.SetActive(false);
			MainMenuUI.SetActive(true);
			OnMovingToMainMenu();
		});

        LevelSelectButton.onClick.AddListener(() =>
        {
			MainMenuUI.SetActive(false);
			levelSelectUI.Show();
        });

		levelSelectUI.OnBackButtonClicked += () =>
		{
			levelSelectUI.Hide();
			MainMenuUI.SetActive(true);
		};

		AchievementsButton.onClick.AddListener(() =>
		{
			OnAchievementsScreenRequested();
		});

        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
