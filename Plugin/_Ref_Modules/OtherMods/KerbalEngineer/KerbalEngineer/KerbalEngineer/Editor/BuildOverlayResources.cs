using System;
using System.Collections.Generic;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.Editor;

public class BuildOverlayResources : MonoBehaviour
{
	private static bool visible = true;

	private readonly Dictionary<int, ResourceInfoItem> resources = new Dictionary<int, ResourceInfoItem>();

	private bool open = true;

	private float openPercent;

	private GUIContent tabContent;

	private Rect tabPosition;

	private Vector2 tabSize;

	private Rect windowPosition = new Rect(0f, 0f, BuildOverlay.MinimumWidth, 0f);

	private static Part part;

	private static PartResource partResource;

	public static bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}

	public bool Open
	{
		get
		{
			return open;
		}
		set
		{
			open = value;
		}
	}

	protected void OnGUI()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Visible && resources.Count != 0)
			{
				open = GUI.Toggle(tabPosition, open, tabContent, BuildOverlay.TabStyle);
				if ((double)openPercent > 0.0)
				{
					windowPosition = GUILayout.Window(((Object)this).GetInstanceID(), windowPosition, new WindowFunction(Window), string.Empty, BuildOverlay.WindowStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				}
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void Start()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			tabContent = new GUIContent("RESOURCES");
			tabSize = BuildOverlay.TabStyle.CalcSize(tabContent);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void Update()
	{
		try
		{
			if (Visible)
			{
				SetResources();
				SetSlidePosition();
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void SetResources()
	{
		int count = resources.Count;
		resources.Clear();
		for (int i = 0; i < EditorLogic.fetch.ship.parts.Count; i++)
		{
			part = EditorLogic.fetch.ship.parts[i];
			for (int j = 0; j < part.Resources.dict.Count; j++)
			{
				partResource = part.Resources.dict.At(j);
				if (resources.ContainsKey(partResource.info.id))
				{
					resources[partResource.info.id].Amount += partResource.amount;
				}
				else
				{
					resources.Add(partResource.info.id, new ResourceInfoItem(partResource));
				}
			}
		}
		if (resources.Count < count)
		{
			((Rect)(ref windowPosition)).height = 0f;
		}
	}

	private void SetSlidePosition()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (open && openPercent < 1f)
		{
			openPercent = Mathf.Clamp(openPercent + Time.deltaTime * BuildOverlay.TabSpeed, 0f, 1f);
		}
		else if (!open && openPercent > 0f)
		{
			openPercent = Mathf.Clamp(openPercent - Time.deltaTime * BuildOverlay.TabSpeed, 0f, 1f);
		}
		ref Rect reference = ref windowPosition;
		Rect val = BuildOverlay.BuildOverlayVessel.WindowPosition;
		((Rect)(ref reference)).x = ((Rect)(ref val)).xMax + 5f;
		((Rect)(ref windowPosition)).y = Mathf.Lerp((float)Screen.height, (float)Screen.height - ((Rect)(ref windowPosition)).height, openPercent);
		((Rect)(ref tabPosition)).width = tabSize.x;
		((Rect)(ref tabPosition)).height = tabSize.y;
		((Rect)(ref tabPosition)).x = ((Rect)(ref windowPosition)).x;
		((Rect)(ref tabPosition)).y = ((Rect)(ref windowPosition)).y - ((Rect)(ref tabPosition)).height;
	}

	private void Window(int windowId)
	{
		try
		{
			bool flag = true;
			foreach (KeyValuePair<int, ResourceInfoItem> resource in resources)
			{
				if (!flag)
				{
					GUILayout.Space(2f);
				}
				flag = false;
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUILayout.Label(resource.Value.Name + ":", BuildOverlay.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				GUILayout.Space(50f);
				if (resource.Value.Mass > 0.0)
				{
					GUILayout.Label("(" + resource.Value.Mass.ToMass() + ") " + resource.Value.Amount.ToString("N1"), BuildOverlay.ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Label(resource.Value.Amount.ToString("N1"), BuildOverlay.ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				}
				GUILayout.EndHorizontal();
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
