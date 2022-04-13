using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PatrolCommand 
{
	public abstract void Begin();

	public abstract void End();

	public abstract void Update();

	public event Action OnCommandComplete;

	protected virtual void CompleteCommand()
	{
		OnCommandComplete?.Invoke();
	}
}
