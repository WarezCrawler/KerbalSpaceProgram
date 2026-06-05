using KerbalEngineer.VesselSimulator;

namespace KerbalEngineer.Flight;

public class FlightEngineerModule : PartModule
{
	[KSPEvent(guiName = "Verbose Simulation Log", guiActive = true, guiActiveEditor = true)]
	public void SimulationDump()
	{
		SimManager.logOutput = true;
	}
}
