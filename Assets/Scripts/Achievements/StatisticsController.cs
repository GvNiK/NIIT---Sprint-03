using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StatisticsController
{
	public Action<int> OnGuardKilled = delegate { };
	private int guardsKilled;

	private void GuardKilled()
	{
		guardsKilled++;
		OnGuardKilled(guardsKilled);
	}
}
