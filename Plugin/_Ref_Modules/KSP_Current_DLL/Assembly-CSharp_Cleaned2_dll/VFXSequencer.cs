using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSequencer : MonoBehaviour
{
	[Serializable]
	public class SequenceFX
	{
		public float startTime;

		public float startTimeVariation;

		public float scheduled;

		public ParticleSystem particleSystem;

		public AudioClip audioFx;

		public AudioSource audioSource;

		public float pitchVariance;

		public void Play()
		{
			if (audioSource != null && audioFx != null)
			{
				audioSource.volume = GameSettings.AMBIENCE_VOLUME;
				if (pitchVariance != 0f)
				{
					audioSource.pitch = 1f + UnityEngine.Random.Range(0f - pitchVariance, pitchVariance);
				}
				audioSource.PlayOneShot(audioFx);
			}
			if ((UnityEngine.Object)(object)particleSystem != null)
			{
				particleSystem.Play();
			}
		}

		public float GetFXDuration()
		{
			float num = 0f;
			if (audioFx != null && audioSource != null)
			{
				num = audioFx.length;
			}
			if ((UnityEngine.Object)(object)particleSystem != null)
			{
				num = getSubFXHierarchyDuration(particleSystem, num);
			}
			return num;
		}

		public float getSubFXHierarchyDuration(ParticleSystem ps, float d)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			MainModule main = ps.main;
			float a = d;
			MinMaxCurve val = ((MainModule)(ref main)).startDelay;
			float num = ((MinMaxCurve)(ref val)).constant + ((MainModule)(ref main)).duration;
			val = ((MainModule)(ref main)).startLifetime;
			d = Mathf.Max(a, (num + ((MinMaxCurve)(ref val)).constant) * ((MainModule)(ref main)).simulationSpeed);
			ParticleSystem[] componentsInChildren = ((Component)(object)ps).GetComponentsInChildren<ParticleSystem>(includeInactive: false);
			int num2 = componentsInChildren.Length;
			while (num2-- > 0)
			{
				MainModule main2 = componentsInChildren[num2].main;
				float a2 = d;
				val = ((MainModule)(ref main2)).startDelay;
				float num3 = ((MinMaxCurve)(ref val)).constant + ((MainModule)(ref main2)).duration;
				val = ((MainModule)(ref main2)).startLifetime;
				d = Mathf.Max(a2, (num3 + ((MinMaxCurve)(ref val)).constant) * ((MainModule)(ref main2)).simulationSpeed);
			}
			return d;
		}
	}

	public bool PlayOnStart;

	public bool SelfDestructOnComplete;

	public List<SequenceFX> FXList;

	private bool isPlaying;

	private bool isComplete;

	public bool IsPlaying => isPlaying;

	public bool IsComplete => isComplete;

	private void Start()
	{
		if (PlayOnStart)
		{
			StartCoroutine(playSequence(new List<SequenceFX>(FXList), delegate
			{
			}));
		}
	}

	[ContextMenu("Play")]
	public void Play()
	{
		StartCoroutine(playSequence(new List<SequenceFX>(FXList), delegate
		{
		}));
	}

	public void Play(Callback<VFXSequencer> onComplete)
	{
		StartCoroutine(playSequence(new List<SequenceFX>(FXList), onComplete));
	}

	private IEnumerator playSequence(List<SequenceFX> fxList, Callback<VFXSequencer> onComplete)
	{
		float t = Time.timeSinceLevelLoad;
		isPlaying = true;
		float tToComplete = t;
		for (int num = fxList.Count - 1; num >= 0; num--)
		{
			if (fxList[num].startTimeVariation != 0f)
			{
				fxList[num].scheduled = fxList[num].startTime + UnityEngine.Random.Range(0f - fxList[num].startTimeVariation, fxList[num].startTimeVariation);
			}
			else
			{
				fxList[num].scheduled = fxList[num].startTime;
			}
			tToComplete = Mathf.Max(tToComplete, t + fxList[num].scheduled + fxList[num].GetFXDuration());
		}
		while (fxList.Count > 0)
		{
			for (int num2 = fxList.Count - 1; num2 >= 0; num2--)
			{
				if (fxList[num2].scheduled <= Time.timeSinceLevelLoad - t)
				{
					fxList[num2].Play();
					fxList.RemoveAt(num2);
				}
			}
			yield return null;
		}
		yield return new WaitForSeconds(tToComplete - Time.timeSinceLevelLoad);
		isPlaying = false;
		isComplete = true;
		onComplete(this);
		if (SelfDestructOnComplete)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
