using UnityEngine;
using UnityEngine.UI;

namespace Expansions.Missions.Editor;

[MEGUI_ColorPicker]
public class MEGUIParameterColorPicker : MEGUIParameter
{
	public ColorPicker colorPicker;

	public Canvas colorControls;

	public Button colorPickerButton;

	public Button modeButton;

	public GameObject[] rgbModeSliders;

	public GameObject[] hueModeSliders;

	protected GameObject blocker;

	public override bool IsInteractable
	{
		get
		{
			return colorPickerButton.interactable;
		}
		set
		{
			colorPickerButton.interactable = value;
		}
	}

	public Color FieldValue
	{
		get
		{
			return (Color)field.GetValue();
		}
		set
		{
			MissionEditorHistory.PushUndoAction(this, OnHistoryValueChange);
			field.SetValue(value);
		}
	}

	protected override void Setup(string name)
	{
		title.text = name;
		colorPicker.CurrentColor = FieldValue;
		colorPickerButton.onClick.AddListener(OnPickColorButton);
		modeButton.onClick.AddListener(OnModeButton);
		colorPicker.onValueChanged.AddListener(OnColorChange);
	}

	protected override void ResetDefaultValue(string value)
	{
		Color color = Color.white;
		if (ColorUtility.TryParseHtmlString(value, out color))
		{
			FieldValue = color;
		}
	}

	public override void RefreshUI()
	{
		colorPicker.CurrentColor = FieldValue;
	}

	private void OnPickColorButton()
	{
		colorControls.enabled = !colorControls.enabled;
		if (colorControls.enabled)
		{
			blocker = new GameObject("blocker", typeof(Button), typeof(Image));
			blocker.GetComponent<Button>().onClick.AddListener(OnPickColorButton);
			blocker.GetComponent<Image>().color = Color.clear;
			RectTransform obj = blocker.transform as RectTransform;
			obj.SetParent(GetComponentInParent<Canvas>().transform, worldPositionStays: false);
			obj.anchorMin = Vector2.zero;
			obj.anchorMax = Vector2.one;
			obj.offsetMin = Vector2.zero;
			obj.offsetMax = Vector2.zero;
			obj.anchoredPosition = Vector3.zero;
		}
		else
		{
			Object.Destroy(blocker);
		}
	}

	private void OnModeButton()
	{
		int i = 0;
		for (int num = rgbModeSliders.Length; i < num; i++)
		{
			rgbModeSliders[i].SetActive(!rgbModeSliders[i].activeSelf);
		}
		int j = 0;
		for (int num2 = hueModeSliders.Length; j < num2; j++)
		{
			hueModeSliders[j].SetActive(!hueModeSliders[j].activeSelf);
		}
	}

	private void OnColorChange(Color color)
	{
		FieldValue = color;
	}

	public override ConfigNode GetState()
	{
		ConfigNode configNode = new ConfigNode();
		configNode.AddValue("pinned", isPinned);
		configNode.AddValue("value", "#" + ColorUtility.ToHtmlStringRGB((Color)field.GetValue()));
		return configNode;
	}

	public void OnHistoryValueChange(ConfigNode data, HistoryType type)
	{
		Color value = Color.white;
		if (data.TryGetValue("value", ref value))
		{
			colorPicker.CurrentColor = value;
		}
	}
}
