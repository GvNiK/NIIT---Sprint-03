using System;
using UnityEngine;

public class PlayerInputCallbacks
{
	public Action<Vector2> OnPlayerMoveFired = delegate { };

	public Action OnPlayerTapFired = delegate { };
	public Action OnPlayerTapReleased = delegate { };

	public Action OnPlayerStartUseFired = delegate { };
	public Action OnPlayerEndUseFired = delegate { };

	public Action OnPlayerPauseRequested = delegate { };
	public Action OnPlayerInventoryOpenRequested = delegate { };
}
