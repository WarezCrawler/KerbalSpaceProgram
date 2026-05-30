using UnityEngine;

public class FXFadeByPressure : MonoBehaviour
{
	private float alphaNewSystem;

	public float maxAlpha = 0.9f;

	public float airDensity = 1f;

	public float fadeStartDns = 0.05089431f;

	public float fadeEndDns = 0.001362253f;

	public ParticleSystem particleSystem;

	private ParticleSystemRenderer particleSystemRenderer;

	private Color[] modifiedColors;

	private void Start()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		particleSystem = base.transform.GetComponent<ParticleSystem>();
		particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
		ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
		((ColorOverLifetimeModule)(ref colorOverLifetime)).enabled = true;
		MinMaxGradient color = ((ColorOverLifetimeModule)(ref colorOverLifetime)).color;
		alphaNewSystem = ((MinMaxGradient)(ref color)).color.a;
	}

	private void Update()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)particleSystem == null) && HighLogic.LoadedSceneIsFlight && FlightGlobals.ready && !FlightDriver.Pause)
		{
			airDensity = Mathf.InverseLerp(fadeEndDns, fadeStartDns, (float)FlightGlobals.ship_dns);
			if (airDensity > 0.001f)
			{
				ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
				MinMaxGradient color = ((ColorOverLifetimeModule)(ref colorOverLifetime)).color;
				Color color2 = ((MinMaxGradient)(ref color)).color;
				color2.a = Mathf.Lerp(0f, alphaNewSystem, airDensity);
				((Renderer)(object)particleSystemRenderer).enabled = true;
			}
			else
			{
				((Renderer)(object)particleSystemRenderer).enabled = false;
			}
		}
	}
}
