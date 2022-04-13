using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InventorySubMenuController
{
	private GameObject subMenuPrefab;
	private InventorySubMenuItem subMenuItemPrefab;
	private GameObject subMenu;

	public InventorySubMenuController(GameObject subMenuPrefab,
		InventorySubMenuItem subMenuItemPrefab)
	{
		this.subMenuPrefab = subMenuPrefab;
		this.subMenuItemPrefab = subMenuItemPrefab;
	}

	public void Show(Transform parent, List<Entry> entries)
	{
		if(subMenu != null)
		{
			GameObject.Destroy(subMenu);
		}

		subMenu = GameObject.Instantiate(subMenuPrefab, parent, false);

		foreach(Entry entry in entries)
		{
			GameObject itemObj = GameObject.Instantiate(subMenuItemPrefab.gameObject, subMenu.transform, false);
			InventorySubMenuItem item = itemObj.GetComponent<InventorySubMenuItem>();
			
			item.label.text = entry.label;
			item.button.onClick.AddListener(() =>
			{
				Hide();
				entry.OnClicked();
			});
		}
	}

	public void Hide()
	{
		GameObject.Destroy(subMenu);
	}

	public class Entry
	{
		public Action OnClicked = delegate { };
		public string label;

		public Entry(Action OnClicked, string label)
		{
			this.OnClicked = OnClicked;
			this.label = label;
		}
	}
}
