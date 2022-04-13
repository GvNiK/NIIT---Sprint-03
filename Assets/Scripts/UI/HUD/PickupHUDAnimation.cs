using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PickupHUDAnimation : MonoBehaviour
{
	public Transform pickupContainer;
	public GameObject prefab;
	public ItemIcons icons;
	public Animator inventoryButtonAnimator;

	public void OnItemPickedUp(ItemType type)
	{
		GameObject instance = GameObject.Instantiate(prefab, pickupContainer, false);

		Image icon = instance.GetComponentInChildren<Image>();
		icon.sprite = icons.Get(type);

		AnimationListener animListener = instance.GetComponent<AnimationListener>();
		animListener.OnAnimationEvent += (param) =>
		{
			switch (param)
			{
				case "AddingToInventory":
					PlayInventoryPulse();
					break;
				case "AnimationComplete":
					GameObject.Destroy(instance);
					break;
			}
		};
	}

	private void PlayInventoryPulse()
	{
		inventoryButtonAnimator.SetTrigger("Pulse");
	}
}
