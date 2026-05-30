using System;
using UnityEngine;

namespace ProceduralFairings;

[Serializable]
public class FairingXSection : IComparable<FairingXSection>, IConfigNode
{
	public float h;

	public float r;

	public bool isLast;

	public bool isCap;

	public bool isValid;

	public Color color;

	[NonSerialized]
	private FairingXSection lerp1;

	[NonSerialized]
	private FairingXSection lerp2;

	private bool isLerp;

	public bool IsLerp => isLerp;

	public FairingXSection()
	{
		isLerp = false;
	}

	public FairingXSection(bool isCap)
	{
		this.isCap = isCap;
	}

	public FairingXSection(FairingXSection from, FairingXSection to)
	{
		lerp1 = from;
		lerp2 = to;
		isLerp = true;
	}

	public FairingXSection(FairingXSection cloneOf)
	{
		h = cloneOf.h;
		r = cloneOf.r;
		isCap = cloneOf.isCap;
		isLast = cloneOf.isLast;
		isValid = cloneOf.isValid;
		color = cloneOf.color;
		isLerp = cloneOf.isLerp;
		lerp1 = cloneOf.lerp1;
		lerp2 = cloneOf.lerp2;
	}

	public void Load(ConfigNode node)
	{
		if (node.HasValue("h"))
		{
			h = float.Parse(node.GetValue("h"));
		}
		if (node.HasValue("r"))
		{
			r = float.Parse(node.GetValue("r"));
		}
	}

	public void Save(ConfigNode node)
	{
		node.AddValue("h", h);
		node.AddValue("r", r);
	}

	public void UpdateLerp(float t, float y)
	{
		if (isLerp)
		{
			h = Mathf.Lerp(lerp1.h, lerp2.h, t);
			r = Mathf.Lerp(lerp1.r, lerp2.r, t) + y;
			color = Color.Lerp(lerp1.color, lerp2.color, t);
		}
	}

	public static float GetSlopeAngle(FairingXSection from, FairingXSection to)
	{
		return Mathf.Atan((to.h - from.h) / (from.r - to.r));
	}

	public static float CircleCast(FairingXSection xs, Vector3 wAxis, Vector3 wPivot, Vector3 wRadial, int nRays, float rLength, int layerMask, out float lVariance, out RaycastHit hit)
	{
		nRays = Mathf.Max(nRays, 2);
		float num = 360f / Mathf.Max(nRays, 2f);
		float num2 = 0f;
		float num3 = float.MaxValue;
		lVariance = 0f;
		hit = default(RaycastHit);
		for (int i = 0; i < nRays; i++)
		{
			Vector3 vector = Quaternion.AngleAxis(num * (float)i, wAxis) * wRadial;
			if (Physics.Raycast(new Ray(wPivot + wAxis * xs.h + vector * rLength, -vector), out hit, rLength, layerMask))
			{
				num2 = Mathf.Max(rLength - hit.distance, num2);
				num3 = Mathf.Min(rLength - hit.distance, num3);
			}
		}
		lVariance = num2 - num3;
		return num2;
	}

	public static bool ConeCast(FairingXSection xsFrom, FairingXSection xsTo, Vector3 wAxis, Vector3 wPivot, Vector3 wRadial, float radiusOffset, int nRays, int layerMask, out float hitLengthScalar, float aOffset = 0f)
	{
		nRays = Mathf.Max(nRays, 2);
		float num = 360f / Mathf.Max(nRays, 2f);
		hitLengthScalar = 1f;
		int num2 = 0;
		float magnitude;
		RaycastHit hitInfo;
		while (true)
		{
			if (num2 < nRays)
			{
				Vector3 vector = Quaternion.AngleAxis(num * (float)num2 + aOffset, wAxis) * wRadial;
				Vector3 vector2 = wPivot + wAxis * xsFrom.h + vector * (xsFrom.r + radiusOffset);
				Vector3 vector3 = wPivot + wAxis * xsTo.h + vector * (xsTo.r + radiusOffset);
				Ray ray = new Ray(vector3, vector2 - vector3);
				magnitude = (vector3 - vector2).magnitude;
				if (Physics.Raycast(ray, out hitInfo, magnitude, layerMask))
				{
					break;
				}
				num2++;
				continue;
			}
			return false;
		}
		hitLengthScalar = hitInfo.distance / magnitude;
		return true;
	}

	public int CompareTo(FairingXSection b)
	{
		return h.CompareTo(b.h);
	}
}
