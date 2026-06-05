using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KerbalEngineer;

public static class CelestialBodies
{
	public class BodyInfo
	{
		public CelestialBody CelestialBody { get; private set; }

		public List<BodyInfo> Children { get; private set; }

		public double Gravity { get; private set; }

		public string Name { get; private set; }

		public BodyInfo Parent { get; private set; }

		public bool Selected { get; private set; }

		public int SelectedDepth { get; private set; }

		public BodyInfo(CelestialBody body, BodyInfo parent = null)
		{
			try
			{
				CelestialBody = body;
				Name = body.bodyName;
				Gravity = 9.81 * body.GeeASL;
				Parent = parent;
				Children = new List<BodyInfo>();
				foreach (CelestialBody orbitingBody in body.orbitingBodies)
				{
					Children.Add(new BodyInfo(orbitingBody, this));
				}
				SelectedDepth = 0;
			}
			catch (Exception ex)
			{
				MyLogger.Exception(ex);
			}
		}

		public BodyInfo GetBodyInfo(string bodyName)
		{
			try
			{
				if (string.Equals(Name, bodyName, StringComparison.CurrentCultureIgnoreCase))
				{
					return this;
				}
				foreach (BodyInfo child in Children)
				{
					BodyInfo bodyInfo = child.GetBodyInfo(bodyName);
					if (bodyInfo != null)
					{
						return bodyInfo;
					}
				}
			}
			catch (Exception ex)
			{
				MyLogger.Exception(ex);
			}
			return null;
		}

		public double GetDensity(double altitude)
		{
			return CelestialBody.GetDensity(GetPressure(altitude), GetTemperature(altitude));
		}

		public double GetPressure(double altitude)
		{
			return CelestialBody.GetPressure(altitude);
		}

		public double GetTemperature(double altitude)
		{
			return CelestialBody.GetTemperature(altitude);
		}

		public double GetAtmospheres(double altitude)
		{
			return GetPressure(altitude) * PhysicsGlobals.KpaToAtmospheres;
		}

		public void SetSelected(bool state, int depth = 0)
		{
			Selected = state;
			SelectedDepth = depth;
			if (Parent != null)
			{
				Parent.SetSelected(state, depth + 1);
			}
		}

		public override string ToString()
		{
			string seed = "\n" + Name + "\n\tGravity: " + Gravity + "\n\tSelected: " + Selected.ToString();
			return Children.Aggregate(seed, (string current, BodyInfo child) => current + "\n" + child);
		}
	}

	public static BodyInfo SelectedBody { get; private set; }

	public static BodyInfo SystemBody { get; private set; }

	static CelestialBodies()
	{
		try
		{
			SystemBody = new BodyInfo(PSystemManager.Instance.localBodies.Find((CelestialBody b) => (Object)(object)b.referenceBody == (Object)null || (Object)(object)b.referenceBody == (Object)(object)b));
			if (!SetSelectedBody(Planetarium.fetch.Home.bodyName))
			{
				SelectedBody = SystemBody;
				SelectedBody.SetSelected(state: true);
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	public static BodyInfo GetBodyInfo(string bodyName)
	{
		try
		{
			return SystemBody.GetBodyInfo(bodyName);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
		return null;
	}

	public static bool SetSelectedBody(string bodyName)
	{
		try
		{
			BodyInfo bodyInfo = GetBodyInfo(bodyName);
			if (bodyInfo != null)
			{
				if (SelectedBody != null)
				{
					SelectedBody.SetSelected(state: false);
				}
				SelectedBody = bodyInfo;
				SelectedBody.SetSelected(state: true);
				return true;
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
		return false;
	}
}
