using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PickupFactory
{
	private PickupLibrary library;

	public PickupFactory(PickupLibrary library)
	{
		this.library = library;
	}

	public Pickup Create(ItemType type)
	{
		GameObject instance = GameObject.Instantiate(library.Find(type).obj);
		Pickup pickup = instance.GetComponent<Pickup>();

		return pickup;
	}
}
