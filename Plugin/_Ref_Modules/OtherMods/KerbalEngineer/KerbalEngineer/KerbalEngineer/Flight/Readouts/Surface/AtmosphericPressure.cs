using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class AtmosphericPressure : ReadoutModule
{
	public AtmosphericPressure()
	{
		base.Name = "Atmos. Pressure";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Displays the current atmospheric pressure.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (AtmosphericProcessor.ShowDetails)
		{
			DrawLine(AtmosphericProcessor.StaticPressure.ToPressure(), section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(AtmosphericProcessor.Instance);
	}

	public override void Update()
	{
		AtmosphericProcessor.RequestUpdate();
	}
}
