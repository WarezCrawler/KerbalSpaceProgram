using KerbalEngineer.Editor;
using UnityEngine;

namespace KerbalEngineer.Control.Panels;

public class BuildOverlayPanel : IControlPanel
{
	public string Name => "Build Overlay";

	public void Draw()
	{
		GUILayout.Label("Build Overlay", ControlCentre.Title, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		DrawPartInfo();
		GUILayout.Space(10f);
		DrawDisplays();
	}

	private static void DrawPartInfo()
	{
		GUILayout.Label("Part Information (Hover Tooltips)", ControlCentre.Label, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		BuildOverlayPartInfo.Visible = GUILayout.Toggle(BuildOverlayPartInfo.Visible, "Visible", ControlCentre.Button, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(150f) });
		if (BuildOverlayPartInfo.Visible)
		{
			BuildOverlayPartInfo.NamesOnly = GUILayout.Toggle(BuildOverlayPartInfo.NamesOnly, "Show Names Only", ControlCentre.Button, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(150f) });
			if (!BuildOverlayPartInfo.NamesOnly)
			{
				BuildOverlayPartInfo.ClickToOpen = GUILayout.Toggle(BuildOverlayPartInfo.ClickToOpen, "Click To Open", ControlCentre.Button, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(150f) });
			}
		}
		GUILayout.EndHorizontal();
	}

	private static void DrawDisplays()
	{
		GUILayout.Label("Informational Displays", ControlCentre.Label, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		BuildOverlayVessel.Visible = GUILayout.Toggle(BuildOverlayVessel.Visible, "Vessel Details", ControlCentre.Button, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(150f) });
		BuildOverlayResources.Visible = GUILayout.Toggle(BuildOverlayResources.Visible, "Resources List", ControlCentre.Button, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(150f) });
		GUILayout.EndHorizontal();
	}
}
