using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KerbalEngineer.Unity.Flight;

[RequireComponent(typeof(RectTransform))]
public class FlightMenu : CanvasGroupFader, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private Toggle showEngineerToggle = null;

	[SerializeField]
	private Toggle controlBarToggle = null;

	[SerializeField]
	private GameObject menuSectionPrefab = null;

	[SerializeField]
	private Transform sectionsTransform = null;

	[SerializeField]
	private float fastFadeDuration = 0.2f;

	[SerializeField]
	private float slowFadeDuration = 1f;

	private IFlightAppLauncher flightAppLauncher;

	private RectTransform rectTransform;

	public void OnPointerEnter(PointerEventData eventData)
	{
		FadeIn();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (flightAppLauncher != null && !flightAppLauncher.IsOn)
		{
			FadeTo(0f, slowFadeDuration, Destroy);
		}
	}

	public void Close()
	{
		FadeTo(0f, fastFadeDuration, Destroy);
	}

	public void FadeIn()
	{
		FadeTo(1f, fastFadeDuration);
	}

	public void NewCustomSection()
	{
		if (flightAppLauncher != null)
		{
			CreateSectionControl(flightAppLauncher.NewCustomSection());
		}
	}

	public void SetControlBarVisible(bool visible)
	{
		if (flightAppLauncher != null)
		{
			flightAppLauncher.IsControlBarVisible = visible;
		}
	}

	public void SetDisplayStackVisible(bool visible)
	{
		if (flightAppLauncher != null)
		{
			flightAppLauncher.IsDisplayStackVisible = visible;
		}
	}

	public void SetFlightAppLauncher(IFlightAppLauncher flightAppLauncher)
	{
		if (flightAppLauncher != null)
		{
			this.flightAppLauncher = flightAppLauncher;
			CreateSectionControls(this.flightAppLauncher.GetStockSections());
			CreateSectionControls(this.flightAppLauncher.GetCustomSections());
		}
	}

	protected override void Awake()
	{
		base.Awake();
		rectTransform = ((Component)this).GetComponent<RectTransform>();
	}

	protected virtual void Start()
	{
		SetAlpha(0f);
		FadeIn();
	}

	protected virtual void Update()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (flightAppLauncher != null)
		{
			SetToggle(showEngineerToggle, flightAppLauncher.IsDisplayStackVisible);
			SetToggle(controlBarToggle, flightAppLauncher.IsControlBarVisible);
			if ((Object)(object)rectTransform != (Object)null)
			{
				((Transform)rectTransform).position = flightAppLauncher.GetAnchor();
				flightAppLauncher.ClampToScreen(rectTransform);
			}
		}
	}

	private static void SetToggle(Toggle toggle, bool state)
	{
		if ((Object)(object)toggle != (Object)null)
		{
			toggle.isOn = state;
		}
	}

	private void CreateSectionControl(ISectionModule section)
	{
		if (section == null)
		{
			return;
		}
		GameObject val = Object.Instantiate<GameObject>(menuSectionPrefab);
		if ((Object)(object)val != (Object)null)
		{
			flightAppLauncher.ApplyTheme(val);
			val.transform.SetParent(sectionsTransform, false);
			FlightMenuSection component = val.GetComponent<FlightMenuSection>();
			if ((Object)(object)component != (Object)null)
			{
				component.SetAssignedSection(section);
			}
		}
	}

	private void CreateSectionControls(IList<ISectionModule> sections)
	{
		if (sections == null || (Object)(object)menuSectionPrefab == (Object)null || (Object)(object)sectionsTransform == (Object)null)
		{
			return;
		}
		for (int i = 0; i < sections.Count; i++)
		{
			ISectionModule sectionModule = sections[i];
			if (sectionModule != null)
			{
				CreateSectionControl(sectionModule);
			}
		}
	}

	private void Destroy()
	{
		((Component)this).gameObject.SetActive(false);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}
}
