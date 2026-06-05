using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class AttitudeProcessor : IUpdatable, IUpdateRequest
{
	private static readonly AttitudeProcessor instance = new AttitudeProcessor();

	private Vector3 centreOfMass = Vector3.zero;

	private double heading;

	private double headingRate;

	private Vector3 north = Vector3.zero;

	private double pitch;

	private double pitchRate;

	private double previousHeading;

	private double previousPitch;

	private double previousRoll;

	private double roll;

	private double rollRate;

	private Quaternion surfaceRotation;

	private Vector3 up = Vector3.zero;

	public static double Heading => instance.heading;

	public static double HeadingRate => instance.headingRate;

	public static AttitudeProcessor Instance => instance;

	public static double Pitch => instance.pitch;

	public static double PitchRate => instance.pitchRate;

	public static double Roll => instance.roll;

	public static double RollRate => instance.rollRate;

	public bool UpdateRequested { get; set; }

	public static void RequestUpdate()
	{
		instance.UpdateRequested = true;
	}

	public void Update()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		surfaceRotation = GetSurfaceRotation();
		previousHeading = heading;
		previousPitch = pitch;
		previousRoll = roll;
		heading = ((Quaternion)(ref surfaceRotation)).eulerAngles.y;
		pitch = ((((Quaternion)(ref surfaceRotation)).eulerAngles.x > 180f) ? (360f - ((Quaternion)(ref surfaceRotation)).eulerAngles.x) : (0f - ((Quaternion)(ref surfaceRotation)).eulerAngles.x));
		roll = ((((Quaternion)(ref surfaceRotation)).eulerAngles.z > 180f) ? (360f - ((Quaternion)(ref surfaceRotation)).eulerAngles.z) : (0f - ((Quaternion)(ref surfaceRotation)).eulerAngles.z));
		headingRate = heading - previousHeading;
		pitchRate = pitch - previousPitch;
		rollRate = roll - previousRoll;
	}

	private Quaternion GetSurfaceRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		centreOfMass = Vector3d.op_Implicit(FlightGlobals.ActiveVessel.CoMD);
		Vector3d val = centreOfMass - FlightGlobals.ActiveVessel.mainBody.position;
		up = Vector3d.op_Implicit(((Vector3d)(ref val)).normalized);
		Vector3 val2 = Vector3.ProjectOnPlane(Vector3d.op_Implicit(FlightGlobals.ActiveVessel.mainBody.position + ((Component)FlightGlobals.ActiveVessel.mainBody).transform.up * (float)FlightGlobals.ActiveVessel.mainBody.Radius - centreOfMass), up);
		north = ((Vector3)(ref val2)).normalized;
		return Quaternion.Inverse(Quaternion.Euler(90f, 0f, 0f) * Quaternion.Inverse(((Component)FlightGlobals.ActiveVessel).transform.rotation) * Quaternion.LookRotation(north, up));
	}
}
