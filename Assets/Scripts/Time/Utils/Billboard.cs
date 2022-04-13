using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	private Transform cam;

	private void Start()
	{
		cam = Camera.main.transform;
	}

	private void LateUpdate()
	{
		Vector3 focusPosition = transform.position + cam.rotation * Vector3.forward;
		Vector3 focusUp = cam.rotation * Vector3.up;
		transform.LookAt(focusPosition, focusUp);
	}
}
