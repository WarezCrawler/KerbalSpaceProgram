using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class ImpactMarker : ReadoutModule
{
	private bool show = true;

	public ImpactMarker()
	{
		base.Name = "Impact Marker";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows your estimated impact position on the surface and the map.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (!ImpactProcessor.ShowDetails)
		{
			return;
		}
		DrawLine(delegate
		{
			GUIStyle val = (section.IsHud ? base.CompactButtonStyle : base.ButtonStyle);
			if (GUILayout.Button(ImpactProcessor.ShowMarker ? "Hide" : "Show", val, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(base.ContentWidth / 4f),
				GUILayout.Height(val.fixedHeight)
			}))
			{
				show = !show;
			}
		}, showName: true, section.IsHud);
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(ImpactProcessor.Instance);
	}

	public override void Update()
	{
		if (show)
		{
			FlightEngineerCore.markerDeadman = 2;
		}
		ImpactProcessor.RequestUpdate();
	}
}
