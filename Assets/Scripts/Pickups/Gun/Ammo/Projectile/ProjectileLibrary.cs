using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectileLibrary : MonoBehaviour
{
	public List<Entry> data;

	public ProjectileLibrary()
	{
		data = new List<Entry>();
	}

	public Entry Find(ProjectileType ID)
	{
		foreach (Entry entry in data)
		{
			if (entry.ID == ID)
			{
				return entry;
			}
		}

		return null;
	}

	[Serializable]
	public class Entry
	{
		public ProjectileType ID;
		public GameObject obj;
	}
}
