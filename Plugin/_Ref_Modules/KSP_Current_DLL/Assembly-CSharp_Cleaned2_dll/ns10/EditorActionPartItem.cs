using TMPro;
using UnityEngine.EventSystems;
using ns11;
using ns2;
using ns9;

namespace ns10;

public class EditorActionPartItem : UISelectableGridLayoutGroupItem, IEventSystemHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler
{
	public TextMeshProUGUI text;

	public UIButtonToggle invertButton;

	public UIButtonToggle modeButton;

	private TooltipController_Text invertTooltip;

	private TooltipController_Text modeTooltip;

	private string[] invertTooltipText;

	private string[] modeTooltipText;

	public BaseAction evt { get; set; }

	public BaseAxisField axisField { get; set; }

	public EditorActionPartSelector selector { get; set; }

	public int selectedGroup { get; set; }

	public int groupOverride { get; set; }

	public uint SelectedControllerId { get; set; }

	public EditorActionGroupType selectedGroupType { get; set; }

	public bool addToGroup { get; set; }

	private static KSPAxisGroup GetAxisIncremental(BaseAxisField axisField, int groupOverride)
	{
		if (groupOverride > 0)
		{
			return axisField.overrideIncremental[groupOverride - 1];
		}
		return axisField.axisIncremental;
	}

	private static KSPAxisGroup GetAxisInverted(BaseAxisField axisField, int groupOverride)
	{
		if (groupOverride > 0)
		{
			return axisField.overrideInverted[groupOverride - 1];
		}
		return axisField.axisInverted;
	}

	private static void SetAxisIncremental(BaseAxisField axisField, int groupOverride, KSPAxisGroup value)
	{
		if (groupOverride > 0)
		{
			axisField.overrideIncremental[groupOverride - 1] = value;
		}
		else
		{
			axisField.axisIncremental = value;
		}
	}

	private static void SetAxisInverted(BaseAxisField axisField, int groupOverride, KSPAxisGroup value)
	{
		if (groupOverride > 0)
		{
			axisField.overrideInverted[groupOverride - 1] = value;
		}
		else
		{
			axisField.axisInverted = value;
		}
	}

	public void Setup(string text, KSPActionGroup selectedGroup, int groupOverride, EditorActionPartSelector selector, BaseAction evt, bool addToGroup)
	{
		this.text.text = text;
		this.selectedGroup = (int)selectedGroup;
		this.groupOverride = groupOverride;
		SelectedControllerId = 0u;
		selectedGroupType = EditorActionGroupType.Action;
		this.selector = selector;
		this.evt = evt;
		axisField = null;
		this.addToGroup = addToGroup;
		if (invertButton != null)
		{
			invertButton.gameObject.SetActive(value: false);
		}
		if (modeButton != null)
		{
			modeButton.gameObject.SetActive(value: false);
		}
	}

	public void Setup(string text, KSPAxisGroup selectedGroup, int groupOverride, EditorActionPartSelector selector, BaseAxisField axisField, bool addToGroup)
	{
		this.text.text = text;
		this.selectedGroup = (int)selectedGroup;
		this.groupOverride = groupOverride;
		SelectedControllerId = 0u;
		selectedGroupType = EditorActionGroupType.Axis;
		this.selector = selector;
		evt = null;
		this.axisField = axisField;
		this.addToGroup = addToGroup;
		if (invertButton != null)
		{
			invertButton.gameObject.SetActive(value: true);
			invertButton.onToggle.AddListener(ToggleInvert);
			invertButton.SetState((GetAxisInverted(axisField, groupOverride) & selectedGroup) != 0);
			invertTooltipText = new string[2];
			invertTooltipText[0] = Localizer.Format("#autoLOC_6013020", "#autoLOC_6013021");
			invertTooltipText[1] = Localizer.Format("#autoLOC_6013020", "#autoLOC_6013022");
			invertTooltip = invertButton.gameObject.GetComponent<TooltipController_Text>();
			ToggleInvert();
		}
		if (modeButton != null)
		{
			if (selectedGroup != KSPAxisGroup.MainThrottle)
			{
				modeButton.gameObject.SetActive(value: true);
				modeButton.onToggle.AddListener(ToggleMode);
				modeButton.SetState((GetAxisIncremental(axisField, groupOverride) & selectedGroup) != 0);
				modeTooltipText = new string[2];
				modeTooltipText[0] = Localizer.Format("#autoLOC_6013023", Localizer.Format("#autoLOC_6013024", "#autoLOC_6013026"), "#autoLOC_6013027");
				modeTooltipText[1] = Localizer.Format("#autoLOC_6013023", Localizer.Format("#autoLOC_6013025", "#autoLOC_6013027"), "#autoLOC_6013026");
				modeTooltip = modeButton.gameObject.GetComponent<TooltipController_Text>();
				ToggleMode();
			}
			else
			{
				modeButton.gameObject.SetActive(value: false);
			}
		}
	}

