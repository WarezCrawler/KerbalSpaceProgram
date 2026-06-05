using System.Collections.Generic;
using UnityEngine;

namespace KerbalEngineer.Drawing;

internal class DebugDrawing
{
	private static Material _material;

	private static Material material
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			if ((Object)(object)_material == (Object)null)
			{
				_material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
			}
			return _material;
		}
	}

	public static void DrawGroundMarker(CelestialBody body, double latitude, double longitude, Color c, bool map, double rotation = 0.0, double radius = 0.0)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		Vector3d surfaceNVector = body.GetSurfaceNVector(latitude, longitude);
		double num = 1.0 + body.pqsController.GetSurfaceHeight(QuaternionD.AngleAxis(longitude, Vector3d.down) * QuaternionD.AngleAxis(latitude, Vector3d.forward) * Vector3d.right);
		if (num < body.Radius + 1.0)
		{
			num = body.Radius + 1.0;
		}
		Vector3d val = body.position + num * surfaceNVector;
		Vector3d val2 = Vector3d.Exclude(surfaceNVector, Vector3d.op_Implicit(((Component)body).transform.up));
		Vector3d normalized = ((Vector3d)(ref val2)).normalized;
		if (map)
		{
			Vector3d camPos = ScaledSpace.ScaledToLocalSpace(Vector3d.op_Implicit(((Component)PlanetariumCamera.Camera).transform.position));
			if (IsOccluded(val, body, camPos))
			{
				return;
			}
		}
		if (radius <= 0.0)
		{
			radius = (map ? (body.Radius / 50.0) : 15.0);
		}
		List<Vector3d> list = new List<Vector3d>();
		int num2 = 64;
		for (int i = 0; i <= num2; i++)
		{
			list.Add(val + radius * 0.85 * (QuaternionD.AngleAxis(rotation + (double)(i * 360 / num2), surfaceNVector) * normalized));
			list.Add(val + radius * (QuaternionD.AngleAxis(rotation + (double)(i * 360 / num2), surfaceNVector) * normalized));
		}
		GLTriangleStrip(list, c, map);
		for (int j = 0; j <= num2; j++)
		{
			list.Add(val + radius * 0.45 * (QuaternionD.AngleAxis(rotation + (double)(j * 360 / num2), surfaceNVector) * normalized));
			list.Add(val + radius * 0.6 * (QuaternionD.AngleAxis(rotation + (double)(j * 360 / num2), surfaceNVector) * normalized));
		}
		GLTriangleStrip(list, c, map);
		for (int k = 0; k <= num2; k++)
		{
			list.Add(val);
			list.Add(val + radius * 0.2 * (QuaternionD.AngleAxis(rotation + (double)(k * 360 / num2), surfaceNVector) * normalized));
		}
		GLTriangleStrip(list, c, map);
	}

	public static void GLTriangleStrip(List<Vector3d> Verts, Color c, bool map)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		GL.PushMatrix();
		material.SetPass(0);
		if (map)
		{
			GL.LoadOrtho();
			GL.Begin(5);
			GL.Color(c);
			foreach (Vector3d Vert in Verts)
			{
				Vector3 val = PlanetariumCamera.Camera.WorldToViewportPoint(Vector3d.op_Implicit(ScaledSpace.LocalToScaledSpace(Vert)));
				GL.Vertex3(val.x, val.y, 0f);
			}
			GL.End();
		}
		else
		{
			GL.LoadProjectionMatrix(Camera.current.projectionMatrix);
			GL.Begin(5);
			GL.Color(c);
			foreach (Vector3d Vert2 in Verts)
			{
				GL.Vertex3((float)Vert2.x, (float)Vert2.y, (float)Vert2.z);
			}
			GL.End();
		}
		GL.PopMatrix();
	}

	public static void GLPixelLine(Vector3d worldPosition1, Vector3d worldPosition2, bool map)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val;
		Vector3 val2;
		if (map)
		{
			val = PlanetariumCamera.Camera.WorldToScreenPoint(Vector3d.op_Implicit(ScaledSpace.LocalToScaledSpace(worldPosition1)));
			val2 = PlanetariumCamera.Camera.WorldToScreenPoint(Vector3d.op_Implicit(ScaledSpace.LocalToScaledSpace(worldPosition2)));
		}
		else
		{
			val = FlightCamera.fetch.mainCamera.WorldToScreenPoint(Vector3d.op_Implicit(worldPosition1));
			val2 = FlightCamera.fetch.mainCamera.WorldToScreenPoint(Vector3d.op_Implicit(worldPosition2));
		}
		if (val.z > 0f && val2.z > 0f)
		{
			GL.Vertex3(val.x, val.y, 0f);
			GL.Vertex3(val2.x, val2.y, 0f);
		}
	}

	public static bool IsOccluded(Vector3d worldPosition, CelestialBody byBody, Vector3d camPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector3d val = (byBody.position - camPos) / (byBody.Radius - 100.0);
		Vector3d val2 = (worldPosition - camPos) / (byBody.Radius - 100.0);
		double num = Vector3d.Dot(val2, val);
		if (num < ((Vector3d)(ref val)).sqrMagnitude - 1.0)
		{
			return false;
		}
		return num * num / ((Vector3d)(ref val2)).sqrMagnitude > ((Vector3d)(ref val)).sqrMagnitude - 1.0;
	}

	public static void DrawPath(CelestialBody mainBody, List<Vector3d> points, Color c, bool map, bool dashed = false)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		GL.PushMatrix();
		material.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Begin(1);
		GL.Color(c);
		Vector3d camPos = (map ? ScaledSpace.ScaledToLocalSpace(Vector3d.op_Implicit(((Component)PlanetariumCamera.Camera).transform.position)) : Vector3d.op_Implicit(((Component)FlightCamera.fetch.mainCamera).transform.position));
		int num = ((!dashed) ? 1 : 2);
		for (int i = 0; i < points.Count - 1; i += num)
		{
			if (!IsOccluded(points[i], mainBody, camPos) && !IsOccluded(points[i + 1], mainBody, camPos))
			{
				GLPixelLine(points[i], points[i + 1], map);
			}
		}
		GL.End();
		GL.PopMatrix();
	}
}
