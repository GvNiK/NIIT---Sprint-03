using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEndMenu : MonoBehaviour	
{
	public TextMeshProUGUI message;
	public Transform scoreContainer;
	public TextMeshProUGUI score;
	public TextMeshProUGUI enemiesKilled;
	public TextMeshProUGUI pickupsCollected;
	public Button nextLevelButton;
	public Button retryButton;
	public Button exitToLevelSelectButton;
}
