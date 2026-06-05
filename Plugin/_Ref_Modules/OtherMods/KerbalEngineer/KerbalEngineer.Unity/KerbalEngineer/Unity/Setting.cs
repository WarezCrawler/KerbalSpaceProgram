using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KerbalEngineer.Unity;

public class Setting : MonoBehaviour
{
	[SerializeField]
	private Text label = null;

	[SerializeField]
	private Transform buttonsTransform = null;

	[SerializeField]
	private GameObject settingButtonPrefab = null;

	[SerializeField]
	private GameObject settingTogglePrefab = null;

	private Action onUpdate;

	public Button AddButton(string text, float width, UnityAction onClick)
	{
		Button result = null;
		if ((Object)(object)settingButtonPrefab != (Object)null)
		{
			GameObject val = Object.Instantiate<GameObject>(settingButtonPrefab);
			if ((Object)(object)val != (Object)null)
			{
				result = val.GetComponent<Button>();
				SetParentTransform(val, buttonsTransform);
				SetWidth(val, width);
				SetText(val, text);
				SetButton(val, onClick);
			}
		}
		return result;
	}

	public Toggle AddToggle(string text, float width, UnityAction<bool> onValueChanged)
	{
		Toggle result = null;
		if ((Object)(object)settingTogglePrefab != (Object)null)
		{
			GameObject val = Object.Instantiate<GameObject>(settingTogglePrefab);
			if ((Object)(object)val != (Object)null)
			{
				result = val.GetComponent<Toggle>();
				SetParentTransform(val, buttonsTransform);
				SetWidth(val, width);
				SetText(val, text);
				SetToggle(val, onValueChanged);
			}
		}
		return result;
	}

	public void AddUpdateHandler(Action onUpdate)
	{
		this.onUpdate = onUpdate;
	}

	public void SetLabel(string text)
	{
		if ((Object)(object)label != (Object)null)
		{
			label.text = text;
		}
	}

	protected virtual void Update()
	{
		if (onUpdate != null)
		{
			onUpdate();
		}
	}

	private static void SetButton(GameObject buttonObject, UnityAction onClick)
	{
		if ((Object)(object)buttonObject != (Object)null)
		{
			Button component = buttonObject.GetComponent<Button>();
			if ((Object)(object)component != (Object)null)
			{
				((UnityEvent)component.onClick).AddListener(onClick);
			}
		}
	}

	private static void SetParentTransform(GameObject childObject, Transform parentTransform)
	{
		if ((Object)(object)childObject != (Object)null && (Object)(object)parentTransform != (Object)null)
		{
			childObject.transform.SetParent(parentTransform, false);
		}
	}

	private static void SetText(GameObject parentObject, string text)
	{
		if ((Object)(object)parentObject != (Object)null)
		{
			Text componentInChildren = parentObject.GetComponentInChildren<Text>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				componentInChildren.text = text;
			}
		}
	}

	private static void SetToggle(GameObject toggleObject, UnityAction<bool> onValueChanged)
	{
		if ((Object)(object)toggleObject != (Object)null)
		{
			Toggle component = toggleObject.GetComponent<Toggle>();
			if ((Object)(object)component != (Object)null)
			{
				((UnityEvent<bool>)(object)component.onValueChanged).AddListener(onValueChanged);
			}
		}
	}

	private static void SetWidth(GameObject parentObject, float width)
	{
		if (!((Object)(object)parentObject != (Object)null))
		{
			return;
		}
		LayoutElement component = parentObject.GetComponent<LayoutElement>();
		if ((Object)(object)component != (Object)null)
		{
			if (width > 0f)
			{
				component.preferredWidth = width;
			}
			else
			{
				component.flexibleWidth = 1f;
			}
		}
	}
}
