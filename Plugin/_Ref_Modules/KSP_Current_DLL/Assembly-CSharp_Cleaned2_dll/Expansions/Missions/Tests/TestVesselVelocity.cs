using System;
using System.ComponentModel;
using Expansions.Missions.Editor;
using ns9;

namespace Expansions.Missions.Tests;

[MEScoreModule(new Type[]
{
	typeof(ScoreModule_Resource),
	typeof(ScoreModule_Time)
})]
public class TestVesselVelocity : TestVessel
{
	public enum SpeedType
	{
		[Description("#autoLOC_8004167")]
		SurfaceVelocity,
		[Description("#autoLOC_8004168")]
		OrbitalVelocity
	}

	[MEGUI_InputField(ContentType = MEGUI_Control.InputContentType.DecimalNumber, resetValue = "0", guiName = "#autoLOC_8004169", Tooltip = "#autoLOC_8004170")]
	public float velocity;

	[MEGUI_Dropdown(canBePinned = false, resetValue = "GreaterThan", canBeReset = true, guiName = "#autoLOC_8000052", Tooltip = "#autoLOC_8000053")]
	public TestComparisonLessGreaterOnly comparisonOperator = TestComparisonLessGreaterOnly.GreaterThan;

	[MEGUI_Dropdown(canBePinned = true, resetValue = "SurfaceVelocity", canBeReset = true, guiName = "#autoLOC_8004171", Tooltip = "#autoLOC_8004172")]
	public SpeedType speedType;

	public override void Awake()
	{
		base.Awake();
		title = Localizer.Format("#autoLOC_8004164");
		useActiveVessel = true;
	}

	public override bool Test()
	{
		base.Test();
		if (vessel == null)
		{
			return true;
		}
		double num = 0.0;
		if (speedType == SpeedType.SurfaceVelocity)
		{
			num = vessel.srfSpeed;
		}
		else if (speedType == SpeedType.OrbitalVelocity)
		{
			num = vessel.obt_speed;
		}
		if (comparisonOperator == TestComparisonLessGreaterOnly.GreaterThan && !(num < (double)velocity))
		{
			return true;
		}
		if (comparisonOperator == TestComparisonLessGreaterOnly.LessThan)
		{
			return num <= (double)velocity;
		}
		return false;
	}

	public override string GetInfo()
	{
		return Localizer.Format("#autoLOC_8004165");
	}

	public override string GetNodeBodyParameterString(BaseAPField field)
	{
		if (field.name == "velocity")
		{
			return Localizer.Format("#autoLOC_8100154", field.guiName, (comparisonOperator == TestComparisonLessGreaterOnly.LessThan) ? "<" : ">", velocity.ToString("0"));
		}
		return base.GetNodeBodyParameterString(field);
	}

	public override void Save(ConfigNode node)
	{
		base.Save(node);
		node.AddValue("velocity", velocity);
		node.AddValue("comparisonOperator", comparisonOperator);
		node.AddValue("speedType", speedType);
	}

	public override void Load(ConfigNode node)
	{
		base.Load(node);
		node.TryGetValue("velocity", ref velocity);
		node.TryGetEnum("comparisonOperator", ref comparisonOperator, TestComparisonLessGreaterOnly.GreaterThan);
		node.TryGetEnum("speedType", ref speedType, SpeedType.SurfaceVelocity);
	}
}
