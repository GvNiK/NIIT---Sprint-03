using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GuardUIController
{
	private Dictionary<Guard, HealthBar> healthBars;
	private GameObject healthBarPrefab;

	public GuardUIController(GameObject healthBarPrefab, GuardEvents guardEvents)
	{
		this.healthBarPrefab = healthBarPrefab;
		healthBars = new Dictionary<Guard, HealthBar>();
		guardEvents.AddSpawnedListener(GuardSpawned);
		guardEvents.AddHitListener(GuardHit);
		guardEvents.AddKilledListener(GuardKilled);
	}

	private void GuardSpawned(Guard guard, GuardController _)
	{
		GameObject healthBar = GameObject.Instantiate(healthBarPrefab, guard.generalData.infoContainer, false);
		healthBars[guard] = new HealthBar(healthBar);
		healthBar.SetActive(false);
	}

	private void GuardHit(Guard guard, float health, float maxHealth)
	{
		HealthBar healthBar = healthBars[guard];
		healthBar.fill.fillAmount = health / maxHealth;

		healthBar.barObj.SetActive(true);
	}

	private void GuardKilled(Guard guard)
	{
		healthBars[guard].barObj.SetActive(false);
	}

	private class HealthBar
	{
		public GameObject barObj;
		public Image fill;

		public HealthBar(GameObject barObj)
		{
			this.barObj = barObj;
			fill = barObj.transform.Find("Background/Fill").GetComponent<Image>();
		}
	}
}
