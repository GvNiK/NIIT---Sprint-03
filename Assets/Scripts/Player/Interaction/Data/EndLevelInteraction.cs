using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class EndLevelInteraction : InteractionPoint
{
	public PlayableDirector playableDirector;
	public PlayableAsset playableAsset;
	public Renderer monitorRenderer;
	public int materialIndex;
	public Texture locked;
	public Texture unlocked;
	public GameObject unlockedVFX;
}
