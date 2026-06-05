using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class VerticalAcceleration : ReadoutModule
{
	private double acceleration;

	private double speed;

	public VerticalAcceleration()
	{
		base.Name = "Vertical Acceleration";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's vertical acceleration up and down.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(acceleration.ToAcceleration(), section.IsHud);
	}

	public override void FixedUpdate()
	{
		acceleration = (FlightGlobals.ship_verticalSpeed - speed) / (double)TimeWarp.fixedDeltaTime;
		speed = FlightGlobals.ship_verticalSpeed;
	}
}
