using System;
using UnityEngine;

namespace InterstellarFuelSwitch;

public class IFSinfoPopup : PartModule
{
	[KSPField(isPersistant = true)]
	public string textHeading = "Part Info";

	[KSPField(isPersistant = true)]
	public string textBody1 = "";

	[KSPField(isPersistant = true)]
	public string textBody2 = "";

	[KSPField(isPersistant = true)]
	public string textBody3 = "";

	[KSPField(isPersistant = true)]
	public string textBody4 = "";

	[KSPField(isPersistant = true)]
	public string textBody5 = "";

	[KSPField(isPersistant = true)]
	public string textBody6 = "";

	[KSPField(isPersistant = true)]
	public string textBody7 = "";

	[KSPField(isPersistant = true)]
	public string textBody8 = "";

	[KSPField(isPersistant = true)]
	public string textBody9 = "";

	[KSPField(isPersistant = true)]
	public string textBody10 = "";

	[KSPField(isPersistant = true)]
	public string textBody11 = "";

	[KSPField(isPersistant = true)]
	public int positionX;

	[KSPField(isPersistant = true)]
	public int positionY;

	[KSPField(isPersistant = true)]
	public bool showAtFlightStart = true;

	[KSPField(isPersistant = true)]
	public bool hideAfterCountdown = true;

	[KSPField(isPersistant = true)]
	public bool showOnEachFlightStart;

	[KSPField(isPersistant = true)]
	public bool hasBeenShown;

	[KSPField]
	public float countDownDuration = 20f;

	[KSPField]
	public string toggleKey = "i";

	[KSPField(isPersistant = true)]
	public bool useHotkey = true;

	private float countDown;

	private bool showInfo;

	private bool shownByUser;

	private bool editMode;

	private float oldTime;

	private string windowTitle;

	private int editorButtonCooldown;

	private int windowID;

	private static System.Random randomgenerator;

	private Vector2 menuItemPosition = new Vector2(0f, 0f);

	private Vector2 menuItemSize = new Vector2(300f, 22f);

	private Vector2 buttonSize = new Vector2(25f, 20f);

	private Rect windowRect = new Rect(300f, 300f, 320f, 320f);

	[KSPEvent(name = "showInfo", active = true, guiActive = true, guiActiveEditor = true, guiName = "Show Info", guiActiveUncommand = true, guiActiveUnfocused = true)]
	public void showInfoEvent()
	{
		if (showInfo)
		{
			showInfo = false;
			editMode = false;
		}
		else
		{
			showInfo = true;
		}
		shownByUser = true;
	}

	[KSPAction("Show Info")]
	public void showInfoAction(KSPActionParam param)
	{
		if (showInfo)
		{
			showInfo = false;
			editMode = false;
		}
		else
		{
			showInfo = true;
		}
		shownByUser = true;
	}

	private string writeLine(Rect rect, string text)
	{
		if (editMode)
		{
			return GUI.TextField(rect, text);
		}
		GUI.Label(rect, text);
		return text;
	}

	private static int GetRandom(int range = int.MaxValue)
	{
		if (randomgenerator == null)
		{
			randomgenerator = new System.Random();
		}
		return randomgenerator.Next(range);
	}

	public override void OnStart(StartState state)
	{
		base.OnStart(state);
		windowID = GetRandom();
		if (positionX == 0)
		{
			positionX = GetRandom(600);
		}
		if (positionY == 0)
		{
			positionY = GetRandom(1000);
		}
		windowRect = new Rect(positionX, positionY, 320f, 320f);
		if (!HighLogic.LoadedSceneIsFlight || !base.vessel.isActiveVessel)
		{
			showInfo = false;
			return;
		}
		showInfo = false;
		if (showAtFlightStart && showOnEachFlightStart)
		{
			showInfo = true;
		}
		if (showAtFlightStart && !showOnEachFlightStart && !hasBeenShown)
		{
			showInfo = true;
			showAtFlightStart = false;
			hasBeenShown = true;
		}
		oldTime = Time.time;
		countDown = countDownDuration;
	}

