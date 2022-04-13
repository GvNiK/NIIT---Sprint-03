using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class GameplayInput : ControlMapping.IGameplayActions
{
	private ControlMapping controlMapping;
	private PlayerInputCallbacks callbacks;

	public GameplayInput(ControlMapping controlMapping,
		PlayerInputCallbacks callbacks)
	{
		this.controlMapping = controlMapping;
		this.callbacks = callbacks;
		controlMapping.Gameplay.SetCallbacks(this);
	}

	public void Enable()
	{
		controlMapping.Gameplay.Enable();
	}

	public void Disable()
	{
		controlMapping.Gameplay.Disable();
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		Vector2 movementVector = context.ReadValue<Vector2>();
		callbacks.OnPlayerMoveFired.Invoke(movementVector);
	}

	public void OnInventory(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				callbacks.OnPlayerInventoryOpenRequested.Invoke();
				break;
		}
	}

	public void OnPause(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				callbacks.OnPlayerPauseRequested.Invoke();
				break;
		}
	}

	public void OnUse(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Started:
				callbacks.OnPlayerStartUseFired();
				break;
			case InputActionPhase.Canceled:
			case InputActionPhase.Disabled:
				callbacks.OnPlayerEndUseFired();
				break;
		}
	}

	public void OnTap(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Started:
				callbacks.OnPlayerTapFired.Invoke();
				break;
			case InputActionPhase.Canceled:
			case InputActionPhase.Disabled:
				callbacks.OnPlayerTapReleased.Invoke();
				break;
		}
	}
}
