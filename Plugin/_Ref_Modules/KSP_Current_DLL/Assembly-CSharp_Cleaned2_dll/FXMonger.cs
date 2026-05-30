using System.Collections;
using System.Collections.Generic;
using Expansions;
using UnityEngine;

public class FXMonger : MonoBehaviour
{
	private class ProtoExplosion
	{
		public Vector3d position;

		public double power;

		public List<Part> sources;

		public ProtoExplosion(Part source, Vector3d pos, double powah)
		{
			sources = new List<Part>();
			sources.Add(source);
			position = pos;
			power = powah;
		}
	}

	private class ROCProtoExplosion
	{
		public Vector3d position;

		public double power;

		public ROCProtoExplosion(Vector3 pos, double powah)
		{
			position = pos;
			power = powah;
		}
	}

	public GameObject[] explosions;

	public AudioClip[] explosionSounds;

	public GameObject[] thuds;

	public AudioClip[] crashSounds;

	public GameObject[] splashes;

	public AudioClip[] splashSounds;

	public float MinSqr;

	public double minPower = 0.001;

	private List<ProtoExplosion> queuedExplosions = new List<ProtoExplosion>();

	private List<ROCProtoExplosion> queuedROCExplosions = new List<ROCProtoExplosion>();

	protected List<FXObject> explosionObjects = new List<FXObject>();

	private static FXMonger fetch;

	private static Particle[] sParts = (Particle[])(object)new Particle[1];

	public float MINIMUM_DISTANCE_FROM_OTHER_BLASTS => 10f;

	private void Awake()
	{
		fetch = this;
		MinSqr = MINIMUM_DISTANCE_FROM_OTHER_BLASTS;
		MinSqr *= MinSqr;
		if (minPower <= 0.0)
		{
			minPower = 0.001;
		}
		else if (minPower > 1.0)
		{
			minPower = 1.0;
		}
	}

	private void OnDestroy()
	{
		if (fetch != null && fetch == this)
		{
			fetch = null;
		}
	}

	public static void Explode(Part source, Vector3d blastPos, double howhard)
	{
		if (fetch != null)
		{
			fetch.explode(source, blastPos, howhard);
		}
	}

	private void explode(Part source, Vector3d blastPos, double howHard)
	{
		if (howHard < minPower)
		{
			howHard = minPower;
		}
		int count = queuedExplosions.Count;
		int num = 0;
		ProtoExplosion protoExplosion;
		while (true)
		{
			if (num < count)
			{
				protoExplosion = queuedExplosions[num];
				if (!((protoExplosion.position - blastPos).sqrMagnitude >= (double)MinSqr))
				{
					break;
				}
				num++;
				continue;
			}
			queuedExplosions.Add(new ProtoExplosion(source, blastPos, howHard));
			return;
		}
		double num2 = protoExplosion.power + howHard;
		protoExplosion.position = (protoExplosion.position * protoExplosion.power + blastPos * howHard) / num2;
		protoExplosion.sources.Add(source);
		PDebug.Log("[Explosion] Combined.");
	}

	public static void ROCExplode(Vector3d blastPos, double howhard)
	{
		if (ExpansionsLoader.IsExpansionInstalled("Serenity") && fetch != null)
		{
			fetch.rocexplode(blastPos, howhard);
		}
	}

	private void rocexplode(Vector3d blastPos, double howHard)
	{
		if (howHard < minPower)
		{
			howHard = minPower;
		}
		int count = queuedROCExplosions.Count;
		int num = 0;
		ROCProtoExplosion rOCProtoExplosion;
		while (true)
		{
			if (num < count)
			{
				rOCProtoExplosion = queuedROCExplosions[num];
				if (!((rOCProtoExplosion.position - blastPos).sqrMagnitude >= (double)MinSqr))
				{
					break;
				}
				num++;
				continue;
			}
			queuedROCExplosions.Add(new ROCProtoExplosion(blastPos, howHard));
			return;
		}
		double num2 = rOCProtoExplosion.power + howHard;
		rOCProtoExplosion.position = (rOCProtoExplosion.position * rOCProtoExplosion.power + blastPos * howHard) / num2;
		PDebug.Log("[ROCExplosion] Combined.");
	}

