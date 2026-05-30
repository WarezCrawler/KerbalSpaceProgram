using System;
using Expansions;
using Expansions.Missions;
using Expansions.Serenity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ns11;

namespace ns2;

public class CrewListItem : MonoBehaviour
{
	[Serializable]
	public class ClickEvent<ButtonTypes, CrewListItem> : UnityEvent<ButtonTypes, CrewListItem>
	{
	}

	public enum ButtonTypes
	{
		const_0,
		const_1,
		X2
	}

	public enum KerbalTypes
	{
		AVAILABLE,
		BADASS,
		const_2,
		RECRUIT,
		TOURIST
	}

	private bool mouseoverEnabled = true;

	public UIStateButton button;

	[SerializeField]
	private Button suitVariantBtn;

	public TextMeshProUGUI kerbalName;

	[SerializeField]
	public RawImage kerbalSprite;

	public TextMeshProUGUI xp_trait;

	[SerializeField]
	private Slider xp_slider;

	[SerializeField]
	private UIStateImage xp_levels;

	[SerializeField]
	private Slider slider_courage;

	[SerializeField]
	private Slider slider_stupidity;

	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private TooltipController_CrewAC tooltipController;

	[SerializeField]
	private UIHoverPanel hoverPanel;

	private ProtoCrewMember crew;

	private bool setup;

	private bool over;

	public bool isEmpty;

	public uint pUid;

	private KerbalTypes lastType;

	public ClickEvent<ButtonTypes, CrewListItem> onClick = new ClickEvent<ButtonTypes, CrewListItem>();

	public bool MouseoverEnabled
	{
		get
		{
			return mouseoverEnabled;
		}
		set
		{
			mouseoverEnabled = value;
			if (hoverPanel != null)
			{
				if (value)
				{
					hoverPanel.EnableHover();
				}
				else
				{
					hoverPanel.DisableHover();
				}
			}
		}
	}

	private void Awake()
	{
		if (button != null)
		{
			button.onClick.AddListener(delegate
			{
				onClick.Invoke((!(button.currentState == "X")) ? ButtonTypes.const_1 : ButtonTypes.const_0, this);
			});
		}
		if (suitVariantBtn != null)
		{
			if (ExpansionsLoader.IsExpansionAnyKerbalSuitInstalled())
			{
				suitVariantBtn.gameObject.SetActive(value: true);
				suitVariantBtn.interactable = true;
				suitVariantBtn.onClick.AddListener(OnSuitVariantButton);
			}
			else
			{
				suitVariantBtn.gameObject.SetActive(value: false);
				suitVariantBtn.interactable = false;
			}
		}
	}

	private void OnSuitVariantButton()
	{
		if (!HighLogic.LoadedSceneIsMissionBuilder && crew.rosterStatus != 0)
		{
			return;
		}
		int num = Enum.GetNames(typeof(ProtoCrewMember.KerbalSuit)).Length;
		bool flag = false;
		int num2 = (int)(crew.suit + 1);
		while (!flag)
		{
			if (num2 >= num)
			{
				crew.suit = ProtoCrewMember.KerbalSuit.Default;
				flag = true;
			}
			if (ExpansionsLoader.IsExpansionKerbalSuitInstalled((ProtoCrewMember.KerbalSuit)num2))
			{
				crew.suit = (ProtoCrewMember.KerbalSuit)num2;
				flag = true;
			}
			if (!flag)
			{
				num2++;
			}
		}
		SetKerbal(crew, lastType);
	}

	public void SetTooltip(ProtoCrewMember crew)
	{
		if (tooltipController != null)
		{
			tooltipController.SetTooltip(crew);
		}
		else
		{
			Debug.LogError("[CrewListItem] No tooltip serialized.");
		}
	}

	public void SetName(string name)
	{
		kerbalName.text = name;
	}

	public string GetName()
	{
		return kerbalName.text;
	}

	public void SetLabel(string label)
	{
		if (this.label != null)
		{
			this.label.text = label;
		}
	}

