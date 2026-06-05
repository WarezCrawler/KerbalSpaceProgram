namespace B9PartSwitch;

public static class PartModuleExtensions
{
	public static bool ParsedPrefab(this PartModule module)
	{
		return module.part.partInfo != null;
	}

	public static void SetUiGroups(this PartModule module, string uiGroupName, string uiGroupDisplayName)
	{
		module.ThrowIfNullArgument("module");
		foreach (BaseField field in module.Fields)
		{
			if (field.group.name.IsNullOrEmpty())
			{
				field.group.name = uiGroupName;
				field.group.displayName = uiGroupDisplayName;
			}
		}
		foreach (BaseEvent @event in module.Events)
		{
			if (@event.group.name.IsNullOrEmpty())
			{
				@event.group.name = uiGroupName;
				@event.group.displayName = uiGroupDisplayName;
			}
		}
	}

	public static void LogInfo(this PartModule module, object message)
	{
		module.part.LogInfo($"{module.LogTagString()} {message}");
	}

	public static void LogWarning(this PartModule module, object message)
	{
		module.part.LogWarning($"{module.LogTagString()} {message}");
	}

	public static void LogError(this PartModule module, object message)
	{
		module.part.LogError($"{module.LogTagString()} {message}");
	}

	public static string LogTagString(this PartModule module)
	{
		string text = module.GetType().Name;
		if (module is CustomPartModule customPartModule && !customPartModule.moduleID.IsNullOrEmpty())
		{
			text = text + " '" + customPartModule.moduleID + "'";
		}
		return "[" + text + "]";
	}
}
