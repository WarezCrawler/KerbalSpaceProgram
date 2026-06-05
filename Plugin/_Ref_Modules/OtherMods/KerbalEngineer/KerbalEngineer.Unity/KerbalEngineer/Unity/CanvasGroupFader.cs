using System;
using System.Collections;
using UnityEngine;

namespace KerbalEngineer.Unity;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
	private CanvasGroup canvasGroup;

	private IEnumerator fadeCoroutine;

	public bool IsFading => fadeCoroutine != null;

	public void FadeTo(float alpha, float duration, Action callback = null)
	{
		if (!((Object)(object)canvasGroup == (Object)null))
		{
			Fade(canvasGroup.alpha, alpha, duration, callback);
		}
	}

	public void SetAlpha(float alpha)
	{
		if (!((Object)(object)canvasGroup == (Object)null))
		{
			alpha = Mathf.Clamp01(alpha);
			canvasGroup.alpha = alpha;
		}
	}

	protected virtual void Awake()
	{
		canvasGroup = ((Component)this).GetComponent<CanvasGroup>();
	}

	private void Fade(float from, float to, float duration, Action callback)
	{
		if (fadeCoroutine != null)
		{
			((MonoBehaviour)this).StopCoroutine(fadeCoroutine);
		}
		fadeCoroutine = FadeCoroutine(from, to, duration, callback);
		((MonoBehaviour)this).StartCoroutine(fadeCoroutine);
	}

	private IEnumerator FadeCoroutine(float from, float to, float duration, Action callback)
	{
		yield return (object)new WaitForEndOfFrame();
		float progress = 0f;
		while (progress <= 1f)
		{
			progress += Time.deltaTime / duration;
			SetAlpha(Mathf.Lerp(from, to, progress));
			yield return null;
		}
		callback?.Invoke();
		fadeCoroutine = null;
	}
}
