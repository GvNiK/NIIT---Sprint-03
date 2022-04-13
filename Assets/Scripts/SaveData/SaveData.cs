using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SaveData
{
	public List<Level> levels;

	public SaveData()
	{
		levels = new List<Level>();
	}

	public class Level
	{
		public int ID;
		public float score;
	}
}
