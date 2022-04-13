using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class PlayerInputBroadcaster 
{
	private PlayerInputCallbacks callbacks;
	private ControlMapping controlMapping;
	private GameplayInput gameplayInput;

	public PlayerInputBroadcaster()
	{
		callbacks = new PlayerInputCallbacks();
		controlMapping = new ControlMapping();
		gameplayInput = new GameplayInput(controlMapping, callbacks);
		EnableActions(ControlType.Gameplay);
	}

	public void Destroy()
	{
		controlMapping.Dispose();
	}

	public void EnableActions(ControlType controlType)
	{
		DisableActions();

		switch(controlType)
		{
			case ControlType.Gameplay:
				gameplayInput.Enable();
				break;
		}
	}

	private void DisableActions()
	{
		gameplayInput.Disable();
	}

	public PlayerInputCallbacks Callbacks
	{
		get
		{
			return callbacks;
		}
	}
}
