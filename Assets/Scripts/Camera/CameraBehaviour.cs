using UnityEngine;

public abstract class CameraBehaviour
{
    public Transform cameraTarget;

    public abstract void Update();

    public void UpdateCameraTarget(Transform newTarget)
    {
        cameraTarget = newTarget;
    }
}
