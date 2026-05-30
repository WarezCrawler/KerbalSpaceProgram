using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ns2;

public class UIHoverSlidePanel : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public RectTransform panel;

	public Vector2 positionNormal;

	public Vector2 positionHovered;

	public float sharpness = 1f;

	public bool locked;

	protected bool pointOver;

	protected Coroutine coroutine;

	public Callback<Vector2> OnUpdatePosition = delegate
	{
	};

	protected void Reset()
	{
		panel = GetComponent<RectTransform>();
	}

	protected void Start()
	{
		pointOver = false;
		panel.anchoredPosition = positionNormal;
		OnUpdatePosition(panel.anchoredPosition);
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		pointOver = true;
		if (!locked && coroutine == null)
		{
			coroutine = StartCoroutine(MoveToState(0f, newState: true));
		}
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		pointOver = false;
		if (!locked && coroutine == null)
		{
			coroutine = StartCoroutine(MoveToState(0.1f, newState: false));
		}
	}

	protected IEnumerator MoveToState(float delay, bool newState)
	{
		bool state = newState;
		Vector2 vTgt = ((!newState) ? positionNormal : positionHovered);
		yield return new WaitForSeconds(delay);
		if (!locked && state != pointOver)
		{
			coroutine = null;
			yield break;
		}
		while ((panel.anchoredPosition - vTgt).sqrMagnitude > 0.0001f)
		{
			panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, vTgt, sharpness * Time.deltaTime);
			OnUpdatePosition(panel.anchoredPosition);
			yield return null;
		}
		panel.anchoredPosition = vTgt;
		OnUpdatePosition(panel.anchoredPosition);
		if (!locked && state != pointOver)
		{
			coroutine = StartCoroutine(MoveToState(0.5f, pointOver));
		}
		else
		{
			coroutine = null;
		}
	}
}