	public void SetXP(ProtoCrewMember pcm)
	{
		if (pcm.experienceTrait == null)
		{
			KerbalRoster.SetExperienceTrait(pcm);
		}
		if (xp_trait != null)
		{
			xp_trait.text = pcm.experienceTrait.Title;
		}
		if (xp_levels != null)
		{
			xp_levels.SetState(pcm.experienceLevel);
		}
		if (xp_slider != null)
		{
			xp_slider.value = pcm.ExperienceLevelDelta;
		}
	}

	public void SetXP(string trait, float xp, int level)
	{
		if (xp_trait != null)
		{
			xp_trait.text = trait;
		}
		if (xp_levels != null)
		{
			xp_levels.SetState(level);
		}
		if (xp_slider != null)
		{
			xp_slider.value = xp;
		}
	}

	public void SetButton(ButtonTypes type)
	{
		switch (type)
		{
		case ButtonTypes.const_0:
			button.SetState(0);
			break;
		case ButtonTypes.const_1:
			button.SetState(1);
			break;
		case ButtonTypes.X2:
			button.SetState(2);
			break;
		}
	}

	public void SetButtonEnabled(bool state, string disabledReasonTitle = "", string disabledReasonCaption = "")
	{
		if (state)
		{
			MouseoverEnabled = true;
			tooltipController.SetTooltip(crew);
		}
		else
		{
			MouseoverEnabled = false;
			tooltipController.SetTooltip(crew, disabledReasonTitle, disabledReasonCaption);
		}
	}

	public void SetKerbal(ProtoCrewMember crew, KerbalTypes type)
	{
		lastType = type;
		if (suitVariantBtn != null && ExpansionsLoader.IsExpansionAnyKerbalSuitInstalled())
		{
			suitVariantBtn.gameObject.SetActive(crew.type != ProtoCrewMember.KerbalType.Applicant && (crew.rosterStatus == ProtoCrewMember.RosterStatus.Available || crew.rosterStatus == ProtoCrewMember.RosterStatus.Assigned) && crew.type != ProtoCrewMember.KerbalType.Tourist);
		}
		string kerbalIconSuitSuffix = crew.GetKerbalIconSuitSuffix();
		ProtoCrewMember.Gender gender = crew.gender;
		if (gender != 0 && gender == ProtoCrewMember.Gender.Female)
		{
			switch (type)
			{
			default:
				if (kerbalIconSuitSuffix == "_vintage")
				{
					kerbalSprite.texture = MissionsUtils.METexture("Kerbals/Textures/kerbalIcons/kerbalicon_suit_female" + kerbalIconSuitSuffix + ".tif");
				}
				else if (kerbalIconSuitSuffix == "_future")
				{
					kerbalSprite.texture = SerenityUtils.SerenityTexture("Kerbals/Textures/kerbalIcons/kerbalicon_suit_female" + kerbalIconSuitSuffix + ".tif");
				}
				else
				{
					kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_suit_female");
				}
				break;
			case KerbalTypes.BADASS:
				if (kerbalIconSuitSuffix == "_vintage")
				{
					kerbalSprite.texture = MissionsUtils.METexture("Kerbals/Textures/kerbalIcons/kerbalicon_suit_orange_female" + kerbalIconSuitSuffix + ".tif");
				}
				else if (kerbalIconSuitSuffix == "_future")
				{
					kerbalSprite.texture = SerenityUtils.SerenityTexture("Kerbals/Textures/kerbalIcons/kerbalicon_suit_orange_female" + kerbalIconSuitSuffix + ".tif");
				}
				else
				{
					kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_suit_orange_female");
				}
				break;
			case KerbalTypes.const_2:
				if (kerbalIconSuitSuffix == "_vintage")
				{
					kerbalSprite.texture = MissionsUtils.METexture("Kerbals/Textures/kerbalIcons/kerbalicon_eva_female" + kerbalIconSuitSuffix + ".tif");
				}
				else if (kerbalIconSuitSuffix == "_future")
				{
					kerbalSprite.texture = SerenityUtils.SerenityTexture("Kerbals/Textures/kerbalIcons/kerbalicon_eva_female" + kerbalIconSuitSuffix + ".tif");
				}
				else
				{
					kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_eva_female");
				}
				break;
			case KerbalTypes.RECRUIT:
				kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_recruit_female");
				break;
			case KerbalTypes.TOURIST:
				kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_tourist_female");
				break;
			}
			return;
		}
		switch (type)
		{
		default:
			if (kerbalIconSuitSuffix == "_vintage")
			{
				kerbalSprite.texture = MissionsUtils.METexture("Kerbals/Textures/kerbalIcons/kerbalicon_suit" + kerbalIconSuitSuffix + ".tif");
			}
			else if (kerbalIconSuitSuffix == "_future")
			{
				kerbalSprite.texture = SerenityUtils.SerenityTexture("Kerbals/Textures/kerbalIcons/kerbalicon_suit" + kerbalIconSuitSuffix + ".tif");
			}
			else
			{
				kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_suit");
			}
			break;
		case KerbalTypes.BADASS:
			if (kerbalIconSuitSuffix == "_vintage")
			{
				kerbalSprite.texture = MissionsUtils.METexture("Kerbals/Textures/kerbalIcons/kerbalicon_suit_orange" + kerbalIconSuitSuffix + ".tif");
			}
			else if (kerbalIconSuitSuffix == "_future")
			{
				kerbalSprite.texture = SerenityUtils.SerenityTexture("Kerbals/Textures/kerbalIcons/kerbalicon_suit_orange" + kerbalIconSuitSuffix + ".tif");
			}
			else
			{
				kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_suit_orange");
			}
			break;
		case KerbalTypes.const_2:
			if (kerbalIconSuitSuffix == "_vintage")
			{
				kerbalSprite.texture = MissionsUtils.METexture("Kerbals/Textures/kerbalIcons/kerbalicon_eva" + kerbalIconSuitSuffix + ".tif");
			}
			else if (kerbalIconSuitSuffix == "_future")
			{
				kerbalSprite.texture = SerenityUtils.SerenityTexture("Kerbals/Textures/kerbalIcons/kerbalicon_eva" + kerbalIconSuitSuffix + ".tif");
			}
			else
			{
				kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_eva");
			}
			break;
		case KerbalTypes.RECRUIT:
			kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_recruit");
			break;
		case KerbalTypes.TOURIST:
			kerbalSprite.texture = AssetBase.GetTexture("kerbalicon_tourist");
			break;
		}
	}

