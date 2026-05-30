using UnityEngine;

public class KerbalVisorGlossiness : MonoBehaviour
{
	private Material visorMaterial;

	private void Start()
	{
		visorMaterial = base.gameObject.GetComponent<SkinnedMeshRenderer>().material;
		visorMaterial.SetFloat("_Glossiness", 0.542f);
	}
}
