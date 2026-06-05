using KerbalEngineer.Unity;
using KerbalEngineer.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace KerbalEngineer;

public static class StyleManager
{
	private static GameObject windowPrefab;

	private static GameObject settingPrefab;

	public static Setting CreateSetting(string label, Window window)
	{
		Setting setting = null;
		GameObject val = GetSettingPrefab();
		if ((Object)(object)val != (Object)null && (Object)(object)window != (Object)null)
		{
			GameObject val2 = Object.Instantiate<GameObject>(val);
			if ((Object)(object)val2 != (Object)null)
			{
				setting = val2.GetComponent<Setting>();
				if ((Object)(object)setting != (Object)null)
				{
					setting.SetLabel(label);
					window.AddToContent(val2);
				}
			}
		}
		return setting;
	}

	public static Window CreateWindow(string title, float width)
	{
		GameObject val = GetWindowPrefab();
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		GameObject val2 = Object.Instantiate<GameObject>(val);
		if ((Object)(object)val2 == (Object)null)
		{
			return null;
		}
		Process(val2);
		val2.transform.SetParent(((Component)MainCanvasUtil.MainCanvas).transform, false);
		Window component = val2.GetComponent<Window>();
		if ((Object)(object)component != (Object)null)
		{
			component.SetTitle(title);
			component.SetWidth(width);
		}
		return component;
	}

	public static void Process(GameObject gameObject)
	{
		if ((Object)(object)gameObject == (Object)null)
		{
			return;
		}
		StyleApplicator[] componentsInChildren = gameObject.GetComponentsInChildren<StyleApplicator>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Process(componentsInChildren[i]);
			}
		}
	}

	public static void Process(Component component)
	{
		if ((Object)(object)component != (Object)null)
		{
			Process(component.gameObject);
		}
	}

	private static GameObject GetSettingPrefab()
	{
		if ((Object)(object)settingPrefab == (Object)null)
		{
			settingPrefab = AssetBundleLoader.Prefabs.LoadAsset<GameObject>("Setting");
		}
		return settingPrefab;
	}

	private static TextStyle GetTextStyle(UIStyle style, UIStyleState styleState)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		TextStyle textStyle = new TextStyle();
		if (style != null)
		{
			textStyle.Font = style.font;
			textStyle.Style = style.fontStyle;
			textStyle.Size = style.fontSize;
		}
		if (styleState != null)
		{
			textStyle.Colour = styleState.textColor;
		}
		return textStyle;
	}

	private static GameObject GetWindowPrefab()
	{
		if ((Object)(object)windowPrefab == (Object)null)
		{
			windowPrefab = AssetBundleLoader.Prefabs.LoadAsset<GameObject>("Window");
		}
		return windowPrefab;
	}

	private static void Process(StyleApplicator applicator)
	{
		if ((Object)(object)applicator == (Object)null)
		{
			return;
		}
		UISkinDef defaultSkin = UISkinManager.defaultSkin;
		if (defaultSkin != null)
		{
			switch (applicator.ElementType)
			{
			case StyleApplicator.ElementTypes.Window:
				applicator.SetImage(defaultSkin.window.normal.background, (Type)1);
				break;
			case StyleApplicator.ElementTypes.Box:
				applicator.SetImage(defaultSkin.box.normal.background, (Type)1);
				break;
			case StyleApplicator.ElementTypes.Button:
				applicator.SetSelectable(null, defaultSkin.button.normal.background, defaultSkin.button.highlight.background, defaultSkin.button.active.background, defaultSkin.button.disabled.background);
				break;
			case StyleApplicator.ElementTypes.ButtonToggle:
				applicator.SetToggle(null, defaultSkin.button.normal.background, defaultSkin.button.highlight.background, defaultSkin.button.active.background, defaultSkin.button.disabled.background);
				break;
			case StyleApplicator.ElementTypes.Label:
				applicator.SetText(GetTextStyle(defaultSkin.label, defaultSkin.label.normal));
				break;
			}
		}
	}
}
