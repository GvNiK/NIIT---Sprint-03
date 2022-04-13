using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SFXLibrary : MonoBehaviour
{
	public List<Entry> data;

	public SFXLibrary()
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
		public AudioClip clip;
		[Range(0f, 1f)]
		public float volume = 1f;
		[Range(0f, 0.5f)]
		public float pitchVariance = 0f;
	}
}
