using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test_particles : MonoBehaviour
{
	public float power = 0.5f;

	public float powerVariation;

	public float throttleSpeed = 2f;

	private List<float> flameInitSizeValuesNewSystem;

	private List<float> flameInitLifeValuesNewSystem;

	private List<float> smokeInitSizeValuesNewSystem;

	private List<float> smokeInitLifeValuesNewSystem;

	public List<ParticleSystem> flameoutList;

	public List<ParticleSystem> smokeList;

	public List<ParticleSystem> flameList;

	public bool flameEnabled = true;

	public bool smokeEnabled = true;

	public Text throttleText;

	public Text smokeText;

	public Text flameText;

	public Camera testcamera;

	public float camMoveSpeed = 15f;

	public float camRotateSpeed = 2f;

	private Vector3 dragOrigin;

	private Vector3 camMove;

	private void Start()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		flameInitSizeValuesNewSystem = new List<float>();
		flameInitLifeValuesNewSystem = new List<float>();
		smokeInitSizeValuesNewSystem = new List<float>();
		smokeInitLifeValuesNewSystem = new List<float>();
		MinMaxCurve val;
		for (int i = 0; i < flameList.Count; i++)
		{
			MainModule main = flameList[i].main;
			List<float> list = flameInitSizeValuesNewSystem;
			val = ((MainModule)(ref main)).startSize;
			list.Add(((MinMaxCurve)(ref val)).constantMax);
			List<float> list2 = flameInitLifeValuesNewSystem;
			val = ((MainModule)(ref main)).startLifetime;
			list2.Add(((MinMaxCurve)(ref val)).constantMax);
		}
		for (int j = 0; j < smokeList.Count; j++)
		{
			MainModule main2 = smokeList[j].main;
			List<float> list3 = smokeInitSizeValuesNewSystem;
			val = ((MainModule)(ref main2)).startSize;
			list3.Add(((MinMaxCurve)(ref val)).constantMax);
			List<float> list4 = smokeInitLifeValuesNewSystem;
			val = ((MainModule)(ref main2)).startLifetime;
			list4.Add(((MinMaxCurve)(ref val)).constantMax);
		}
		for (int k = 0; k < flameoutList.Count; k++)
		{
			flameoutList[k].Stop();
		}
		SetPower(power);
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			SetPower(power + Time.deltaTime / throttleSpeed);
		}
		else if (Input.GetKey(KeyCode.LeftControl))
		{
			SetPower(power - Time.deltaTime / throttleSpeed);
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			SetPower(0f);
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			SetPower(1f);
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			for (int i = 0; i < flameoutList.Count; i++)
			{
				ParticleSystem obj = flameoutList[i];
				obj.Stop();
				obj.Play();
			}
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			flameEnabled = !flameEnabled;
			SetPower(power);
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			smokeEnabled = !smokeEnabled;
			SetPower(power);
		}
		camMove = default(Vector3);
		if (Input.GetKey(KeyCode.W))
		{
			camMove.z += camMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.S))
		{
			camMove.z -= camMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.A))
		{
			camMove.x -= camMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D))
		{
			camMove.x += camMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			camMove.y += camMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.E))
		{
			camMove.y -= camMoveSpeed * Time.deltaTime;
		}
		MouseDrag();
		if (throttleText != null)
		{
			throttleText.text = $"Throttle: {power:0.00}";
		}
		if (smokeText != null)
		{
			smokeText.text = $"Smoke: {smokeEnabled}";
		}
		if (flameText != null)
		{
			flameText.text = $"Flame: {flameEnabled}";
		}
	}

	private void MouseDrag()
	{
		if (Input.GetMouseButton(1))
		{
			Camera.main.transform.Rotate(new Vector3((0f - Input.GetAxis("Mouse Y")) * camRotateSpeed, Input.GetAxis("Mouse X") * camRotateSpeed, 0f), Space.Self);
		}
	}

	private void LateUpdate()
	{
		if (testcamera != null)
		{
			testcamera.transform.Translate(camMove);
		}
	}

	public void SetPower(float pwr)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		power = Mathf.Clamp01(pwr);
		int i = 0;
		for (int count = flameList.Count; i < count; i++)
		{
			ParticleSystem val = flameList[i];
			if (flameEnabled)
			{
				MainModule main = val.main;
				MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
				((MinMaxCurve)(ref startLifetime)).constantMin = flameInitLifeValuesNewSystem[i] * power;
				((MinMaxCurve)(ref startLifetime)).constantMax = flameInitLifeValuesNewSystem[i] * power;
				((MinMaxCurve)(ref startLifetime)).mode = (ParticleSystemCurveMode)3;
				((MainModule)(ref main)).startLifetime = startLifetime;
				MinMaxCurve startSize = ((MainModule)(ref main)).startSize;
				((MinMaxCurve)(ref startSize)).constantMin = flameInitSizeValuesNewSystem[i] * power;
				((MinMaxCurve)(ref startSize)).constantMax = flameInitSizeValuesNewSystem[i] * power;
				((MinMaxCurve)(ref startSize)).mode = (ParticleSystemCurveMode)3;
				((MainModule)(ref main)).startSize = startSize;
				if (!val.isPlaying)
				{
					val.Play();
				}
			}
			else if (val.isPlaying)
			{
				val.Stop();
			}
		}
		int j = 0;
		for (int count2 = smokeList.Count; j < count2; j++)
		{
			ParticleSystem val = smokeList[j];
			if (smokeEnabled)
			{
				MainModule main2 = val.main;
				MinMaxCurve startLifetime2 = ((MainModule)(ref main2)).startLifetime;
				((MinMaxCurve)(ref startLifetime2)).constantMin = smokeInitLifeValuesNewSystem[j] * power - powerVariation * 0.5f * power;
				((MinMaxCurve)(ref startLifetime2)).constantMax = smokeInitLifeValuesNewSystem[j] * power + powerVariation * 0.5f * power;
				((MinMaxCurve)(ref startLifetime2)).mode = (ParticleSystemCurveMode)3;
				((MainModule)(ref main2)).startLifetime = startLifetime2;
				MinMaxCurve startSize2 = ((MainModule)(ref main2)).startSize;
				((MinMaxCurve)(ref startSize2)).constantMin = smokeInitSizeValuesNewSystem[j] * power - powerVariation * 0.5f * power;
				((MinMaxCurve)(ref startSize2)).constantMax = smokeInitSizeValuesNewSystem[j] * power + powerVariation * 0.5f * power;
				((MinMaxCurve)(ref startSize2)).mode = (ParticleSystemCurveMode)3;
				((MainModule)(ref main2)).startSize = startSize2;
				if (!val.isPlaying)
				{
					val.Play();
				}
			}
			else if (val.isPlaying)
			{
				val.Stop();
			}
		}
	}
}
