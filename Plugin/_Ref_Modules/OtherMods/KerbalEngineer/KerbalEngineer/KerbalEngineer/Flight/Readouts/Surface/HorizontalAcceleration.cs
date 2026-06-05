using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class HorizontalAcceleration : ReadoutModule
{
	private double acceleration;

	private double speed;

	public HorizontalAcceleration()
	{
		base.Name = "Horizontal Acceleration";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's horizontal acceleration across a celestial body's surface.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(acceleration.ToAcceleration(), section.IsHud);
	}

	public override void FixedUpdate()
	{
		acceleration = (FlightGlobals.ActiveVessel.horizontalSrfSpeed - speed) / (double)TimeWarp.fixedDeltaTime;
		speed = FlightGlobals.ActiveVessel.horizontalSrfSpeed;
	}
}
