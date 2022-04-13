using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth
{
	public Action<float> OnDamageTaken = delegate { };
	public Action OnKilled = delegate { };

	private float maxHealth;
	private float currentHealth;

	public PlayerHealth(float maxHealth)
	{
		this.maxHealth = maxHealth;
		currentHealth = maxHealth;
	}

	public void TakeDamage(float damageTaken)
	{
		currentHealth -= damageTaken;

		if (currentHealth <= 0)
		{
			OnKilled.Invoke();
			currentHealth = 0;
		}

		OnDamageTaken.Invoke(currentHealth);
	}
}
