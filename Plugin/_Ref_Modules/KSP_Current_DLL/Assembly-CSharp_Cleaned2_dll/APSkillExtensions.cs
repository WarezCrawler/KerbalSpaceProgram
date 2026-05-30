using System;

public static class APSkillExtensions
{
	public static bool AvailableAtLevel(this VesselAutopilot.AutopilotMode mode, Vessel vessel)
	{
		VesselValues vesselValues = vessel.VesselValues;
		if (HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().KerbalExperienceEnabled(HighLogic.CurrentGame.Mode))
		{
			return vesselValues.AutopilotSkill.value >= mode.GetRequiredSkill();
		}
		if ((HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX) && !HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().EnableFullSASInSandbox)
		{
			if (vesselValues.AutopilotKerbalSkill.value <= -1 && vesselValues.RepairSkill.value <= -1 && vesselValues.ScienceSkill.value <= -1)
			{
				return vesselValues.AutopilotSASSkill.value >= mode.GetRequiredSkill();
			}
			return true;
		}
		return true;
	}

	[Obsolete("\n\nUse AvailableAtLevel(Vessel vessel) instead.\nAutopilot level now caters to Kerbals and SASModules with more depth")]
	public static bool AvailableAtLevel(this VesselAutopilot.AutopilotMode mode, int skillLvl)
	{
		if (HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().KerbalExperienceEnabled(HighLogic.CurrentGame.Mode))
		{
			return skillLvl >= mode.GetRequiredSkill();
		}
		return true;
	}

	public static int GetRequiredSkill(this VesselAutopilot.AutopilotMode mode)
	{
		switch (mode)
		{
		default:
			return 0;
		case VesselAutopilot.AutopilotMode.Prograde:
		case VesselAutopilot.AutopilotMode.Retrograde:
			return 1;
		case VesselAutopilot.AutopilotMode.Normal:
		case VesselAutopilot.AutopilotMode.Antinormal:
		case VesselAutopilot.AutopilotMode.RadialIn:
		case VesselAutopilot.AutopilotMode.RadialOut:
			return 2;
		case VesselAutopilot.AutopilotMode.Target:
		case VesselAutopilot.AutopilotMode.AntiTarget:
		case VesselAutopilot.AutopilotMode.Maneuver:
			return 3;
		}
	}
}
