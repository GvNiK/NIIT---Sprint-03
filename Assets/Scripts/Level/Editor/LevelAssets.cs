using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LevelAssets
{
	public List<AssetDirectory> directories;

	public LevelAssets()
	{
		directories = new List<AssetDirectory>();
	}

	public void AddDirectory(string path)
	{
		string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { path });

		string[] paths = guids.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray();

		directories.Add(new AssetDirectory(path, paths));
	}
}

public class AssetDirectory
{
	public string directoryName;
	public string[] assetPaths;
	public string[] assetNames;

	public AssetDirectory(string directoryName, string[] assetPaths)
	{
		this.directoryName = directoryName;
		this.assetPaths = assetPaths;
		assetNames = assetPaths.Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
	}
}
