using System;
using KerbalEngineer.Unity.Flight;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class SimulationDelay : ReadoutModule
{
	public SimulationDelay()
	{
		base.Name = "Minimum Simulation Delay";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Controls the minimum delay between processing vessel simulations.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Sim Delay", base.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUI.skin = HighLogic.Skin;
		SimManager.minSimTime = TimeSpan.FromMilliseconds(GUILayout.HorizontalSlider((float)SimManager.minSimTime.TotalMilliseconds, 0f, 2000f, (GUILayoutOption[])(object)new GUILayoutOption[0]));
		GUI.skin = null;
		GUILayout.EndHorizontal();
	}
}
