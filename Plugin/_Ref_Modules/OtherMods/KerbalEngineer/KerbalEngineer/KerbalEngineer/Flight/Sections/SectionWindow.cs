using KerbalEngineer.Extensions;
using KerbalEngineer.Helpers;
using UnityEngine;

namespace KerbalEngineer.Flight.Sections;

public class SectionWindow : MonoBehaviour
{
	private bool resizeRequested;

	private int windowId;

	private Rect windowPosition;

	private GUIStyle hudWindowBgStyle;

	private GUIStyle hudWindowStyle;

	private GUIStyle windowStyle;

	public SectionModule ParentSection { get; set; }

	public Rect WindowPosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return windowPosition;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			windowPosition = value;
		}
	}

	private void InitialiseStyles()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002f: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0067: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		windowStyle = new GUIStyle(HighLogic.Skin.window)
		{
			margin = new RectOffset(),
			padding = new RectOffset(5, 5, 0, 5)
		};
		GUIStyle val = new GUIStyle(windowStyle);
		val.normal.background = null;
		val.onNormal.background = null;
		val.padding = new RectOffset(5, 5, 0, 8);
		hudWindowStyle = val;
		GUIStyle val2 = new GUIStyle(hudWindowStyle);
		val2.normal.background = TextureHelper.CreateTextureFromColour(new Color(0f, 0f, 0f, 0.5f));
		val2.onNormal.background = TextureHelper.CreateTextureFromColour(new Color(0f, 0f, 0f, 0.5f));
		hudWindowBgStyle = val2;
	}

	private void OnSizeChanged()
	{
		InitialiseStyles();
		RequestResize();
	}

	private void OnGUI()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		if (HighLogic.LoadedSceneIsFlight && ParentSection != null && ParentSection.IsVisible && !((Object)(object)DisplayStack.Instance == (Object)null) && ParentSection != null && (!DisplayStack.Instance.Hidden || ParentSection.IsHud) && FlightEngineerCore.IsDisplayable)
		{
			if (resizeRequested)
			{
				((Rect)(ref windowPosition)).width = 0f;
				((Rect)(ref windowPosition)).height = 0f;
				resizeRequested = false;
			}
			GUI.skin = null;
			windowPosition = GUILayout.Window(windowId, windowPosition, new WindowFunction(Window), string.Empty, (!ParentSection.IsHud || ParentSection.IsEditorVisible) ? windowStyle : ((ParentSection.IsHudBackground && ParentSection.LineCount > 0) ? hudWindowBgStyle : hudWindowStyle), (GUILayoutOption[])(object)new GUILayoutOption[0]);
			windowPosition = (ParentSection.IsHud ? windowPosition.ClampInsideScreen() : windowPosition.ClampToScreen());
			ParentSection.FloatingPositionX = ((Rect)(ref windowPosition)).x;
			ParentSection.FloatingPositionY = ((Rect)(ref windowPosition)).y;
		}
	}

	private void Window(int windowId)
	{
		ParentSection.Draw();
		if (!ParentSection.IsHud || ParentSection.IsEditorVisible)
		{
			GUI.DragWindow();
		}
	}

	private void OnDestroy()
	{
		GuiDisplaySize.OnSizeChanged -= OnSizeChanged;
	}

	public void RequestResize()
	{
		resizeRequested = true;
	}

	private void Start()
	{
		windowId = ((object)this).GetHashCode();
		InitialiseStyles();
		GuiDisplaySize.OnSizeChanged += OnSizeChanged;
	}
}
