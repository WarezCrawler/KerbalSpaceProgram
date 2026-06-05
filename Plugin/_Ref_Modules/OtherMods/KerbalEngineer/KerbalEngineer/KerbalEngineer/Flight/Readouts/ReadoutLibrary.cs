using System;
using System.Collections.Generic;
using System.Linq;
using KerbalEngineer.Flight.Readouts.Body;
using KerbalEngineer.Flight.Readouts.Miscellaneous;
using KerbalEngineer.Flight.Readouts.Orbital;
using KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;
using KerbalEngineer.Flight.Readouts.Rendezvous;
using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Flight.Readouts.Thermal;
using KerbalEngineer.Flight.Readouts.Vessel;
using KerbalEngineer.Settings;

namespace KerbalEngineer.Flight.Readouts;

public static class ReadoutLibrary
{
	private static List<ReadoutModule> readouts;

	public static List<ReadoutModule> Readouts
	{
		get
		{
			return readouts;
		}
		set
		{
			readouts = value;
		}
	}

	static ReadoutLibrary()
	{
		readouts = new List<ReadoutModule>();
		try
		{
			ReadoutCategory.SetCategory("Orbital", "Readout for orbital manovoeures.");
			ReadoutCategory.SetCategory("Surface", "Surface and atmospheric readouts.");
			ReadoutCategory.SetCategory("Vessel", "Vessel performance statistics.");
			ReadoutCategory.SetCategory("Rendezvous", "Readouts for rendezvous manovoeures.");
			ReadoutCategory.SetCategory("Thermal", "Thermal characteristics readouts.");
			ReadoutCategory.SetCategory("Body", "Characteristics of the current SOI.");
			ReadoutCategory.SetCategory("Miscellaneous", "Miscellaneous readouts.");
			ReadoutCategory.Selected = ReadoutCategory.GetCategory("Orbital");
			readouts.Add(new KerbalEngineer.Flight.Readouts.Orbital.ApoapsisHeight());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Orbital.PeriapsisHeight());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Orbital.TimeToApoapsis());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Orbital.TimeToPeriapsis());
			readouts.Add(new Inclination());
			readouts.Add(new TimeToEquatorialAscendingNode());
			readouts.Add(new TimeToEquatorialDescendingNode());
			readouts.Add(new AngleToEquatorialAscendingNode());
			readouts.Add(new AngleToEquatorialDescendingNode());
			readouts.Add(new Eccentricity());
			readouts.Add(new OrbitalSpeed());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Orbital.OrbitalPeriod());
			readouts.Add(new LongitudeOfAscendingNode());
			readouts.Add(new LongitudeOfPeriapsis());
			readouts.Add(new ArgumentOfPeriapsis());
			readouts.Add(new TrueAnomaly());
			readouts.Add(new MeanAnomaly());
			readouts.Add(new MeanAnomalyAtEpoc());
			readouts.Add(new EccentricAnomaly());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Orbital.SemiMajorAxis());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Orbital.SemiMinorAxis());
			readouts.Add(new AngleToPrograde());
			readouts.Add(new AngleToRetrograde());
			readouts.Add(new NodeProgradeDeltaV());
			readouts.Add(new NodeNormalDeltaV());
			readouts.Add(new NodeRadialDeltaV());
			readouts.Add(new NodeTotalDeltaV());
			readouts.Add(new NodeBurnTime());
			readouts.Add(new NodeHalfBurnTime());
			readouts.Add(new NodeTimeToManoeuvre());
			readouts.Add(new NodeTimeToHalfBurn());
			readouts.Add(new NodeAngleToPrograde());
			readouts.Add(new NodeAngleToRetrograde());
			readouts.Add(new PostBurnApoapsis());
			readouts.Add(new PostBurnPeriapsis());
			readouts.Add(new PostBurnInclination());
			readouts.Add(new PostBurnRealtiveInclination());
			readouts.Add(new PostBurnPeriod());
			readouts.Add(new PostBurnEccentricity());
			readouts.Add(new SpeedAtApoapsis());
			readouts.Add(new SpeedAtPeriapsis());
			readouts.Add(new TimeToAtmosphere());
			readouts.Add(new TripTotalDeltaV());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Surface.AltitudeSeaLevel());
			readouts.Add(new AltitudeTerrain());
			readouts.Add(new VerticalSpeed());
			readouts.Add(new VerticalAcceleration());
			readouts.Add(new HorizontalSpeed());
			readouts.Add(new HorizontalAcceleration());
			readouts.Add(new MachNumber());
			readouts.Add(new Latitude());
			readouts.Add(new Longitude());
			readouts.Add(new GeeForce());
			readouts.Add(new TerminalVelocity());
			readouts.Add(new AtmosphericEfficiency());
			readouts.Add(new AtmosphericPressure());
			readouts.Add(new Biome());
			readouts.Add(new Situation());
			readouts.Add(new Slope());
			readouts.Add(new ImpactTime());
			readouts.Add(new ImpactLatitude());
			readouts.Add(new ImpactLongitude());
			readouts.Add(new ImpactMarker());
			readouts.Add(new ImpactAltitude());
			readouts.Add(new ImpactBiome());
			readouts.Add(new Name());
			readouts.Add(new DeltaVStaged());
			readouts.Add(new DeltaVCurrent());
			readouts.Add(new DeltaVTotal());
			readouts.Add(new DeltaVCurrentTotal());
			readouts.Add(new SpecificImpulse());
			readouts.Add(new Mass());
			readouts.Add(new Thrust());
			readouts.Add(new ThrustToWeight());
			readouts.Add(new ThrustOffsetAngle());
			readouts.Add(new ThrustTorque());
			readouts.Add(new SurfaceThrustToWeight());
			readouts.Add(new Gravity());
			readouts.Add(new Acceleration());
			readouts.Add(new SuicideBurnAltitude());
			readouts.Add(new SuicideBurnDistance());
			readouts.Add(new SuicideBurnDeltaV());
			readouts.Add(new SuicideBurnCountdown());
			readouts.Add(new SuicideBurnLength());
			readouts.Add(new IntakeAirUsage());
			readouts.Add(new IntakeAirDemand());
			readouts.Add(new IntakeAirSupply());
			readouts.Add(new IntakeAirDemandSupply());
			readouts.Add(new PartCount());
			readouts.Add(new Throttle());
			readouts.Add(new Heading());
			readouts.Add(new Pitch());
			readouts.Add(new Roll());
			readouts.Add(new HeadingRate());
			readouts.Add(new PitchRate());
			readouts.Add(new RollRate());
			readouts.Add(new RCSDeltaV());
			readouts.Add(new RCSIsp());
			readouts.Add(new RCSThrust());
			readouts.Add(new RCSTWR());
			readouts.Add(new TargetSelector());
			readouts.Add(new PhaseAngle());
			readouts.Add(new InterceptAngle());
			readouts.Add(new TimeToTransferAngleTime());
			readouts.Add(new RelativeVelocity());
			readouts.Add(new RelativeSpeed());
			readouts.Add(new RelativeInclination());
			readouts.Add(new TimeToRelativeAscendingNode());
			readouts.Add(new TimeToRelativeDescendingNode());
			readouts.Add(new AngleToRelativeAscendingNode());
			readouts.Add(new AngleToRelativeDescendingNode());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Rendezvous.AltitudeSeaLevel());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Rendezvous.ApoapsisHeight());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Rendezvous.PeriapsisHeight());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Rendezvous.TimeToApoapsis());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Rendezvous.TimeToPeriapsis());
			readouts.Add(new Distance());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Rendezvous.OrbitalPeriod());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Rendezvous.SemiMajorAxis());
			readouts.Add(new KerbalEngineer.Flight.Readouts.Rendezvous.SemiMinorAxis());
			readouts.Add(new TimeTilClosestApproach());
			readouts.Add(new SeparationAtClosestApproach());
			readouts.Add(new SpeedAtClosestApproach());
			readouts.Add(new TargetLatitude());
			readouts.Add(new TargetLongitude());
			readouts.Add(new InternalFlux());
			readouts.Add(new ConvectionFlux());
			readouts.Add(new RadiationFlux());
			readouts.Add(new CriticalPart());
			readouts.Add(new CriticalTemperature());
			readouts.Add(new CriticalSkinTemperature());
			readouts.Add(new CriticalThermalPercentage());
			readouts.Add(new HottestPart());
			readouts.Add(new HottestTemperature());
			readouts.Add(new HottestSkinTemperature());
			readouts.Add(new CoolestPart());
			readouts.Add(new CoolestTemperature());
			readouts.Add(new CoolestSkinTemperature());
			readouts.Add(new BodyName());
			readouts.Add(new HasAtmosphere());
			readouts.Add(new HasOxygen());
			readouts.Add(new MinOrbitHeight());
			readouts.Add(new HighAtmosphereHeight());
			readouts.Add(new LowSpaceHeight());
			readouts.Add(new HighSpaceHeight());
			readouts.Add(new GeostationaryHeight());
			readouts.Add(new CurrentSoi());
			readouts.Add(new BodyRotationPeriod());
			readouts.Add(new BodyOrbitalPeriod());
			readouts.Add(new EscapeVelocity());
			readouts.Add(new BodyMass());
			readouts.Add(new BodyRadius());
			readouts.Add(new BodyGravity());
			readouts.Add(new Separator());
			readouts.Add(new ClearSeparator());
			readouts.Add(new GuiSizeAdjustor());
			readouts.Add(new SimulationDelay());
			readouts.Add(new VectoredThrustToggle());
			readouts.Add(new SystemTime());
			readouts.Add(new SystemTime24());
			readouts.Add(new SystemDateTime());
			readouts.Add(new LogSimToggle());
			LoadHelpStrings();
			LoadReadoutConfig();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	public static List<ReadoutModule> GetCategory(ReadoutCategory category)
	{
		return readouts.Where((ReadoutModule r) => r.Category == category).ToList();
	}

	public static ReadoutModule GetReadout(string name)
	{
		return readouts.FirstOrDefault((ReadoutModule r) => r.Name == name || r.GetType().Name == name || string.Concat(r.Category, ".", r.GetType().Name) == name);
	}

	public static void Reset()
	{
		foreach (ReadoutModule readout in readouts)
		{
			readout.Reset();
		}
	}

	private static void LoadHelpStrings()
	{
		try
		{
			SettingHandler settingHandler = SettingHandler.Load("HelpStrings.xml");
			foreach (ReadoutModule readout in readouts)
			{
				readout.HelpString = settingHandler.GetSet(string.Concat(readout.Category, ".", readout.GetType().Name), readout.HelpString);
			}
			settingHandler.Save("HelpStrings.xml");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private static void LoadReadoutConfig()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SettingHandler settingHandler = SettingHandler.Load("ReadoutsConfig.xml", new Type[1] { typeof(ReadoutModuleConfigNode) });
			foreach (ReadoutModule readout in readouts)
			{
				ReadoutModuleConfigNode readoutModuleConfigNode = settingHandler.Get<ReadoutModuleConfigNode>(readout.Name, null);
				if (readoutModuleConfigNode != null)
				{
					readout.ValueStyle.normal.textColor = readoutModuleConfigNode.Color;
				}
			}
			settingHandler.Save("ReadoutsConfig.xml");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	public static void RemoveReadoutConfig(ReadoutModule readout)
	{
		try
		{
			SettingHandler settingHandler = SettingHandler.Load("ReadoutsConfig.xml", new Type[1] { typeof(ReadoutModuleConfigNode) });
			if (settingHandler.Get<ReadoutModuleConfigNode>(readout.Name, null) != null)
			{
				settingHandler.Items.Remove(settingHandler.Items.Find((SettingItem i) => i.Name == readout.Name));
				settingHandler.Save("ReadoutsConfig.xml");
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	public static void SaveReadoutConfig(ReadoutModule readout)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SettingHandler settingHandler = SettingHandler.Load("ReadoutsConfig.xml", new Type[1] { typeof(ReadoutModuleConfigNode) });
			ReadoutModuleConfigNode readoutModuleConfigNode = settingHandler.Get<ReadoutModuleConfigNode>(readout.Name, null);
			if (readoutModuleConfigNode == null)
			{
				readoutModuleConfigNode = new ReadoutModuleConfigNode();
			}
			readoutModuleConfigNode.Name = readout.Name;
			readoutModuleConfigNode.Color = readout.ValueStyle.normal.textColor;
			settingHandler.Set(readoutModuleConfigNode.Name, readoutModuleConfigNode);
			settingHandler.Save("ReadoutsConfig.xml");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
