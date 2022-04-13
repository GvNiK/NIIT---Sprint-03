using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public ItemType type;
	public bool isRequiredToFinishLevel;
	public int quantity = 1;
}
