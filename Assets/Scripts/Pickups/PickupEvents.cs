using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PickupEvents
{
	public Action<Pickup> OnPickupCollected = delegate { };
	public Action OnPickupDropped = delegate { };
	public Action<int> OnRemainingPickupCountUpdated = delegate{ };
}