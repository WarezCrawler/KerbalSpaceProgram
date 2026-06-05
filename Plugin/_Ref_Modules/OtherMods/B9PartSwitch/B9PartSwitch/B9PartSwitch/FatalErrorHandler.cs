using System;
using System.Collections.Generic;
using UnityEngine;

namespace B9PartSwitch;

public static class FatalErrorHandler
{
	private const int MAX_MESSAGE_COUNT = 10;

	private static PopupDialog dialog;

	private static List<string> allMessages = new List<string>();

	public static void HandleFatalError(Exception exception)
	{
		try
		{
			string text = exception.Message;
			for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
			{
				text += "\n  ";
				text += innerException.Message;
			}
			UpsertDialog(text);
		}
		catch (Exception exception2)
		{
			Debug.LogError("Exception while trying to create the fatal exception dialog");
			Debug.LogException(exception2);
			Application.Quit();
		}
	}

	private static void UpsertDialog(string message)
	{
		if (string.IsNullOrEmpty(message) || allMessages.Contains(message))
		{
			return;
		}
		if (allMessages.Count < 10)
		{
			allMessages.Add(message);
		}
		else
		{
			if (allMessages.Count != 10)
			{
				Debug.LogError("[FatalExceptionHandler] Not displaying fatal error because too many errors have already been added:");
				Debug.LogError(message);
				return;
			}
			Debug.LogError("[FatalExceptionHandler] Not displaying fatal error because too many errors have already been added:");
			Debug.LogError(message);
			allMessages.Add("(too many error messages to display)");
		}
		if (dialog != null)
		{
			dialog.Dismiss();
		}
		dialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("B9PartSwitchFatalError", "B9PartSwitch has encountered a fatal error and KSP needs to close.\n\n" + string.Join("\n\n", allMessages.ToArray()) + "\n\nPlease see KSP's log for additional details", "B9PartSwitch - Fatal Error", HighLogic.UISkin, new Rect(0.5f, 0.5f, 500f, 60f), new DialogGUIFlexibleSpace(), new DialogGUIHorizontalLayout(new DialogGUIBase[3]
		{
			new DialogGUIFlexibleSpace(),
			new DialogGUIButton("Quit", Application.Quit, 140f, 30f, true),
			new DialogGUIFlexibleSpace()
		})), persistAcrossScenes: true, HighLogic.UISkin);
	}
}
