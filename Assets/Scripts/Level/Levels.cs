using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Levels 
{
	public static readonly List<Data> ALL = new List<Data>()
	{
		new Data(0, "Camp Rutland", "Scenes/BaseLevels/LevelOne-Movement"),
		new Data(1, "Backdoor Breach", "Scenes/BaseLevels/LevelTwo-Sword"),
		new Data(2, "From Under Their Noses", "Scenes/BaseLevels/LevelThree-Guards"),
		new Data(3, "Outpost Heist", "Scenes/BaseLevels/LevelFour-TwoGuards"),
		new Data(4, "In Too Deep", "Scenes/BaseLevels/LevelFive-Bow"),
	};

	public class Data
	{
		public int ID;
		public string name;
		public string scenePath;

		public Data(int ID, string name, string scenePath)
		{
			this.ID = ID;
			this.name = name;
			this.scenePath = scenePath;
		}
	}
}