	private void drawWindow(int WindowID)
	{
		Rect rect = new Rect(10f, 8f, menuItemSize.x, menuItemSize.y);
		if (GUI.Button(new Rect(rect.x + menuItemSize.x - buttonSize.x, rect.y, buttonSize.x, buttonSize.y), "X"))
		{
			showInfo = false;
			editMode = false;
		}
		if (editMode)
		{
			windowTitle = "";
			textHeading = writeLine(new Rect(rect.x, rect.y, rect.width - buttonSize.x - 10f, rect.height), textHeading);
		}
		else
		{
			windowTitle = textHeading;
		}
		rect.y += menuItemSize.y;
		textBody1 = writeLine(rect, textBody1);
		rect.y += menuItemSize.y;
		textBody2 = writeLine(rect, textBody2);
		rect.y += menuItemSize.y;
		textBody3 = writeLine(rect, textBody3);
		rect.y += menuItemSize.y;
		textBody4 = writeLine(rect, textBody4);
		rect.y += menuItemSize.y;
		textBody5 = writeLine(rect, textBody5);
		rect.y += menuItemSize.y;
		textBody6 = writeLine(rect, textBody6);
		rect.y += menuItemSize.y;
		textBody7 = writeLine(rect, textBody7);
		rect.y += menuItemSize.y;
		textBody8 = writeLine(rect, textBody8);
		rect.y += menuItemSize.y;
		textBody9 = writeLine(rect, textBody9);
		rect.y += menuItemSize.y;
		textBody10 = writeLine(rect, textBody10);
		rect.y += menuItemSize.y;
		textBody11 = writeLine(rect, textBody11);
		rect.y += menuItemSize.y;
		if (GUI.Button(text: (!showAtFlightStart) ? "N" : "Y", position: new Rect(rect.x, rect.y, buttonSize.x, buttonSize.y)))
		{
			showAtFlightStart = !showAtFlightStart;
			if (showAtFlightStart)
			{
				showOnEachFlightStart = true;
			}
		}
		GUI.Label(new Rect(rect.x + buttonSize.x + 10f, rect.y, menuItemSize.x - buttonSize.x - 10f, buttonSize.y), "Show on start");
		if (GUI.Button(text: (!useHotkey) ? "N" : "Y", position: new Rect(rect.x + menuItemSize.x / 2f, rect.y, buttonSize.x, buttonSize.y)))
		{
			useHotkey = !useHotkey;
		}
		GUI.Label(new Rect(rect.x + buttonSize.x + 10f + menuItemSize.x / 2f, rect.y, menuItemSize.x - buttonSize.x - 10f, buttonSize.y), "Use hotkey (" + toggleKey + ")");
		rect.y += menuItemSize.y;
		if (GUI.Button(new Rect(rect.x, rect.y, buttonSize.x * 2f, buttonSize.y), "Edit"))
		{
			editMode = !editMode;
			shownByUser = true;
		}
		if (!shownByUser)
		{
			GUI.Label(new Rect(rect.x + buttonSize.x * 2f + 20f, rect.y, rect.width, rect.height), "Hiding this window in " + (int)countDown);
		}
		GUI.DragWindow();
	}

	public void OnGUI()
	{
		if (showInfo)
		{
			float num = Time.time - oldTime;
			oldTime = Time.time;
			if (countDown > 0f)
			{
				countDown -= num;
			}
			if (countDown <= 0f && !shownByUser && hideAfterCountdown)
			{
				showInfo = false;
			}
			windowRect = GUI.Window(windowID, windowRect, drawWindow, windowTitle);
		}
		if (!HighLogic.LoadedSceneIsEditor)
		{
			return;
		}
		EditorLogic fetch = EditorLogic.fetch;
		if ((bool)fetch)
		{
			if (editorButtonCooldown > 0)
			{
				editorButtonCooldown--;
			}
			if (Input.GetKeyDown(toggleKey) && editorButtonCooldown <= 0)
			{
				showInfo = !showInfo;
				shownByUser = true;
				editorButtonCooldown = 20;
			}
			shownByUser = true;
		}
	}

	public void Update()
	{
		positionX = (int)windowRect.x;
		positionY = (int)windowRect.y;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (!HighLogic.LoadedSceneIsFlight || !base.vessel.isActiveVessel)
		{
			showInfo = false;
		}
		else if (Input.GetKeyDown(toggleKey) && useHotkey)
		{
			showInfo = !showInfo;
			shownByUser = true;
		}
	}
}
