using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class GeeForce : ReadoutModule
{
	private double maxGeeForce;

	public GeeForce()
	{
		base.Name = "G-Force";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the current g-force and maximum g-force experienced.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (FlightGlobals.ship_geeForce > maxGeeForce)
		{
			maxGeeForce = FlightGlobals.ship_geeForce;
		}
		DrawLine(delegate
		{
			if (!section.IsHud)
			{
				GUILayout.Label(FlightGlobals.ship_geeForce.ToString("F3") + " / " + maxGeeForce.ToString("F3"), base.ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
			else
			{
				GUILayout.Label(FlightGlobals.ship_geeForce.ToString("F3") + " / " + maxGeeForce.ToString("F3"), base.ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height((float)base.ValueStyle.fontSize * 1.2f) });
			}
			if (GUILayout.Button("R", section.IsHud ? base.CompactButtonStyle : base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ButtonStyle.fixedHeight) }))
			{
				maxGeeForce = 0.0;
			}
		}, showName: true, section.IsHud);
	}

	public override void Reset()
	{
		maxGeeForce = 0.0;
	}
}
