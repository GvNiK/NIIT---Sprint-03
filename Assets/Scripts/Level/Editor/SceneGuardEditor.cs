using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SceneGuardEditor
{
	public Action OnRenderDividerRequested = delegate { };
	private List<SceneGuard> sceneGuards;
	private SceneViewSelector sceneViewSelector;
	private GameObject guardPrefab;
	private Transform guardRoot;

	public SceneGuardEditor(SceneViewSelector sceneViewSelector)
	{
		this.sceneViewSelector = sceneViewSelector;
		guardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Guards/GuardTail.prefab");
	}

	public void OnLevelLoaded(Transform guardRoot, List<SceneGuard> sceneGuards)
	{
		this.guardRoot = guardRoot;
		this.sceneGuards = sceneGuards;
	}

	public void Render()
	{
		GUILayout.Label("Guards", EditorStyles.boldLabel);

		foreach (SceneGuard sceneGuard in sceneGuards)
		{
			sceneGuard.expanded = EditorGUILayout.Foldout(sceneGuard.expanded, sceneGuard.container.name);

			if (sceneGuard.expanded == false)
			{
				continue;
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				sceneGuard.container.name = EditorGUILayout.TextField("Name", sceneGuard.container.name);

				if (GUILayout.Button("Select", GUILayout.Width(150)))
				{
					sceneViewSelector.Select(sceneGuard.behaviour.gameObject);
				}

				if (GUILayout.Button("Delete Guard", GUILayout.Width(150)))
				{
					sceneGuards.Remove(sceneGuard);
					GameObject.DestroyImmediate(sceneGuard.container.gameObject);
					return;
				}
			}

			GUILayout.Label("Waypoints", EditorStyles.boldLabel);

			foreach (WaypointInfo waypoint in sceneGuard.behaviour.patrolData.waypoints)
			{
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
				{
					waypoint.NodeType = (WaypointType)EditorGUILayout.EnumPopup("Type", waypoint.NodeType);

					switch(waypoint.NodeType)
					{
						case WaypointType.None:
							break;
						case WaypointType.MoveTo:
							using (new EditorGUILayout.HorizontalScope())
							{
								if(waypoint.TransformTarget != null)
								{
									waypoint.TransformTarget.name = EditorGUILayout.TextField("Name", waypoint.TransformTarget.name);

									if (GUILayout.Button("Select", GUILayout.Width(150)))
									{
										sceneViewSelector.Select(waypoint.TransformTarget.gameObject);
									}
								}
								else
								{
									GUILayout.Label("No assigned waypoint");
									if (GUILayout.Button("Create", GUILayout.Width(150)))
									{
										string name = (sceneGuard.behaviour.patrolData.waypoints.Count + 1).ToString();
										GameObject newWaypoint = new GameObject(name);
										newWaypoint.transform.SetParent(sceneGuard.waypointContainer.transform);
										waypoint.TransformTarget = newWaypoint.transform;
									}
								}
								
							}
							break;
						case WaypointType.Rotate:
							EditorGUIUtility.wideMode = true;
							waypoint.TargetRotation = EditorGUILayout.Vector3Field("Target Rotation", waypoint.TargetRotation);
							waypoint.RotationSpeed = EditorGUILayout.FloatField("Speed", waypoint.RotationSpeed);
							break;
						case WaypointType.Wait:
							waypoint.WaitTime = EditorGUILayout.FloatField("Wait Time", waypoint.WaitTime);
							break;
					}
				}
			}

			if (GUILayout.Button("+", GUILayout.Width(50)))
			{
				string name = (sceneGuard.behaviour.patrolData.waypoints.Count + 1).ToString();
				GameObject newWaypoint = new GameObject(name);
				newWaypoint.transform.SetParent(sceneGuard.waypointContainer.transform);
			
				WaypointInfo waypointInfo = new WaypointInfo();
				waypointInfo.NodeType = WaypointType.MoveTo;
				waypointInfo.TransformTarget = newWaypoint.transform;

				sceneGuard.behaviour.patrolData.waypoints.Add(waypointInfo);
			}

			OnRenderDividerRequested();
		}

		if (GUILayout.Button("+", GUILayout.Width(50)))
		{
			GameObject guardContainer = new GameObject("New Guard");
			guardContainer.transform.SetParent(guardRoot);

			GameObject waypointContainer = new GameObject(LevelLoader.WAYPOINT_CONTAINER);
			waypointContainer.transform.SetParent(guardContainer.transform);

			GameObject guardObj = (GameObject)PrefabUtility.InstantiatePrefab(guardPrefab, guardContainer.transform);
			Guard guardScript = guardObj.GetComponent<Guard>();

			SceneGuard sceneGuard = new SceneGuard();
			sceneGuard.container = guardContainer;
			sceneGuard.waypointContainer = waypointContainer;
			sceneGuard.behaviour = guardScript;
			sceneGuards.Add(sceneGuard);
		}
	}
}
