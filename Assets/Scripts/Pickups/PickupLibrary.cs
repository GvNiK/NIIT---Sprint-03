using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PickupLibrary : MonoBehaviour
{
	public List<Entry> data;

	public PickupLibrary()
	{
		data = new List<Entry>();
	}

	public Entry Find(ItemType ID)
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
		public ItemType ID;
		public GameObject obj;
	}
}
