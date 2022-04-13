using UnityEngine;

public class PlayerViewRelativeMovement
{
	private PlayerInputCallbacks inputCallbacks;
	private PlayerMovementController movementController;

	private Vector2 movementVector;

	public PlayerViewRelativeMovement(PlayerMovementController movementController, 
		PlayerInputCallbacks inputCallbacks)
	{
		this.inputCallbacks = inputCallbacks;
		this.movementController = movementController;

		SubscribeToPlayerInputCallbacks();
	}

	public void SubscribeToPlayerInputCallbacks()
	{
		inputCallbacks.OnPlayerMoveFired += SetMovementVector;
	}

	private void SetMovementVector(Vector2 movementVector)
	{
		this.movementVector = movementVector;
	}

	public void Destroy()
	{
		inputCallbacks.OnPlayerMoveFired -= SetMovementVector;
	}

	public void Update(Vector3 viewForward)
	{
		Move(movementVector, viewForward);
	}

	private void Move(Vector2 movementVector, Vector3 viewForward)
	{
		Vector3 viewRight = -Vector3.Cross(viewForward.normalized, Vector3.up);
		viewForward.y = 0f;
		viewRight.y = 0f;
		viewForward.Normalize();
		viewRight.Normalize();

		Vector3 relativeMovementVector = new Vector3(movementVector.x, 0f, movementVector.y);
		Vector3 cameraRelativeMovementVector = viewForward * relativeMovementVector.z + viewRight * relativeMovementVector.x;

		movementController.MoveTo(cameraRelativeMovementVector);
	}
}
