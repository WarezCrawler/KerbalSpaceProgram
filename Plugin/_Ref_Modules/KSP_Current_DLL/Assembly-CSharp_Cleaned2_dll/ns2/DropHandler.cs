using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ns2;

public class DropHandler : MonoBehaviour, IEventSystemHandler, IDropHandler
{
	[Serializable]
	public class DropEvent<PointerEventData> : UnityEvent<PointerEventData>
	{
	}

	public DropEvent<PointerEventData> onDrop = new DropEvent<PointerEventData>();

	public virtual void OnDrop(PointerEventData eventData)
	{
		onDrop.Invoke(eventData);
	}
}
