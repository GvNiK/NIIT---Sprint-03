using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GuardHealth
{
	public Action<float, float> OnDamageTaken = delegate { };
	public Action OnKilled = delegate { };
	private Guard.GeneralData healthData;
	private float currentHealth;

	public GuardHealth(Guard.GeneralData healthData)
	{
		this.healthData = healthData;
		currentHealth = healthData.maxHealth;
	}

	public void TakeDamage(float damageTaken)
	{
		currentHealth -= damageTaken;

		OnDamageTaken.Invoke(currentHealth, healthData.maxHealth);

		if (currentHealth <= 0)
		{
			OnKilled.Invoke();
			currentHealth = 0;
		}
	}

	public bool IsAlive
	{
		get
		{
			return (currentHealth > 0);
		}
	}
}
