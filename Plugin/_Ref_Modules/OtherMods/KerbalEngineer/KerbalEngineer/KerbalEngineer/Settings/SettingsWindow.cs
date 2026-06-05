using System;
using KerbalEngineer.Editor;
using KerbalEngineer.Flight;
using KerbalEngineer.KeyBinding;
using KerbalEngineer.Unity;
using KerbalEngineer.Unity.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KerbalEngineer.Settings;

public class SettingsWindow : MonoBehaviour
{
	private static Window m_Window;

	public static void Close()
	{
		if ((Object)(object)m_Window != (Object)null)
		{
			m_Window.Close();
		}
	}

	public static void Open()
	{
		if ((Object)(object)m_Window == (Object)null)
		{
			m_Window = StyleManager.CreateWindow("SETTINGS", 600f);
			AddKeyBindingsButton();
			AddFlightActivationModes();
			AddBuildOverlayOptions();
			StyleManager.Process((Component)(object)m_Window);
		}
	}

	private static void AddBuildOverlayOptions()
	{
		if ((Object)(object)m_Window != (Object)null)
		{
			Setting setting = StyleManager.CreateSetting("Build Engineer Overlay", m_Window);
			Toggle buildOverlayVisible = AddToggle(setting, "VISIBLE", 100f, delegate(bool value)
			{
				BuildOverlay.Visible = value;
			});
			Toggle buildOverlayNamesOnly = AddToggle(setting, "NAMES ONLY", 100f, delegate(bool value)
			{
				BuildOverlayPartInfo.NamesOnly = value;
			});
			Toggle buildOverlayClickToOpen = AddToggle(setting, "CLICK TO OPEN", 100f, delegate(bool value)
			{
				BuildOverlayPartInfo.ClickToOpen = value;
			});
			AddUpdateHandler(setting, delegate
			{
				buildOverlayVisible.isOn = BuildOverlay.Visible;
				buildOverlayNamesOnly.isOn = BuildOverlayPartInfo.NamesOnly;
				buildOverlayClickToOpen.isOn = BuildOverlayPartInfo.ClickToOpen;
			});
		}
	}

	private static Button AddButton(Setting setting, string text, float width, UnityAction onClick)
	{
		Button result = null;
		if ((Object)(object)setting != (Object)null)
		{
			result = setting.AddButton(text, width, onClick);
		}
		return result;
	}

	private static void AddFlightActivationModes()
	{
		if ((Object)(object)m_Window != (Object)null)
		{
			Setting setting = StyleManager.CreateSetting("Flight Engineer Activation Mode", m_Window);
			Toggle flightActivationModeCareer = AddToggle(setting, "CAREER", 100f, delegate(bool value)
			{
				FlightEngineerCore.IsCareerMode = value;
			});
			Toggle flightActivationModePartless = AddToggle(setting, "PARTLESS", 100f, delegate(bool value)
			{
				FlightEngineerCore.IsCareerMode = !value;
			});
			AddUpdateHandler(setting, delegate
			{
				flightActivationModeCareer.isOn = FlightEngineerCore.IsCareerMode;
				flightActivationModePartless.isOn = !FlightEngineerCore.IsCareerMode;
			});
		}
	}

	private static void AddKeyBindingsButton()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		if ((Object)(object)m_Window != (Object)null)
		{
			AddButton(StyleManager.CreateSetting("Key Bindings", m_Window), "EDIT KEY BINDINGS", 304f, new UnityAction(KeyBinder.Show));
		}
	}

	private static Toggle AddToggle(Setting setting, string text, float width, UnityAction<bool> onValueChanged)
	{
		Toggle result = null;
		if ((Object)(object)setting != (Object)null)
		{
			result = setting.AddToggle(text, width, onValueChanged);
		}
		return result;
	}

	private static void AddUpdateHandler(Setting setting, Action onUpdate)
	{
		if ((Object)(object)setting != (Object)null && onUpdate != null)
		{
			setting.AddUpdateHandler(onUpdate);
		}
	}
}
