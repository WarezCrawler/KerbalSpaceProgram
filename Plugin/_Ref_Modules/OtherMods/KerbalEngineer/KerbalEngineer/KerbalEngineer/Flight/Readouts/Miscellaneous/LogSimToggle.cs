using KerbalEngineer.Unity.Flight;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class LogSimToggle : ReadoutModule
{
	public LogSimToggle()
	{
		base.Name = "Log Simulation";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Shows a button that allows you to make the next run of the simulation code dump extra debugging output.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Log Simulation: ", base.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		SimManager.logOutput = GUILayout.Toggle(SimManager.logOutput, "ENABLED", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
	}
}
