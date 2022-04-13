using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSuspicion 
{
	public Action<SuspicionState, SuspicionState, Transform> OnSuspicionStateUpdated = delegate { };

	private float currentSuspicion;
	private SuspicionState currentSuspicionState;

	private Transform firstObjectInView;
	private Guard.SuspicionData suspicionData;
	private GuardHealth healthController;

	public GuardSuspicion(GuardVision vision, Guard.SuspicionData suspicionData, Guard guard, GuardHealth healthController)
	{
		this.healthController = healthController;
		this.suspicionData = suspicionData;
		vision.OnObjectsInView += BuildSuspicion;
		vision.OnObjectsInAwarenessZone += BuildSuspicion;
		vision.OnNoObjectInView += DecaySuspicion;
		guard.OnDamageTaken += (damage, damageSource) => DamageTaken(damageSource);
	}

	private void BuildSuspicion(List<GameObject> ObjectsInView)
	{
		firstObjectInView = ObjectsInView[0].transform;
		currentSuspicion += suspicionData.suspicionBuildRate * Time.deltaTime;
		currentSuspicion = Mathf.Clamp(currentSuspicion, 0.0f, 1.0f);
		CheckThresholds();
	}

	private void DecaySuspicion()
	{
		firstObjectInView = null;
		currentSuspicion -= suspicionData.suspicionDecayRate * Time.deltaTime;
		currentSuspicion = Mathf.Clamp(currentSuspicion, 0.0f, 1.0f);
		CheckThresholds();
	}

	private void CheckThresholds()
	{

		if (currentSuspicionState != SuspicionState.Pursuing
					&& currentSuspicion >= suspicionData.pursuingThreshold)
		{
			OnSuspicionStateUpdated(SuspicionState.Pursuing, currentSuspicionState, firstObjectInView);
			currentSuspicionState = SuspicionState.Pursuing;
		}
		else if (currentSuspicionState != SuspicionState.Patrolling
			&& currentSuspicion < suspicionData.pursuingThreshold)
		{
			OnSuspicionStateUpdated(SuspicionState.Patrolling, currentSuspicionState, null);
			currentSuspicionState = SuspicionState.Patrolling;
		}

		
		// TODO: Implement Investigating behaviour
		//else if (CurrentSuspicionState != SuspicionState.Investigating
		//&& CurrentSuspicion > InvestigatingThreshold)
		//{
			//UpdateObservers(SuspicionState.Investigating, CurrentSuspicionState, firstObjectInView);
			//CurrentSuspicionState = SuspicionState.Investigating;
		//}
	}

	private void DamageTaken(Transform damageInstigator)
	{
		if(healthController.IsAlive)
		{
			currentSuspicion = 1.0f;
			OnSuspicionStateUpdated(SuspicionState.Pursuing, currentSuspicionState, damageInstigator);
			currentSuspicionState = SuspicionState.Pursuing;
		}
		
	}
}

public enum SuspicionState
{
	Patrolling,
	Investigating,
	Pursuing
}
