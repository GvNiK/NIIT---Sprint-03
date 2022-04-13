using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PickupPool
{
	private Dictionary<ItemType, Pool<Pickup>> pools;
	private GameObject container;

	public PickupPool(PickupFactory factory)
	{
		container = new GameObject("Pickup Container");
		pools = new Dictionary<ItemType, Pool<Pickup>>();
		foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
		{
			pools[type] = new Pool<Pickup>(() =>
			{
				return factory.Create(type);
			},
			(pickup) => pickup.gameObject.SetActive(true),
			(pickup) =>
			{
				pickup.gameObject.SetActive(false);
				pickup.transform.SetParent(container.transform);
			});
		}
	}

	public Pickup Get(ItemType type)
	{
		return pools[type].Get();
	}

	public void Return(Pickup pickup)
	{
		pools[pickup.type].Return(pickup);
	}
}
