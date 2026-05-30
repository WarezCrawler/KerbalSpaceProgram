using System;
using UnityEngine;

public class PAsteroid : MonoBehaviour
{
	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private Mesh visualMesh;

	private Renderer visualRenderer;

	[SerializeField]
	private GameObject colliderObject;

	[SerializeField]
	private Mesh colliderMesh;

	private AsteroidCollider ac;

	private Part partComponent;

	[SerializeField]
	private GameObject convexObject;

	[SerializeField]
	private Mesh convexColliderMesh;

	[SerializeField]
	private int genTime;

	public float volume;

	public float highestPoint;

	[SerializeField]
	private float maxRange = 2500f;

	[SerializeField]
	private float minRange;

	private Callback onGenComplete;

	public void Awake()
	{
		if (genTime != 0)
		{
			visualMesh = null;
			colliderMesh = null;
			convexColliderMesh = null;
		}
	}

	public void Setup(Mesh visualMesh, Material visualMaterial, string visualLayer, string visualTag, Mesh colliderMesh, PhysicMaterial colliderMaterial, string colliderLayer, string colliderTag, Mesh convexMesh, PhysicMaterial convexMaterial, string convexLayer, string convexTag, Func<Transform, float> rangefinder, Callback onGenComplete)
	{
		this.visualMesh = visualMesh;
		this.colliderMesh = colliderMesh;
		convexColliderMesh = convexMesh;
		this.onGenComplete = onGenComplete;
		CreateVisual(visualMesh, visualMaterial, visualLayer, visualTag);
		CreateCollider(convexMesh, colliderMaterial, colliderLayer, colliderTag, rangefinder);
		genTime = Time.frameCount;
	}

	private void CreateVisual(Mesh visualMesh, Material visualMaterial, string visualLayer, string visualTag)
	{
		if (!(visualMesh == null))
		{
			visualObject = new GameObject("Visual");
			visualObject.transform.NestToParent(base.transform);
			visualObject.layer = LayerMask.NameToLayer(visualLayer);
			visualObject.tag = visualTag;
			visualObject.AddComponent<MeshFilter>().sharedMesh = visualMesh;
			visualObject.AddComponent<MeshRenderer>().sharedMaterial = visualMaterial;
		}
	}

	private void CreateCollider(Mesh colliderMesh, PhysicMaterial colliderMaterial, string colliderLayer, string colliderTag, Func<Transform, float> rangefinder)
	{
		if (!(colliderMesh == null))
		{
			colliderObject = new GameObject("Collider");
			colliderObject.transform.NestToParent(base.transform);
			colliderObject.layer = LayerMask.NameToLayer(colliderLayer);
			colliderObject.tag = colliderTag;
			ac = colliderObject.AddComponent<AsteroidCollider>();
			ac.Setup(this, colliderMesh, Vector3.zero, rangefinder, maxRange, minRange, onACSetupComplete);
		}
	}

	public void SetupPartParameters()
	{
		partComponent = base.gameObject.transform.parent.GetComponent<Part>();
		switch (DiscoveryInfo.GetObjectClass(partComponent.vessel.DiscoveryInfo.size.Value))
		{
		default:
			partComponent.explosionPotential = 0.75f;
			partComponent.maxTemp = 4000.0;
			break;
		case UntrackedObjectClass.const_0:
			partComponent.explosionPotential = 0.15f;
			partComponent.maxTemp = 2000.0;
			break;
		case UntrackedObjectClass.const_1:
			partComponent.explosionPotential = 0.3f;
			partComponent.maxTemp = 2500.0;
			break;
		case UntrackedObjectClass.const_2:
			partComponent.explosionPotential = 0.45f;
			partComponent.maxTemp = 2750.0;
			break;
		case UntrackedObjectClass.const_3:
			partComponent.explosionPotential = 0.6f;
			partComponent.maxTemp = 3000.0;
			break;
		case UntrackedObjectClass.const_4:
			partComponent.explosionPotential = 0.75f;
			partComponent.maxTemp = 4000.0;
			break;
		}
		partComponent.asteroidCollider = ac;
		partComponent.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
	}

	private void CreateConvexCollider(Mesh convexMesh, PhysicMaterial convexMaterial, string convexLayer, string convexTag)
	{
		if (!(convexMesh == null))
		{
			convexObject = new GameObject("ColliderConvex");
			convexObject.transform.NestToParent(base.transform);
			convexObject.layer = LayerMask.NameToLayer(convexLayer);
			convexObject.tag = convexTag;
			MeshCollider meshCollider = convexObject.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = convexMesh;
			meshCollider.sharedMaterial = convexMaterial;
			meshCollider.convex = true;
		}
	}

	private void onACSetupComplete()
	{
		if (convexObject != null)
		{
			convexObject.SetActive(value: false);
		}
		onGenComplete();
	}

	private void OnDestroy()
	{
		if (visualMesh != null)
		{
			UnityEngine.Object.Destroy(visualMesh);
		}
		if (colliderMesh != null)
		{
			UnityEngine.Object.Destroy(colliderMesh);
		}
		if (convexColliderMesh != null)
		{
			UnityEngine.Object.Destroy(convexColliderMesh);
		}
	}

	public void SetMaterialColor(string name, Color value)
	{
		if (!(visualObject == null))
		{
			Renderer componentCached = visualObject.GetComponentCached(ref visualRenderer);
			if (componentCached != null)
			{
				componentCached.SetupProperties(new ColorMaterialProperty(name, value));
			}
		}
	}
}
