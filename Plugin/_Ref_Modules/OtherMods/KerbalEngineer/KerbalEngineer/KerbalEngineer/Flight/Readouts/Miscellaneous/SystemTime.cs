using System;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class SystemTime : ReadoutModule
{
	public SystemTime()
	{
		base.Name = "System Time";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Shows the System Time in 12 hour format (AM/PM)";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(DateTime.Now.ToString("h:mm:ss tt"), section.IsHud);
	}
}
