using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class ImpactTime : ReadoutModule
{
	public ImpactTime()
	{
		base.Name = "Impact Time";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows time until the vessel impacts the central object.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ImpactProcessor.ShowDetails)
		{
			DrawLine(TimeFormatter.ConvertToString(ImpactProcessor.Time), section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(ImpactProcessor.Instance);
	}

	public override void Update()
	{
		ImpactProcessor.RequestUpdate();
	}
}
