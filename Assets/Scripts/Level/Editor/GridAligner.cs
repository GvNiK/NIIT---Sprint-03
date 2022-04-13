using UnityEditor;
using UnityEngine;

public class GridAligner : EditorWindow
{
	[MenuItem("Window/AlignToGrid")]
	public static void ShowWindow()
	{
		GetWindow<GridAligner>("Align To Grid");
	}
	private void OnGUI()
	{
		GUILayout.Label("Make a selection and press the button to move them to the nearest 1");
		if (GUILayout.Button("Align Selected"))
		{
			AlignSelected();
		}
	}

	private void AlignSelected()
	{
		var transforms = Selection.transforms;

		foreach (var trans in transforms)
		{
			trans.position = new Vector3(
				Mathf.RoundToInt(trans.position.x), 
				Mathf.RoundToInt(trans.position.y), 
				Mathf.RoundToInt(trans.position.z)
				);

			trans.rotation = Quaternion.Euler(new Vector3(
				Mathf.RoundToInt(trans.rotation.eulerAngles.x),
				Mathf.RoundToInt(trans.rotation.eulerAngles.y),
				Mathf.RoundToInt(trans.rotation.eulerAngles.z)
				));
		}
	}
}