	private void LateUpdate()
	{
		int num = 0;
		int count = queuedExplosions.Count;
		int num2 = 0;
		while (true)
		{
			if (num2 < count)
			{
				ProtoExplosion protoExplosion = queuedExplosions[num2];
				if (protoExplosion != null)
				{
					double num3 = UtilMath.Clamp01(protoExplosion.power);
					if (double.IsNaN(protoExplosion.position.x) || double.IsNaN(protoExplosion.position.y) || double.IsNaN(protoExplosion.position.z))
					{
						break;
					}
					GameObject gameObject = Object.Instantiate(explosions[(int)(num3 * (double)(explosions.Length - 1))]);
					gameObject.SetActive(value: true);
					gameObject.transform.position = protoExplosion.position;
					gameObject.transform.up = FlightGlobals.upAxis;
					AudioClip effectSound = explosionSounds[(int)(num3 * (double)(explosionSounds.Length - 1))];
					FXObject fXObject = new FXObject(gameObject);
					fXObject.effectSound = effectSound;
					gameObject.AddComponent<FXObjectPhoneHome>().parent = fXObject;
					explosionObjects.Add(fXObject);
					if (!gameObject.GetComponent<AudioSource>())
					{
						gameObject.gameObject.AddComponent<AudioSource>();
					}
					gameObject.GetComponent<AudioSource>().PlayOneShot(fXObject.effectSound, GameSettings.SHIP_VOLUME);
					num++;
				}
				num2++;
				continue;
			}
			if (num > 0)
			{
				PDebug.Log(num + " explosions created.");
			}
			int num4 = 0;
			int count2 = queuedROCExplosions.Count;
			int num5 = 0;
			while (true)
			{
				if (num5 < count2)
				{
					ROCProtoExplosion rOCProtoExplosion = queuedROCExplosions[num5];
					if (rOCProtoExplosion != null)
					{
						double num6 = UtilMath.Clamp01(rOCProtoExplosion.power);
						if (double.IsNaN(rOCProtoExplosion.position.x) || double.IsNaN(rOCProtoExplosion.position.y) || double.IsNaN(rOCProtoExplosion.position.z))
						{
							break;
						}
						GameObject gameObject2 = Object.Instantiate(explosions[(int)(num6 * (double)(explosions.Length - 1))]);
						gameObject2.SetActive(value: true);
						gameObject2.transform.position = rOCProtoExplosion.position;
						gameObject2.transform.up = FlightGlobals.upAxis;
						AudioClip effectSound2 = explosionSounds[(int)(num6 * (double)(explosionSounds.Length - 1))];
						FXObject fXObject2 = new FXObject(gameObject2);
						fXObject2.effectSound = effectSound2;
						gameObject2.AddComponent<FXObjectPhoneHome>().parent = fXObject2;
						explosionObjects.Add(fXObject2);
						if (!gameObject2.GetComponent<AudioSource>())
						{
							gameObject2.gameObject.AddComponent<AudioSource>();
						}
						gameObject2.GetComponent<AudioSource>().PlayOneShot(fXObject2.effectSound, GameSettings.SHIP_VOLUME);
						num4++;
					}
					num5++;
					continue;
				}
				if (num4 > 0)
				{
					PDebug.Log(num4 + " ROC explosions created.");
				}
				queuedExplosions.Clear();
				queuedROCExplosions.Clear();
				break;
			}
			break;
		}
	}

	public static FXObject Splash(Vector3 pos, float howHard)
	{
		if (!fetch)
		{
			return null;
		}
		return fetch.splash(pos, howHard);
	}

	private FXObject splash(Vector3 pos, float howHard)
	{
		howHard = Mathf.Clamp01(howHard);
		GameObject gameObject = Object.Instantiate(splashes[(int)(howHard * (float)(splashes.Length - 1))]);
		gameObject.gameObject.SetActive(value: true);
		gameObject.transform.position = pos;
		gameObject.transform.up = FlightGlobals.getUpAxis(FlightGlobals.currentMainBody, pos);
		gameObject.transform.Translate(0f, 0f - FlightGlobals.getAltitudeAtPos(gameObject.transform.position, FlightGlobals.currentMainBody), 0f);
		AudioClip effectSound = splashSounds[(int)(howHard * (float)(splashSounds.Length - 1))];
		FXObject fXObject = new FXObject(gameObject);
		fXObject.effectSound = effectSound;
		gameObject.AddComponent<FXObjectPhoneHome>().parent = fXObject;
		explosionObjects.Add(fXObject);
		return fXObject;
	}

	private IEnumerator explosionTest()
	{
		yield return new WaitForSeconds(5f);
		explode(null, Vector3.up * 5f, 1.0);
		MonoBehaviour.print("exploded.");
	}

	public void offsetPositions(Vector3d offset)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Invalid comparison between Unknown and I4
		int count = explosionObjects.Count;
		while (count-- > 0)
		{
			FXObject fXObject = explosionObjects[count];
			GameObject effectObj = explosionObjects[count].effectObj;
			if (effectObj == null)
			{
				fXObject.effectObj = null;
				fXObject.effectSound = null;
				fXObject.systems.Clear();
				explosionObjects.RemoveAt(count);
			}
			effectObj.transform.position = (Vector3d)effectObj.transform.position + offset;
			int count2 = fXObject.systems.Count;
			while (count2-- > 0)
			{
				ParticleSystem val = fXObject.systems[count2];
				if ((Object)(object)val == null)
				{
					continue;
				}
				MainModule main = val.main;
				if ((int)((MainModule)(ref main)).simulationSpace == 1 && val.particleCount > 0)
				{
					int maxParticles = ((MainModule)(ref main)).maxParticles;
					if (sParts.Length < maxParticles)
					{
						sParts = (Particle[])(object)new Particle[maxParticles];
					}
					int particles = val.GetParticles(sParts);
					int num = particles;
					while (num-- > 0)
					{
						((Particle)(ref sParts[num])).position = (Vector3d)((Particle)(ref sParts[num])).position + offset;
					}
					val.SetParticles(sParts, particles);
				}
			}
		}
	}

	public static void RemoveFXOjbect(FXObject obj)
	{
		obj.effectObj = null;
		obj.effectSound = null;
		obj.systems.Clear();
		if ((bool)fetch)
		{
			fetch.explosionObjects.Remove(obj);
		}
	}

	public static void OffsetPositions(Vector3d offset)
	{
		if ((bool)fetch)
		{
			fetch.offsetPositions(offset);
		}
	}
}
