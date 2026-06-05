using KerbalEngineer.Unity.Flight;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class VectoredThrustToggle : ReadoutModule
{
	public VectoredThrustToggle()
	{
		base.Name = "Vectored Thrust";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Shows a control that will allow you to adjust whether the vessel simulation should account for vectored thrust.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Vectored Thrust: ", base.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		SimManager.vectoredThrust = GUILayout.Toggle(SimManager.vectoredThrust, "ENABLED", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
	}
}
