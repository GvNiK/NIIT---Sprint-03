using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemIcons : MonoBehaviour
{
	public List<ItemIcon> items;

	public Sprite Get(ItemType type)
	{
		foreach(ItemIcon item in items)
		{
			if(item.type == type)
			{
				return item.icon;
			}
		}

		return null;
	}

	public Sprite GetInactive(ItemType type)
	{
		foreach (ItemIcon item in items)
		{
			if (item.type == type)
			{
				return item.inactiveIcon;
			}
		}

		return null;
	}
}

[Serializable]
public class ItemIcon
{
	public ItemType type;
	public Sprite icon;
	public Sprite inactiveIcon;
}
