using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LevelLoader
{
	public const string WAYPOINT_CONTAINER = "Waypoints";
	private const string LEVEL_ROOT = "LevelController";
	private const string ENVIRONMENT_ROOT = "Environment";
	private const string GUARD_ROOT = "Guards";
	private const string INTERACTION_ROOT = "Interaction";
	private const string ESCAPE_POINT = "EscapePoint";

	public void Load(Action<Transform, Transform, List<SceneGroup>,
		Transform, List<SceneGuard>, Transform> OnLoaded)
	{
		GameObject levelRootObj = GameObject.Find(LEVEL_ROOT);
		Transform levelRoot = null;
		Transform environmentRoot = null;
		List<SceneGroup> groups = new List<SceneGroup>();
		Transform guardRoot = null;
		List<SceneGuard> guards = new List<SceneGuard>();
		Transform escapePoint = null;

		if (levelRootObj != null)
		{
			levelRoot = levelRootObj.transform;
			environmentRoot = levelRoot.Find(ENVIRONMENT_ROOT);

			if (environmentRoot != null)
			{
				foreach (Transform groupTrans in environmentRoot.transform)
				{
					List<SceneAsset> assets = new List<SceneAsset>();

					foreach (Transform assetTrans in groupTrans)
					{
						string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(assetTrans.gameObject);

						if (string.IsNullOrEmpty(path))
						{
							continue;
						}

						SceneAsset sceneAsset = new SceneAsset();
						sceneAsset.assetPath = path;
						sceneAsset.asset = assetTrans.gameObject;

						assets.Add(sceneAsset);
					}

					SceneGroup group = new SceneGroup();

					group.obj = groupTrans.gameObject;
					group.assets = assets;
					groups.Add(group);
				}

			}
			else
			{
				environmentRoot = CreateEnvironmentRoot(levelRoot);
			}

			guardRoot = levelRoot.Find(GUARD_ROOT);
			if (guardRoot != null)
			{
				foreach(Transform guardContainer in guardRoot)
				{
					Transform waypointContainer = guardContainer.Find(WAYPOINT_CONTAINER);

					if(waypointContainer == null)
					{
						waypointContainer = new GameObject(WAYPOINT_CONTAINER).transform;
						waypointContainer.SetParent(guardContainer);
					}

					List<Transform> waypoints = new List<Transform>();
					foreach(Transform waypoint in waypointContainer)
					{
						waypoints.Add(waypoint);
					}

					Guard guardScript = guardRoot.GetComponentInChildren<Guard>();

					SceneGuard sceneGuard = new SceneGuard();
					sceneGuard.container = guardContainer.gameObject;
					sceneGuard.behaviour = guardScript;
					sceneGuard.waypointContainer = waypointContainer.gameObject;
					guards.Add(sceneGuard);
				}
			}
			else
			{
				guardRoot = CreateGuardRoot(levelRoot);
			}

			Transform interactionRoot = levelRoot.Find(INTERACTION_ROOT);
			if(interactionRoot == null)
			{
				CreateInteractionRoot(levelRoot);
			}

			EndLevelInteraction escapePointBehaviour = levelRoot.GetComponentInChildren<EndLevelInteraction>();

			if (escapePointBehaviour != null)
			{
				escapePoint = escapePointBehaviour.transform;
			}
			else
			{
				escapePoint = CreateEscapePoint(levelRoot);
			}
		}
		else
		{
			levelRoot = CreateLevelRoot();
			environmentRoot = CreateEnvironmentRoot(levelRoot);
			guardRoot = CreateGuardRoot(levelRoot);
			CreateInteractionRoot(levelRoot);
			escapePoint = CreateEscapePoint(levelRoot);
		}

		OnLoaded(levelRoot, environmentRoot, groups,
			guardRoot, guards, escapePoint);
	}

	private Transform CreateLevelRoot()
	{
		Transform levelRoot = new GameObject(LEVEL_ROOT).transform;
		levelRoot.gameObject.AddComponent<LevelController>();
		return levelRoot;
	}

	private Transform CreateEnvironmentRoot(Transform levelRoot)
	{
		Transform environmentRoot = new GameObject(ENVIRONMENT_ROOT).transform;
		environmentRoot.SetParent(levelRoot);
		return environmentRoot;
	}

	private Transform CreateGuardRoot(Transform levelRoot)
	{
		Transform guardRoot = new GameObject(GUARD_ROOT).transform;
		guardRoot.SetParent(levelRoot);
		return guardRoot;
	}

	private Transform CreateInteractionRoot(Transform levelRoot)
	{
		Transform interactionRoot = new GameObject(INTERACTION_ROOT).transform;
		interactionRoot.SetParent(levelRoot);
		return interactionRoot;
	}

	private Transform CreateEscapePoint(Transform levelRoot)
	{
		Transform interactionRoot = levelRoot.Find(INTERACTION_ROOT);
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EscapePoint.prefab");
		GameObject escapePoint = (GameObject)PrefabUtility.InstantiatePrefab(prefab, interactionRoot);
		return escapePoint.transform;
	}
}
