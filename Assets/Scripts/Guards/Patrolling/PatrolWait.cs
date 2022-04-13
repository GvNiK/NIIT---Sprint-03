using UnityEngine;

public class PatrolWait : PatrolCommand
{
	private float waitDuration;
	private float timeWaited = 0.0f;
	public PatrolWait(float waitDuration)
	{
		this.waitDuration = waitDuration;
	}

	public override void Begin(){}

	public override void End(){}

	public override void Update()
	{
		timeWaited += Time.deltaTime;
		
		if (timeWaited >= waitDuration)
		{
			CompleteCommand();
		}
	}
}
