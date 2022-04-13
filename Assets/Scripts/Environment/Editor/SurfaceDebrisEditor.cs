using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SurfaceDebris))]
public class SurfaceDebrisEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SurfaceDebris script = (SurfaceDebris)target;

		DrawDefaultInspector();

		if (GUILayout.Button("Generate"))
		{
			Generate(script);
		}
	}

	private void Generate(SurfaceDebris script)
	{
		RemovePreviousDetail(script);
		GenerateNewDetail(script);
	}

	private void RemovePreviousDetail(SurfaceDebris script)
	{
		while (script.transform.childCount > 0)
		{
			DestroyImmediate(script.transform.GetChild(0).gameObject);
		}
	}

	private void GenerateNewDetail(SurfaceDebris script)
	{
		Bounds bounds = script.GetComponent<Collider>().bounds;
		GameObject obj;
		Vector3 scale;

		for (int i = 0; i < script.Density; ++i)
		{
			obj = Instantiate(script.Prefabs[Random.Range(0, script.Prefabs.Count)],
				RandomPointInBounds(bounds), Quaternion.Euler(0, Random.Range(0, 359), 0));
			scale = obj.transform.localScale;
			scale += new Vector3(Random.Range(-script.ScaleVarience, script.ScaleVarience),
				Random.Range(-script.ScaleVarience, script.ScaleVarience), Random.Range(-script.ScaleVarience, script.ScaleVarience));
			obj.transform.localScale = scale;
			obj.transform.parent = script.transform;
			obj.GetComponent<MeshRenderer>().scaleInLightmap = script.LightmapScale;
		}
	}

	private Vector3 RandomPointInBounds(Bounds bounds)
	{
		return new Vector3(
			Random.Range(bounds.min.x, bounds.max.x),
			Random.Range(bounds.min.y, bounds.max.y),
			Random.Range(bounds.min.z, bounds.max.z)
		);
	}
}
