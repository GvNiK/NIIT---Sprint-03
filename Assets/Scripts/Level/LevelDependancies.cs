using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelDependancies : MonoBehaviour
{
	public GameObject cameraContainer;

	public GameObject player;
	public PlayerSettings playerSettings;

	public GameObject enemyHealthUI;
	public GameObject inventoryUI;

	public VFXLibrary vfxLibrary;
	public ProjectileLibrary projectileLibrary;
	public PickupLibrary pickupLibrary;
}
