using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class Situation : ReadoutModule
{
	public Situation()
	{
		base.Name = "Situation";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's current scientific situation. (Landed, Splashed, Flying Low/High, In Space Low/High)";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		ExperimentSituations experimentSituation = ScienceUtil.GetExperimentSituation(FlightGlobals.ActiveVessel);
		if ((int)experimentSituation <= 8)
		{
			switch (experimentSituation - 1)
			{
			case 0:
				DrawLine("Landed", section.IsHud);
				return;
			case 1:
				DrawLine("Splashed", section.IsHud);
				return;
			case 3:
				DrawLine("Flying Low", section.IsHud);
				return;
			case 2:
				return;
			}
			if ((int)experimentSituation == 8)
			{
				DrawLine("Flying High", section.IsHud);
			}
		}
		else if ((int)experimentSituation != 16)
		{
			if ((int)experimentSituation == 32)
			{
				DrawLine("In Space High", section.IsHud);
			}
		}
		else
		{
			DrawLine("In Space Low", section.IsHud);
		}
	}

	private static string GetBiome()
	{
		return ScienceUtil.GetExperimentBiomeLocalized(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
	}

	private static string GetBodyPlural()
	{
		if (!FlightGlobals.currentMainBody.bodyName.EndsWith("s"))
		{
			return FlightGlobals.currentMainBody.bodyName + "'s";
		}
		return FlightGlobals.currentMainBody.bodyName + "'";
	}
}
