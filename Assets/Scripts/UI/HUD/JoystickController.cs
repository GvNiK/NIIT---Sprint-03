using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

[AddComponentMenu("Input/JoystickButton")]
public class JoystickController : OnScreenControl
{
    private Vector2 initialJoystickControlPosition;
    private PlayerInputCallbacks playerInputCallbacks;

    public GraphicRaycaster graphicsRaycaster;
    public GameObject joystick;
    public GameObject joystickButton;

	public Image up;
	public Image down;
	public Image left;
	public Image right;

	private Vector2 currentMovementVector;

    [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public void Setup(PlayerInputCallbacks playerInputCallbacks)
    {
        this.playerInputCallbacks = playerInputCallbacks;

        playerInputCallbacks.OnPlayerTapFired += () => OnTap();
        playerInputCallbacks.OnPlayerTapReleased += () => OnTapReleased();
		playerInputCallbacks.OnPlayerMoveFired += (move) => UpdateMoveHeat(move);
    }

	private void UpdateMoveHeat(Vector2 movement)
	{
		Color full = Color.white;
		full.a = movement.y;
		up.color = full;
		full.a = -movement.y;
		down.color = full;
		full.a = movement.x;
		right.color = full;
		full.a = -movement.x;
		left.color = full;
	}

    private void OnTap()
    {
        Vector2 mousePosition = Pointer.current.position.ReadValue();
        PointerEventData ped = new PointerEventData(null);
        ped.position = mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicsRaycaster.Raycast(ped, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.tag.Equals("ValidJoystickArea"))
            {
                ActivateJoystick(result.screenPosition);
            }
        }
    }

    private void OnTapReleased()
    {
        DeactivateJoystick();
    }

    private void ActivateJoystick(Vector2 hitScreenPosition)
    {
        joystick.GetComponent<CanvasGroup>().alpha = 1f;
        joystick.transform.position = hitScreenPosition;

        initialJoystickControlPosition = hitScreenPosition;
        joystickButton.transform.position = initialJoystickControlPosition;
    }

    private void DeactivateJoystick()
    {
        SendValueToControl(Vector2.zero);
        playerInputCallbacks.OnPlayerMoveFired(Vector3.zero);
        joystick.GetComponent<CanvasGroup>().alpha = 0.3f;
        joystickButton.transform.position = initialJoystickControlPosition;
    }
}
