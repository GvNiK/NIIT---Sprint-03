
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineSignalListener : MonoBehaviour
{
	public Action<string> OnMessage = delegate { };
	public void SignalRecieved(string value)
	{
		OnMessage(value);
	}
}
