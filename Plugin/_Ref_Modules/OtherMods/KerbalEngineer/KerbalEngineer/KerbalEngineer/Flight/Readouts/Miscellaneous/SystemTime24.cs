using System;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class SystemTime24 : ReadoutModule
{
	public SystemTime24()
	{
		base.Name = "System Time";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Shows the System Time in 24 hour format";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(DateTime.Now.ToString("HH:mm:ss"), section.IsHud);
	}
}
