using System;
using UnityEngine;
using ns9;

public static class DiscoverableObjectsUtil
{
	public static string GenerateAsteroidName()
	{
		string text = "";
		text += randomLetter(uppercase: true);
		text += randomLetter(uppercase: true);
		text += randomLetter(uppercase: true);
		text += "-";
		text += randomNumber(3);
		return Localizer.Format("#autoLOC_6001923", text);
	}

	private static char randomLetter(bool uppercase)
	{
		int num = UnityEngine.Random.Range(0, 26);
		char c = (char)(97 + num);
		if (uppercase)
		{
			return char.ToUpper(c);
		}
		return c;
	}

	private static string randomNumber(int digits)
	{
		int max = (int)Math.Pow(10.0, digits);
		string text = "";
		for (int i = 0; i < digits; i++)
		{
			text += "0";
		}
		return UnityEngine.Random.Range(0, max).ToString(text);
	}

	public static ProtoVessel SpawnAsteroid(string asteroidName, Orbit o, uint seed, UntrackedObjectClass objClass, double lifeTime, double lifeTimeMax)
	{
		ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(asteroidName, VesselType.SpaceObject, o, 0, new ConfigNode[1] { ProtoVessel.CreatePartNode("PotatoRoid", seed) }, new ConfigNode("ACTIONGROUPS"), ProtoVessel.CreateDiscoveryNode(DiscoveryLevels.Presence, objClass, lifeTime, lifeTimeMax));
		ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(protoVesselNode);
		GameEvents.onAsteroidSpawned.Fire(protoVessel.vesselRef);
		return protoVessel;
	}
}
