using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KerbalEngineer.Unity.UI;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class Window : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler
{
	[SerializeField]
	private Text title = null;

	[SerializeField]
	private Transform content = null;

	private Vector2 beginMousePosition;

	private Vector3 beginWindowPosition;

	private CanvasGroup canvasGroup;

	private RectTransform rectTransform;

	private IEnumerator scaleFadeCoroutine;

	public Transform Content => content;

	public RectTransform RectTransform => rectTransform;

	public void OnBeginDrag(PointerEventData eventData)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)rectTransform == (Object)null))
		{
			beginMousePosition = eventData.position;
			beginWindowPosition = ((Transform)rectTransform).position;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rectTransform != (Object)null)
		{
			((Transform)rectTransform).position = beginWindowPosition + Vector2.op_Implicit(eventData.position - beginMousePosition);
		}
	}

	public void AddToContent(GameObject childObject)
	{
		if ((Object)(object)content != (Object)null && (Object)(object)childObject != (Object)null)
		{
			childObject.transform.SetParent(content, false);
		}
	}

	public void Close()
	{
		ScaleFade(1f, 0f, delegate
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		});
	}

	public void SetTitle(string title)
	{
		if ((Object)(object)this.title != (Object)null)
		{
			this.title.text = title;
		}
	}

	public void SetWidth(float width)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rectTransform != (Object)null)
		{
			Vector2 sizeDelta = rectTransform.sizeDelta;
			sizeDelta.x = width;
			rectTransform.sizeDelta = sizeDelta;
		}
	}

	protected virtual void Awake()
	{
		rectTransform = ((Component)this).GetComponent<RectTransform>();
		canvasGroup = ((Component)this).GetComponent<CanvasGroup>();
	}

	protected virtual void OnEnable()
	{
		ScaleFade(0f, 1f, null);
	}

	private void ScaleFade(float from, float to, Action callback)
	{
		if (scaleFadeCoroutine != null)
		{
			((MonoBehaviour)this).StopCoroutine(scaleFadeCoroutine);
		}
		scaleFadeCoroutine = ScaleFadeCoroutine(from, to, callback);
		((MonoBehaviour)this).StartCoroutine(scaleFadeCoroutine);
	}

	private IEnumerator ScaleFadeCoroutine(float from, float to, Action callback)
	{
		float progress = 0f;
		while (progress <= 1f)
		{
			progress += Time.deltaTime / 0.2f;
			float value = Mathf.Lerp(from, to, progress);
			((Component)this).transform.localScale = Vector3.one * value;
			if ((Object)(object)canvasGroup != (Object)null)
			{
				canvasGroup.alpha = Mathf.Clamp01(value);
			}
			yield return null;
		}
		callback?.Invoke();
		scaleFadeCoroutine = null;
	}
}
