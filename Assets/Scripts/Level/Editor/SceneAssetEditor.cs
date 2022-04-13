using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SceneAssetEditor
{
	public System.Action OnRenderDividerRequested = delegate { };
	private List<SceneGroup> sceneGroups;
	private SceneViewSelector sceneViewSelector;
	private Color defaultInspectorColour;
	private LevelEditorAssetDropdown assetDropdown;
	private Transform environmentRoot;

	public SceneAssetEditor(SceneViewSelector sceneViewSelector,
		LevelEditorAssetDropdown assetDropdown)
	{
		this.sceneViewSelector = sceneViewSelector;
		this.assetDropdown = assetDropdown;
		defaultInspectorColour = GUI.backgroundColor;
	}

	public void OnLevelLoaded(Transform environmentRoot, List<SceneGroup> sceneGroups)
	{
		this.environmentRoot = environmentRoot;
		this.sceneGroups = sceneGroups;
	}

	public void Render()
	{
		GUILayout.Label("Asset Groups", EditorStyles.boldLabel);
		foreach (SceneGroup sceneGroup in sceneGroups)
		{
			sceneGroup.expanded = EditorGUILayout.Foldout(sceneGroup.expanded, sceneGroup.obj.name);

			if (sceneGroup.expanded == false)
			{
				continue;
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				string groupName = EditorGUILayout.TextField("Name", sceneGroup.obj.name);

				if (groupName != sceneGroup.obj.name)
				{
					sceneGroup.obj.name = groupName;
				}

				if (GUILayout.Button("Delete Group", GUILayout.Width(150)))
				{
					sceneGroups.Remove(sceneGroup);
					GameObject.DestroyImmediate(sceneGroup.obj);
					return;
				}
			}

			foreach (SceneAsset sceneAsset in sceneGroup.assets)
			{
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
				{
					if (sceneViewSelector.IsSelected(sceneAsset.asset))
					{
						GUI.backgroundColor = Color.red;
					}
					else
					{
						GUI.backgroundColor = defaultInspectorColour;
					}
					using (new EditorGUILayout.HorizontalScope())
					{
						assetDropdown.Render(sceneAsset.assetPath,
						(newAsset) =>
						{
							sceneAsset.ChangeAsset(newAsset,
								sceneGroup.obj.transform, sceneViewSelector);
						});
					}

					using (new EditorGUILayout.HorizontalScope())
					{
						if (GUILayout.Button("Select", GUILayout.Width(150)))
						{
							sceneViewSelector.Select(sceneAsset.asset);
						}

						if (GUILayout.Button("Duplicate", GUILayout.Width(150)))
						{
							Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(sceneAsset.asset);
							string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot);
							SceneAsset newAsset = new SceneAsset();

							newAsset.ChangeAsset(path, sceneGroup.obj.transform, sceneViewSelector);

							newAsset.asset.transform.position = sceneAsset.asset.transform.position;
							newAsset.asset.transform.rotation = sceneAsset.asset.transform.rotation;
							newAsset.asset.transform.localScale = sceneAsset.asset.transform.localScale;

							sceneGroup.assets.Add(newAsset);

							sceneViewSelector.Select(newAsset.asset);
							return;
						}

						if (GUILayout.Button("Delete", GUILayout.Width(150)))
						{
							sceneGroup.assets.Remove(sceneAsset);
							GameObject.DestroyImmediate(sceneAsset.asset);
							return;
						}
					}
				}
			}

			GUI.backgroundColor = defaultInspectorColour;

			if (GUILayout.Button("+", GUILayout.Width(50)))
			{
				SceneAsset sceneAsset = new SceneAsset();
				sceneAsset.ChangeAsset(assetDropdown.DefaultAsset,
					sceneGroup.obj.transform, sceneViewSelector);
				sceneGroup.assets.Add(sceneAsset);
			}

			OnRenderDividerRequested();
		}

		if (GUILayout.Button("+", GUILayout.Width(50)))
		{
			SceneGroup sceneGroup = new SceneGroup();
			sceneGroup.obj = new GameObject("New Group");
			sceneGroup.obj.transform.SetParent(environmentRoot);
			sceneGroups.Add(sceneGroup);
		}
	}
}
