using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VFXLibrary : MonoBehaviour
{
	public List<Entry> data;

	public VFXLibrary()
	{
		data = new List<Entry>();
	}

	public Entry Find(string ID)
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
		public string ID;
		public GameObject obj;
	}
}
