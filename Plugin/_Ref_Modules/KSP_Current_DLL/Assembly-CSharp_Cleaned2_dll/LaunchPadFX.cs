using UnityEngine;

public class LaunchPadFX : MonoBehaviour
{
	[SerializeField]
	protected ParticleSystem[] ps;

	[SerializeField]
	protected Material smokeParticleMaterial;

	[SerializeField]
	[Range(0f, 1f)]
	protected float fxScale;

	[SerializeField]
	private float maxFX = 5f;

	private float totalFX;

	private void Start()
	{
		int num = ps.Length;
		while (num-- > 0)
		{
			FloatingOrigin.RegisterParticleSystem(ps[num]);
		}
	}

	private void OnDestroy()
	{
		int num = ps.Length;
		while (num-- > 0)
		{
			FloatingOrigin.UnregisterParticleSystem(ps[num]);
		}
	}

	public void AddFX(float fx)
	{
		totalFX = Mathf.Clamp(totalFX + fx, 0f, maxFX);
		fxScale = totalFX / maxFX;
	}

	private void LateUpdate()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		int num = ps.Length;
		while (num-- > 0)
		{
			if (fxScale > 0f)
			{
				MainModule main = ps[num].main;
				if (!ps[num].isPlaying)
				{
					ps[num].Play();
				}
				EmissionModule emission = ps[num].emission;
				if (!((EmissionModule)(ref emission)).enabled)
				{
					EmissionModule emission2 = ps[num].emission;
					((EmissionModule)(ref emission2)).enabled = true;
				}
				MinMaxGradient startColor = ((MainModule)(ref main)).startColor;
				((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(((MinMaxGradient)(ref startColor)).color.smethod_0(Mathf.Lerp(0f, 0.5f, fxScale)));
			}
			if (fxScale == 0f)
			{
				EmissionModule emission3 = ps[num].emission;
				((EmissionModule)(ref emission3)).enabled = false;
			}
		}
		totalFX = 0f;
		fxScale = 0f;
	}
}