	public void Setup(string text, uint selectedControllerId, EditorActionPartSelector selector, BaseAxisField axisField, BaseAction evt, bool addToGroup)
	{
		this.text.text = text;
		selectedGroup = 0;
		SelectedControllerId = selectedControllerId;
		selectedGroupType = EditorActionGroupType.Controller;
		this.selector = selector;
		this.evt = evt;
		this.axisField = axisField;
		this.addToGroup = addToGroup;
		if (invertButton != null)
		{
			invertButton.gameObject.SetActive(value: false);
		}
		if (modeButton != null)
		{
			modeButton.gameObject.SetActive(value: false);
		}
	}

	public void Setup(string text, uint selectedControllerId, EditorActionPartSelector selector, BaseAxisField axisField, bool addToGroup)
	{
		this.text.text = text;
		selectedGroup = 0;
		SelectedControllerId = selectedControllerId;
		selectedGroupType = EditorActionGroupType.Controller;
		this.selector = selector;
		evt = null;
		this.axisField = axisField;
		this.addToGroup = addToGroup;
		if (invertButton != null)
		{
			invertButton.gameObject.SetActive(value: false);
		}
		if (modeButton != null)
		{
			modeButton.gameObject.SetActive(value: false);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		selector.HighLight(highLight: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		selector.HighLight(highLight: false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ItemClicked();
	}

	private void ToggleInvert()
	{
		KSPAxisGroup axisInverted = GetAxisInverted(axisField, groupOverride);
		if (invertButton.state)
		{
			axisInverted = (KSPAxisGroup)((int)axisInverted | selectedGroup);
			invertTooltip.textString = invertTooltipText[1];
		}
		else
		{
			axisInverted = (KSPAxisGroup)((int)axisInverted & ~selectedGroup);
			invertTooltip.textString = invertTooltipText[0];
		}
		SetAxisInverted(axisField, groupOverride, axisInverted);
		Part part = selector.part;
		int num = part.Modules.IndexOf(axisField.module);
		int count = part.symmetryCounterparts.Count;
		while (count-- > 0)
		{
			PartModule partModule = part.symmetryCounterparts[count].Modules[num];
			if (!(partModule == null) && partModule.Fields[axisField.name] is BaseAxisField baseAxisField)
			{
				SetAxisInverted(baseAxisField, groupOverride, axisInverted);
			}
		}
	}

	private void ToggleMode()
	{
		KSPAxisGroup axisIncremental = GetAxisIncremental(axisField, groupOverride);
		if (modeButton.state)
		{
			axisIncremental = (KSPAxisGroup)((int)axisIncremental | selectedGroup);
			modeTooltip.textString = modeTooltipText[1];
		}
		else
		{
			axisIncremental = (KSPAxisGroup)((int)axisIncremental & ~selectedGroup);
			modeTooltip.textString = modeTooltipText[0];
		}
		SetAxisIncremental(axisField, groupOverride, axisIncremental);
		Part part = selector.part;
		int num = part.Modules.IndexOf(axisField.module);
		int count = part.symmetryCounterparts.Count;
		while (count-- > 0)
		{
			PartModule partModule = part.symmetryCounterparts[count].Modules[num];
			if (!(partModule == null) && partModule.Fields[axisField.name] is BaseAxisField baseAxisField)
			{
				SetAxisIncremental(baseAxisField, groupOverride, axisIncremental);
			}
		}
	}

	public void OnSubmit(BaseEventData eventData)
	{
		ItemClicked();
	}

	private void ItemClicked()
	{
		if ((selectedGroupType == EditorActionGroupType.Controller && SelectedControllerId == 0) || (axisField == null && evt == null))
		{
			selector.Select();
			return;
		}
		if (addToGroup)
		{
			EditorActionGroups.Instance.AddActionToGroup(this);
			return;
		}
		EditorActionGroups.Instance.RemoveActionFromGroup(this);
		selector.HighLight(highLight: false);
	}
}
