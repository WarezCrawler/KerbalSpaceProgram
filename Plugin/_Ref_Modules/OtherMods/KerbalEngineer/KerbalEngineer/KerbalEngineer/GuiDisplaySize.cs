using System;
using KerbalEngineer.Settings;

namespace KerbalEngineer;

public class GuiDisplaySize
{
	public delegate void SizeChanged();

	private static float multiplier;

	private static int increment;

	private static float offset;

	public static int Increment
	{
		get
		{
			return increment;
		}
		set
		{
			try
			{
				if (increment != value)
				{
					increment = value;
					SettingHandler settingHandler = SettingHandler.Load("GuiDisplaySize.xml");
					settingHandler.Set("increment", increment);
					settingHandler.Save("GuiDisplaySize.xml");
					offset = 1f + (float)increment * multiplier - (float)increment;
					GuiDisplaySize.OnSizeChanged();
				}
			}
			catch (Exception ex)
			{
				MyLogger.Exception(ex, "GuiDisplaySize->Increment");
			}
		}
	}

	public static float Offset => offset;

	public static event SizeChanged OnSizeChanged;

	static GuiDisplaySize()
	{
		multiplier = 1.1f;
		try
		{
			SettingHandler settingHandler = SettingHandler.Load("GuiDisplaySize.xml");
			settingHandler.Set("multiplier", 1.1);
			increment = settingHandler.GetSet("increment", increment);
			settingHandler.Save("GuiDisplaySize.xml");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "GuiDisplaySize->GuiDisplaySize");
		}
		offset = 1f + (float)increment * multiplier - (float)increment;
	}
}
