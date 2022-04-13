using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditor : EditorWindow
{
	[MenuItem("NIIT/Level Editor")]
	static void ShowEditor()
	{
		LevelEditor window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
		window.Show();
	}

	private LevelEditorAssetDropdown assetDropdown;
	private Transform levelRoot;
	private Transform environmentRoot;
	private SceneViewSelector sceneViewSelector;
	private Vector2 scrollPosition;
	private NavigationEditor navigationEditor;
	private SceneAssetEditor sceneAssetEditor;
	private SceneGuardEditor sceneGuardEditor;

	private void OnEnable()
	{
		if(IsInValidScene() == false)
		{
			Close();
		}

		assetDropdown = new LevelEditorAssetDropdown();
		sceneViewSelector = new SceneViewSelector();

		sceneAssetEditor = new SceneAssetEditor(sceneViewSelector, assetDropdown);
		sceneAssetEditor.OnRenderDividerRequested += RenderDivider;

		sceneGuardEditor = new SceneGuardEditor(sceneViewSelector);
		sceneGuardEditor.OnRenderDividerRequested += RenderDivider;

		ReloadCurrentLevel();
	}

	private bool IsInValidScene()
	{
		return SceneManager.GetActiveScene().name != "Menu";
	}

	private void ReloadCurrentLevel()
	{
		LevelLoader levelLoader = new LevelLoader();
		levelLoader.Load((levelRoot, environmentRoot, sceneGroups,
			guardRoot, sceneGuards, escapePoint) =>
		{
			this.levelRoot = levelRoot;
			this.environmentRoot = environmentRoot;
			navigationEditor = new NavigationEditor(environmentRoot, escapePoint, sceneViewSelector);
			sceneAssetEditor.OnLevelLoaded(environmentRoot, sceneGroups);
			sceneGuardEditor.OnLevelLoaded(guardRoot, sceneGuards);
		});
	}

	private void Update()
	{
		sceneViewSelector.Update();
	}

	private void OnGUI()
	{
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

		sceneViewSelector.RenderSettings();
		RenderDivider();

		navigationEditor.Render();
		RenderDivider();

		sceneAssetEditor.Render();
		RenderDivider();

		sceneGuardEditor.Render();
		RenderDivider();

		EditorGUILayout.EndScrollView();
	}

	private void RenderDivider()
	{
		Rect rect = EditorGUILayout.GetControlRect(false, 2f);
		rect.height = 2f;
		EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
	}
}
