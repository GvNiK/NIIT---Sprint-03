using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SceneAsset
{
	public string assetPath;
	public GameObject asset;

	public void ChangeAsset(string assetPath, Transform parent,
		SceneViewSelector sceneViewSelector)
	{
		GameObject assetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
		GameObject tempAsset = (GameObject)PrefabUtility.InstantiatePrefab(assetPrefab, parent);

		if (asset != null)
		{
			tempAsset.transform.position = asset.transform.position;
			tempAsset.transform.rotation = asset.transform.rotation;
			tempAsset.transform.localScale = asset.transform.localScale;
			GameObject.DestroyImmediate(asset);
		}

		this.assetPath = assetPath;
		asset = tempAsset;
		sceneViewSelector.Select(asset);
	}
}

public class SceneGroup
{
	public List<SceneAsset> assets;
	public GameObject obj;
	public bool expanded;

	public SceneGroup()
	{
		this.assets = new List<SceneAsset>();
	}
}