using System.Collections.Generic;
using UnityEngine;

namespace InterstellarFuelSwitch;

public class InterstellarDebugMessages : MonoBehaviour
{
	public enum OutputMode
	{
		screen,
		log,
		both,
		none
	}

	public bool debugMode = true;

	private List<debugLine> outputLines = new List<debugLine>();

	public Rect screenPosition = new Rect(500f, 500f, 300f, 100f);

	public float lineSpacing = 25f;

	public string moduleName = string.Empty;

	private OutputMode outputMode = OutputMode.log;

	public float postToScreenDuration = 5f;

	public InterstellarDebugMessages()
	{
	}

	public InterstellarDebugMessages(bool _debugMode, string _moduleName)
	{
		debugMode = _debugMode;
		outputMode = OutputMode.log;
		moduleName = _moduleName + ": ";
	}

	public InterstellarDebugMessages(bool _debugMode, OutputMode _outputMode, float _postToScreenDuration)
	{
		debugMode = _debugMode;
		outputMode = _outputMode;
		postToScreenDuration = _postToScreenDuration;
	}

	public void Log(string input)
	{
		debugMessage(input);
	}

	public void debugMessage(object input)
	{
		if (debugMode)
		{
			switch (outputMode)
			{
			case OutputMode.both:
				debugMessage(input, postToLog: true, postToScreenDuration);
				break;
			case OutputMode.screen:
				debugMessage(input, postToLog: false, postToScreenDuration);
				break;
			case OutputMode.log:
				break;
			}
		}
	}

	public void debugMessage(object input, bool postToLog, float postToScreenDuration)
	{
		if (debugMode)
		{
			PostMessage(input, postToLog, postToScreenDuration);
		}
	}

	public void debugMessage(object input, float postToScreenDuration)
	{
		if (debugMode)
		{
			switch (outputMode)
			{
			case OutputMode.both:
				debugMessage(input, postToLog: true, postToScreenDuration);
				break;
			case OutputMode.log:
				debugMessage(input, postToLog: true, postToScreenDuration);
				break;
			case OutputMode.screen:
				debugMessage(input, postToLog: false, postToScreenDuration);
				break;
			}
		}
	}

	public void PostMessage(object input, bool postToLog, float postToScreenDuration)
	{
		if (postToLog)
		{
			Debug.Log(moduleName + input);
		}
		if (postToScreenDuration > 0f)
		{
			outputLines.Add(new debugLine(input.ToString(), postToScreenDuration));
		}
	}

	public void OnGUI()
	{
		if (outputLines.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < outputLines.Count; i++)
		{
			GUI.Label(new Rect(screenPosition.x, screenPosition.y + lineSpacing * (float)i, screenPosition.width, screenPosition.height), outputLines[i].text);
			outputLines[i].delay -= Time.deltaTime;
			if (outputLines[i].delay <= 0f)
			{
				outputLines.RemoveAt(i);
				i--;
			}
		}
	}

	public static void Post(string input, bool postToLog, float postToScreenDuration)
	{
		if (postToLog)
		{
			Debug.Log(input);
		}
		if (postToScreenDuration > 0f && HighLogic.LoadedSceneIsFlight)
		{
			ScreenMessages.PostScreenMessage(new ScreenMessage(input, postToScreenDuration, ScreenMessageStyle.UPPER_RIGHT));
		}
	}
}