	public void SetKerbalAsApplicableType(ProtoCrewMember crew)
	{
		if (crew.veteran)
		{
			SetKerbal(crew, KerbalTypes.BADASS);
		}
		else if (crew.type == ProtoCrewMember.KerbalType.Tourist)
		{
			SetKerbal(crew, KerbalTypes.TOURIST);
		}
		else
		{
			SetKerbal(crew, KerbalTypes.AVAILABLE);
		}
	}

	public void AddButtonInputDelegate(UnityAction<ButtonTypes, CrewListItem> del)
	{
		onClick.AddListener(del);
	}

	public void SetStats(ProtoCrewMember pcm)
	{
		SetStats(pcm.courage, pcm.stupidity);
	}

	public void SetStats(float courage, float stupidity)
	{
		if (slider_courage != null)
		{
			slider_courage.value = courage;
		}
		if (slider_stupidity != null)
		{
			slider_stupidity.value = stupidity;
		}
	}

	public float GetCourage()
	{
		return slider_courage.value;
	}

	public float GetStupidity()
	{
		return slider_stupidity.value;
	}

	public void SetCrewRef(ProtoCrewMember crewRef)
	{
		crew = crewRef;
		if (suitVariantBtn != null && ExpansionsLoader.IsExpansionAnyKerbalSuitInstalled())
		{
			suitVariantBtn.gameObject.SetActive((HighLogic.LoadedSceneIsMissionBuilder || crew.rosterStatus == ProtoCrewMember.RosterStatus.Available) && crew.type != ProtoCrewMember.KerbalType.Tourist);
		}
	}

	public ProtoCrewMember GetCrewRef()
	{
		return crew;
	}
}
