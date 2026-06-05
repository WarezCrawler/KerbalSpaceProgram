using System;
using System.Linq;
using System.Reflection;
using KerbalEngineer.Extensions;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class AtmosphericProcessor : IUpdatable, IUpdateRequest
{
	private static readonly AtmosphericProcessor instance = new AtmosphericProcessor();

	private MethodInfo farTerminalVelocity;

	private bool hasCheckedAeroMods;

	public static AtmosphericProcessor Instance => instance;

	public static double Deceleration { get; private set; }

	public static double Efficiency { get; private set; }

	public static bool FarInstalled { get; private set; }

	public static bool NearInstalled { get; private set; }

	public static bool ShowDetails { get; private set; }

	public static double TerminalVelocity { get; private set; }

	public static double StaticPressure { get; private set; }

	public static double DynamicPressure { get; private set; }

	public bool UpdateRequested { get; set; }

	public void Update()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!hasCheckedAeroMods)
			{
				CheckAeroMods();
			}
			if (FlightGlobals.ActiveVessel.atmDensity < double.Epsilon || NearInstalled)
			{
				ShowDetails = false;
				return;
			}
			ShowDetails = true;
			if (FarInstalled)
			{
				TerminalVelocity = (double)farTerminalVelocity.Invoke(null, null);
			}
			else
			{
				double num = FlightGlobals.ActiveVessel.parts.Sum((Part part) => part.GetWetMass()) * 1000.0;
				Vector3d geeForceAtPosition = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position);
				double magnitude = ((Vector3d)(ref geeForceAtPosition)).magnitude;
				float num2 = FlightGlobals.ActiveVessel.parts.Sum((Part part) => part.DragCubes.AreaDrag) * PhysicsGlobals.DragCubeMultiplier;
				double atmDensity = FlightGlobals.ActiveVessel.atmDensity;
				float dragMultiplier = PhysicsGlobals.DragMultiplier;
				TerminalVelocity = Math.Sqrt(2.0 * num * magnitude / (atmDensity * (double)num2 * (double)dragMultiplier));
				StaticPressure = FlightGlobals.ActiveVessel.staticPressurekPa;
				DynamicPressure = FlightGlobals.ActiveVessel.dynamicPressurekPa;
			}
			Efficiency = FlightGlobals.ship_srfSpeed / TerminalVelocity;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "AtmosphericProcessor->Update");
		}
	}

	public static void RequestUpdate()
	{
		instance.UpdateRequested = true;
	}

	private void CheckAeroMods()
	{
		try
		{
			hasCheckedAeroMods = true;
			foreach (LoadedAssembly loadedAssembly in AssemblyLoader.loadedAssemblies)
			{
				string name = loadedAssembly.name;
				if (!(name == "FerramAerospaceResearch"))
				{
					if (name == "NEAR")
					{
						NearInstalled = true;
						MyLogger.Log("NEAR detected! Turning off atmospheric details!");
					}
				}
				else
				{
					farTerminalVelocity = loadedAssembly.assembly.GetType("ferram4.FARAPI").GetMethod("GetActiveControlSys_TermVel");
					FarInstalled = true;
					MyLogger.Log("FAR detected!");
				}
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "AtmosphericProcessor->CheckAeroMods");
		}
	}
}
