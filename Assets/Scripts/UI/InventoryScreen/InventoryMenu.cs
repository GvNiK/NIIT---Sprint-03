using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryMenu : MonoBehaviour
{
	public ItemIcons icons;
	public Transform itemGrid;
	public Button close;
	public Image mainEquipmentIcon;
	public Image secondaryEquipmentIcon;
	public TextMeshProUGUI secondaryEquipmentCount;
	public GameObject subMenuPrefab;
	public InventorySubMenuItem subMenuItemPrefab;
}
