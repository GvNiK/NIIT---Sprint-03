using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SceneViewSelector
{
	private static Vector3 OFFSET = new Vector3(0f, 10f, -10f);
	private const float DEFAULT_SNAP_INCREMENT = 1f;
	private GameObject latestSelection;
	private float snapIncrement;

	public SceneViewSelector()
	{
		snapIncrement = DEFAULT_SNAP_INCREMENT;
	}

	public void RenderSettings()
	{
		GUILayout.Label("Snap Settings", EditorStyles.boldLabel);
		snapIncrement = EditorGUILayout.Slider("Increment", snapIncrement, 0.001f, 5f);
	}

	public void Select(GameObject obj)
	{
		latestSelection = obj;
		SceneView.lastActiveSceneView.size = 10f;
		SceneView.lastActiveSceneView.LookAt(obj.transform.position);

		Selection.activeGameObject = obj;
	}

	public void Update()
	{
		if(latestSelection == null)
		{
			return;
		}
		if(Selection.activeGameObject == latestSelection)
		{
			Vector3 position = latestSelection.transform.position;
			position.x = Snap(position.x);
			position.y = Snap(position.y);
			position.z = Snap(position.z);
			latestSelection.transform.position = position;
		}
	}

	private float Snap(float value)
	{
		return snapIncrement * Mathf.Round((value / snapIncrement));
	}

	public bool IsSelected(GameObject obj)
	{
		return obj != null && obj == latestSelection;
	}
}
