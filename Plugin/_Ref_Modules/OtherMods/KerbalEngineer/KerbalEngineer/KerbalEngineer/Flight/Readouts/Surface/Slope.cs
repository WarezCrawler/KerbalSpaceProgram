using System;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class Slope : ReadoutModule
{
	public Slope()
	{
		base.Name = "Slope";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the slope of the terrain below your vessel.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(GetSlopeAngleAndHeading(), section.IsHud);
	}

	private string GetSlopeAngleAndHeading()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string result = "--° @ ---°";
			CelestialBody mainBody = FlightGlobals.ActiveVessel.mainBody;
			Vector3d val = FlightGlobals.ActiveVessel.CoM - mainBody.position;
			Vector3d normalized = ((Vector3d)(ref val)).normalized;
			RaycastHit val2 = default(RaycastHit);
			if (Physics.Raycast(FlightGlobals.ActiveVessel.CoM, Vector3d.op_Implicit(-normalized), ref val2, float.PositiveInfinity, 32768))
			{
				Vector3 val3 = ((RaycastHit)(ref val2)).normal;
				val3 = ((Vector3)(ref val3)).normalized;
				double num = Vector3d.Dot(normalized, Vector3d.op_Implicit(val3));
				if (num > 1.0)
				{
					num = 1.0;
				}
				else if (num < 0.0)
				{
					num = 0.0;
				}
				double num2 = Math.Acos(num) * 180.0 / Math.PI;
				result = Units.ToAngle(num2, 1);
				if (num2 < 0.05)
				{
					result += " @ ---°";
				}
				else
				{
					val = Vector3d.Cross(normalized, Vector3d.op_Implicit(val3));
					Vector3d normalized2 = ((Vector3d)(ref val)).normalized;
					val = Vector3d.Cross(normalized, Vector3d.up);
					Vector3d normalized3 = ((Vector3d)(ref val)).normalized;
					val = Vector3d.Cross(normalized, normalized3);
					Vector3d normalized4 = ((Vector3d)(ref val)).normalized;
					double num3 = Math.Acos(Vector3d.Dot(normalized2, normalized3)) * 180.0 / Math.PI;
					if (Vector3d.Dot(normalized2, normalized4) < 0.0)
					{
						num3 = 360.0 - num3;
					}
					result = result + " @ " + Units.ToAngle(num3, 1);
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "Surface->Slope->GetSlopeAngleAndHeading");
			return "--° @ ---°";
		}
	}
}
