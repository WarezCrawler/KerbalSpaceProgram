using System;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class SystemDateTime : ReadoutModule
{
	public SystemDateTime()
	{
		base.Name = "System Time";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Shows the System Date/Time in ISO 8601 format";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(DateTime.Now.ToString("yyyy-MM-dd, HH:mm:ss"), section.IsHud);
	}
}
