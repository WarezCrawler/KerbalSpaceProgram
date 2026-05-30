using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ProceduralAsteroid : MonoBehaviour
{
	[Serializable]
	public class ModValue
	{
		public string name;

		public float minValue;

		public float maxValue;

		public float radiusFactor;
	}

	[Serializable]
	public class ModWrapper
	{
		public string name;

		public ModValue[] values;

		public PQSMod mod { get; set; }
	}

	[SerializeField]
	private int seed;

	[SerializeField]
	public float radius;

	[SerializeField]
	private SphereBaseSO visualSphere;

	[SerializeField]
	private Material primaryMaterial;

	[SerializeField]
	private Material secondaryMaterial;

	[SerializeField]
	private string visualLayer;

	[SerializeField]
	private string visualTag;

	[SerializeField]
	private SphereBaseSO colliderSphere;

	[SerializeField]
	private PhysicMaterial colliderMaterial;

	[SerializeField]
	private string colliderLayer;

	[SerializeField]
	private string colliderTag;

	[SerializeField]
	private SphereBaseSO convexSphere;

	[SerializeField]
	private PhysicMaterial convexMaterial;

	[SerializeField]
	private string convexLayer;

	[SerializeField]
	private string convexTag;

	[SerializeField]
	private bool debugGenTime;

	[SerializeField]
	private List<ModWrapper> mods;

	private PAsteroid paGenerated;

	private void Reset()
	{
		NewSeed();
		UpdateWrappers();
		radius = 1f;
		visualLayer = "Default";
		visualTag = "Untagged";
		colliderLayer = "Default";
		colliderTag = "Untagged";
		convexLayer = "Default";
		convexTag = "Untagged";
	}

	private void Start()
	{
		Generate(seed, radius, null, RangefinderGeneric, delegate
		{
		});
	}

	public PAsteroid Generate(int seed, float radius, Transform parent, Func<Transform, float> rangefinder, Callback onComplete, bool isSecondary = false)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		UnityEngine.Random.InitState(seed);
		List<PQSMod> list = new List<PQSMod>();
		int i = 0;
		for (int count = mods.Count; i < count; i++)
		{
			mods[i].mod = (PQSMod)GetComponent(mods[i].name);
			if (mods[i].mod != null)
			{
				list.Add(mods[i].mod);
			}
		}
		int j = 0;
		for (int count2 = mods.Count; j < count2; j++)
		{
			if (mods[j].mod == null)
			{
				continue;
			}
			ModWrapper modWrapper = mods[j];
			int k = 0;
			for (int num = modWrapper.values.Length; k < num; k++)
			{
				ModValue modValue = modWrapper.values[k];
				FieldInfo field = modWrapper.mod.GetType().GetField(modValue.name);
				if (field != null)
				{
					float num2 = UnityEngine.Random.Range(modValue.minValue, modValue.maxValue);
					if (modValue.radiusFactor != 0f)
					{
						num2 *= radius * modValue.radiusFactor;
					}
					if (field.FieldType == typeof(int))
					{
						field.SetValue(modWrapper.mod, (int)num2);
					}
					else if (field.FieldType == typeof(float))
					{
						field.SetValue(modWrapper.mod, num2);
					}
					else if (field.FieldType == typeof(double))
					{
						field.SetValue(modWrapper.mod, (double)num2);
					}
					else if (field.FieldType == typeof(bool))
					{
						field.SetValue(modWrapper.mod, num2 > 0.5f);
					}
				}
			}
		}
		Color secondaryColor;
		PAsteroid pAsteroid = CreatePAsteroid(radius, list, rangefinder, onComplete, isSecondary, out secondaryColor);
		pAsteroid.transform.parent = parent;
		pAsteroid.transform.localPosition = Vector3.zero;
		pAsteroid.transform.localRotation = Quaternion.identity;
		Part componentInParent = pAsteroid.gameObject.GetComponentInParent<Part>();
		if (isSecondary && componentInParent != null && componentInParent.mpb != null)
		{
			componentInParent.mpb.SetColor("_emissiveColor", secondaryColor);
		}
		if (debugGenTime)
		{
			Debug.Log("AsteroidCreate: " + (Time.realtimeSinceStartup - realtimeSinceStartup).ToString("F3"));
		}
		return pAsteroid;
	}

	private PAsteroid CreatePAsteroid(float radius, List<PQSMod> modArray, Func<Transform, float> rangefinder, Callback onComplete, bool isSecondary, out Color secondaryColor)
	{
		Mesh mesh = null;
		Mesh mesh2 = null;
		Mesh convexMesh = null;
		mesh = CreateMeshVisual(visualSphere, radius, modArray, out var volume, out var highestPoint);
		if (colliderSphere != null)
		{
			mesh2 = ((!(colliderSphere == visualSphere)) ? CreateMeshCollider(colliderSphere, radius, modArray) : mesh);
		}
		if (convexSphere != null)
		{
			convexMesh = ((convexSphere == visualSphere) ? mesh : ((!(convexSphere == colliderSphere)) ? CreateMeshCollider(convexSphere, radius, modArray) : mesh2));
		}
		PAsteroid pAsteroid = new GameObject("Asteroid").AddComponent<PAsteroid>();
		pAsteroid.Setup(mesh, (!isSecondary || !(secondaryMaterial != null)) ? primaryMaterial : secondaryMaterial, visualLayer, visualTag, mesh2, colliderMaterial, colliderLayer, colliderTag, convexMesh, convexMaterial, convexLayer, convexTag, rangefinder, onComplete);
		if (isSecondary)
		{
			ColorHSV colorHSV = new ColorHSV(UnityEngine.Random.value, 0.684f, 0.992f, UnityEngine.Random.value);
			secondaryColor = colorHSV.ToColor();
			pAsteroid.SetMaterialColor("_emissiveColor", secondaryColor);
		}
		else
		{
			secondaryColor = default(Color);
		}
		pAsteroid.volume = volume;
		pAsteroid.highestPoint = highestPoint;
		if (Application.isPlaying)
		{
			paGenerated = pAsteroid;
		}
		return pAsteroid;
	}

	private Mesh CreateMeshVisual(SphereBaseSO sphere, float radius, List<PQSMod> modArray, out float volume, out float highestPoint)
	{
		Mod_OnSetup(modArray);
		GClass4.VertexBuildData vertexBuildData = new GClass4.VertexBuildData();
		float[] array = new float[sphere.vCount];
		Color[] array2 = new Color[sphere.vCount];
		volume = 4.18879f;
		highestPoint = radius;
		float num = 0f;
		for (int i = 0; i < sphere.vCount; i++)
		{
			vertexBuildData.vertHeight = radius;
			vertexBuildData.directionFromCenter = sphere.verts[i].position;
			vertexBuildData.vertColor = Color.white;
			Mod_Build(modArray, vertexBuildData);
			array[i] = (float)vertexBuildData.vertHeight;
			array2[i] = vertexBuildData.vertColor;
			num += array[i];
			if (array[i] > highestPoint)
			{
				highestPoint = array[i];
			}
		}
		if (num != 0f)
		{
			num /= (float)sphere.vCount;
			volume = GetSphereVolume(num);
		}
		return sphere.CreateMesh(array, array2, createUV: true);
	}

	private Mesh CreateMeshCollider(SphereBaseSO sphere, float radius, List<PQSMod> modArray)
	{
		Mod_OnSetup(modArray);
		GClass4.VertexBuildData vertexBuildData = new GClass4.VertexBuildData();
		float[] array = new float[sphere.vCount];
		for (int i = 0; i < sphere.vCount; i++)
		{
			vertexBuildData.vertHeight = radius;
			vertexBuildData.directionFromCenter = sphere.verts[i].position;
			Mod_Build(modArray, vertexBuildData);
			array[i] = (float)vertexBuildData.vertHeight;
		}
		return sphere.CreateMesh(array, null, createUV: false);
	}

	private float GetSphereVolume(float r)
	{
		return 4.1887903f * r * r * r;
	}

	private void Mod_OnSetup(List<PQSMod> mods)
	{
		int i = 0;
		for (int count = mods.Count; i < count; i++)
		{
			mods[i].OnSetup();
		}
	}

	private void Mod_Build(List<PQSMod> mods, GClass4.VertexBuildData vert)
	{
		int i = 0;
		for (int count = mods.Count; i < count; i++)
		{
			mods[i].OnVertexBuildHeight(vert);
		}
		int j = 0;
		for (int count2 = mods.Count; j < count2; j++)
		{
			mods[j].OnVertexBuild(vert);
		}
	}

	private float RangefinderGeneric(Transform trf)
	{
		return Vector3.Distance(trf.position, Camera.main.transform.position);
	}

	[ContextMenu("Update Wrappers")]
	private void UpdateWrappers()
	{
		if (mods == null)
		{
			mods = new List<ModWrapper>();
		}
		int count = mods.Count;
		while (count-- > 0)
		{
			if ((PQSMod)GetComponent(mods[count].name) == null)
			{
				mods.RemoveAt(count);
			}
		}
		PQSMod[] components = GetComponents<PQSMod>();
		int i = 0;
		for (int num = components.Length; i < num; i++)
		{
			PQSMod pQSMod = components[i];
			if (!HasWrapperOfTypeName(pQSMod.GetType().Name))
			{
				ModWrapper modWrapper = new ModWrapper();
				modWrapper.name = pQSMod.GetType().Name;
				mods.Add(modWrapper);
			}
		}
	}

	private bool HasWrapperOfTypeName(string name)
	{
		int num = 0;
		int count = mods.Count;
		while (true)
		{
			if (num < count)
			{
				if (mods[num].name == name)
				{
					break;
				}
				num++;
				continue;
			}
			return false;
		}
		return true;
	}

	[ContextMenu("Rebuild")]
	private void Rebuild()
	{
		if (!(paGenerated == null) && Application.isPlaying)
		{
			UnityEngine.Object.Destroy(paGenerated.gameObject);
			Generate(seed, radius, base.transform.parent, RangefinderGeneric, delegate
			{
			});
		}
	}

	[ContextMenu("New Seed")]
	private void NewSeed()
	{
		seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
	}
}
