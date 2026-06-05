using UnityEngine;
using UnityEngine.EventSystems;

namespace KerbalEngineer.Unity;

public class PointerHoverDetector : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public bool IsPointerHovering { get; private set; }

	public void OnPointerEnter(PointerEventData eventData)
	{
		IsPointerHovering = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		IsPointerHovering = false;
	}
}
