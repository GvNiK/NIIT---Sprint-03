using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LevelSelectData
{
	public List<LevelSelectButtonData> buttons;

	public LevelSelectData()
	{
		buttons = new List<LevelSelectButtonData>();
	}
}

public class LevelSelectButtonData
{
	public string name;
	public float score;
	public bool locked;
	public Action OnClicked = delegate { };
}
