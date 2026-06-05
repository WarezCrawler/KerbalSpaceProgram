using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class DynamicPressure : ReadoutModule
{
	public DynamicPressure()
	{
		base.Name = "Dynamic Pressure";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Displays the current dynamic pressure on the vessel";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (AtmosphericProcessor.ShowDetails)
		{
			DrawLine(AtmosphericProcessor.DynamicPressure.ToPressure(), section.IsHud);
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
