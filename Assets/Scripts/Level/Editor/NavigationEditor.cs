using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class NavigationEditor
{
	private NavMeshSurface navMesh;
	private Transform escapePoint;
	private SceneViewSelector sceneViewSelector;

	public NavigationEditor(Transform environmentRoot, Transform escapePoint,
		SceneViewSelector sceneViewSelector)
	{
		this.escapePoint = escapePoint;
		this.sceneViewSelector = sceneViewSelector;

		navMesh = environmentRoot.GetComponent<NavMeshSurface>();
		if(navMesh == null)
		{
			navMesh = environmentRoot.gameObject.AddComponent<NavMeshSurface>();
		}
	}

	public void Render()
	{
		GUILayout.Label("Navigation Settings", EditorStyles.boldLabel);

		using (new EditorGUILayout.HorizontalScope())
		{
			if (GUILayout.Button("Update NavMesh"))
			{
				navMesh.BuildNavMesh();
			}

			if (GUILayout.Button("Select Escape Point"))
			{
				sceneViewSelector.Select(escapePoint.gameObject);
			}
		}
	}
}
