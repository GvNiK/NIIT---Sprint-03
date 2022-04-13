using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LevelEditorAssetDropdown
{
	private static readonly string[] ASSET_DIRECTORIES = new string[]
	{
		"Assets/Prefabs/Environment/Barriers",
		"Assets/Prefabs/Environment/Props",
		"Assets/Prefabs/Environment/Terrain",
	};

	private LevelAssets levelAssets;
	private string[] directories;

	public LevelEditorAssetDropdown()
	{
		levelAssets = new LevelAssets();
		foreach(string assetDir in ASSET_DIRECTORIES)
		{
			levelAssets.AddDirectory(assetDir);
		}
		directories = levelAssets.directories.Select(x => x.directoryName).ToArray();
	}

	public void Render(string selectedAsset, Action<string> OnSelectedAssetChanged)
	{
		string selectedDirectory = Path.GetDirectoryName(selectedAsset);
		AssetDirectory selectedDirectoryObj = levelAssets.directories.Find(x => Path.GetFullPath(x.directoryName) == Path.GetFullPath(selectedDirectory));

		int currentDirectoryIndex = IndexOf(directories, selectedDirectory);
		int newDirectoryIndex = EditorGUILayout.Popup(currentDirectoryIndex, directories);
		if(currentDirectoryIndex != newDirectoryIndex)
		{
			AssetDirectory newDirectoryObj = levelAssets.directories[newDirectoryIndex];
			OnSelectedAssetChanged(newDirectoryObj.assetPaths[0]);
			return;
		}

		int currentAssetIndex = Array.IndexOf(selectedDirectoryObj.assetPaths, selectedAsset);
		int newAssetIndex = EditorGUILayout.Popup(currentAssetIndex, selectedDirectoryObj.assetNames);
		if(currentAssetIndex != newAssetIndex)
		{
			OnSelectedAssetChanged(selectedDirectoryObj.assetPaths[newAssetIndex]);
			return;
		}
	}

	private int IndexOf(string[] paths, string path)
	{
		for(int i = 0; i < paths.Length; i++)
		{
			if(PathCompare(paths[i], path))
			{
				return i;
			}
		}

		return -1;
	}

	private bool PathCompare(string path1, string path2)
	{
		return Path.GetFullPath(path1) == Path.GetFullPath(path2);
	}

	public string DefaultAsset
	{
		get
		{
			return levelAssets.directories[0].assetPaths[0];
		}
	}
}
