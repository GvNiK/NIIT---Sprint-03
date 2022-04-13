using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OccludingEnvironmentGroupEditor : MonoBehaviour
{
	[MenuItem("NIIT/Setup Occluding Environment Group Item")]
	static void Setup()
	{
		GameObject active = Selection.activeGameObject;
		OccludingEnvironment occludingEnvironment;

		if (active.GetComponent<OccludingEnvironment>() == null)
		{
			active.AddComponent<OccludingEnvironment>();
		}

		if (active.GetComponent<Collider>() == null)
		{
			active.AddComponent<MeshCollider>();
		}

		occludingEnvironment = active.GetComponent<OccludingEnvironment>();

		active.layer = LayerMask.NameToLayer("OccludingEnvironmentGroup");

		occludingEnvironment.originalMaterial = active.GetComponent<MeshRenderer>().sharedMaterial;
		occludingEnvironment.fadeMaterial = FetchFadeMaterial(occludingEnvironment.originalMaterial.name);

		if (occludingEnvironment.fadeMaterial == null)
		{
			Debug.LogWarning("Could not find matching fade material! Please ensure there is a material in the same path named as follows: [materialName]_Fade");
		}
	}

	[MenuItem("NIIT/Remove Occluding Environment Group Item")]
	static void Remove()
	{
		GameObject active = Selection.activeGameObject;
		OccludingEnvironment occludingEnvironment;

		if (active.GetComponent<OccludingEnvironment>() != null)
		{
			occludingEnvironment = active.GetComponent<OccludingEnvironment>();
			Debug.Log("Found matching fade material.");

			active.layer = LayerMask.NameToLayer("Environment");
			GameObject.DestroyImmediate(occludingEnvironment);
		}
	}

	private static Material FetchFadeMaterial(string materialString)
	{
		string source = "Assets/Prefabs/Environment/Meshes/Materials/" + materialString.Replace(" (Instance)", "") + "_Fade.mat";
		Debug.Log("Looking for: " + source);
		return AssetDatabase.LoadAssetAtPath(source, typeof(Material)) as Material;
	}
}
