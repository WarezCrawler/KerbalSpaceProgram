using System;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using UniLinq;
using UnityEngine;

namespace B9PartSwitch;

public abstract class CustomPartModule : PartModule, ISerializationCallbackReceiver
{
	public const string CURRENT_UPGRADE = "CURRENTUPGRADE";

	[NodeData(persistent = true)]
	public string moduleID;

	[SerializeField]
	private SerializedDataContainer serializedData;

	private void Start()
	{
		CustomPartModule[] array = (from m in base.part.Modules.OfType<CustomPartModule>()
			where m != this && m.GetType() == GetType()
			select m).ToArray();
		if (array.Length != 0 && moduleID.IsNullOrEmpty())
		{
			LogError("Must have a moduleID defined if more than one " + GetType().Name + " is present on a part.  This module will be removed");
			base.part.Modules.Remove(this);
			UnityEngine.Object.Destroy(this);
			return;
		}
		CustomPartModule[] array2 = array;
		foreach (CustomPartModule customPartModule in array2)
		{
			if (customPartModule.moduleID.IsNullOrEmpty() || customPartModule.moduleID == moduleID)
			{
				LogError("Two " + GetType().Name + " modules on the same part must have different (and non-empty) moduleID identifiers.  The second " + GetType().Name + " will be removed");
				base.part.Modules.Remove(customPartModule);
				UnityEngine.Object.Destroy(customPartModule);
			}
		}
	}

	public override void OnLoad(ConfigNode node)
	{
		base.OnLoad(node);
		if (!moduleID.IsNullOrEmpty() && node.HasValue("moduleID"))
		{
			string newID = node.GetValue("moduleID");
			if (!string.Equals(moduleID, newID))
			{
				CustomPartModule customPartModule = base.part.Modules.OfType<CustomPartModule>().FirstOrDefault((CustomPartModule m) => m != this && m.GetType() == GetType() && m.moduleID == newID);
				if (customPartModule.IsNotNull())
				{
					LogWarning("OnLoad was called with the wrong ModuleID ('" + newID + "'), but found the correct module to load");
					customPartModule.Load(node);
				}
				else
				{
					LogError("OnLoad was called with the wrong ModuleID and the correct module could not be found");
				}
				return;
			}
		}
		bool flag = base.part.partInfo.IsNull() || node.name == "CURRENTUPGRADE";
		OperationContext context = new OperationContext(flag ? Operation.LoadPrefab : Operation.LoadInstance, this);
		try
		{
			this.LoadFields(node, context);
		}
		catch (Exception innerException)
		{
			Exception ex = new Exception($"Fatal exception while loading fields on module {this}", innerException);
			FatalErrorHandler.HandleFatalError(ex);
			throw ex;
		}
		if (flag)
		{
			OnLoadPrefab(node);
		}
		else
		{
			OnLoadInstance(node);
		}
	}

	protected virtual void OnLoadPrefab(ConfigNode node)
	{
	}

	protected virtual void OnLoadInstance(ConfigNode node)
	{
	}

	public override void OnSave(ConfigNode node)
	{
		base.OnSave(node);
		OperationContext context = new OperationContext(Operation.Save, this);
		try
		{
			this.SaveFields(node, context);
		}
		catch (Exception innerException)
		{
			Exception ex = new Exception($"Fatal exception while saving fields on module {this}", innerException);
			FatalErrorHandler.HandleFatalError(ex);
			throw ex;
		}
	}

	public virtual void OnBeforeSerialize()
	{
		serializedData = this.SerializeToContainer();
	}

	public virtual void OnAfterDeserialize()
	{
		this.DeserializeFromContainer(serializedData);
		UnityEngine.Object.Destroy(serializedData);
		serializedData = null;
	}

	protected void LogInfo(object message)
	{
		PartModuleExtensions.LogInfo(this, message);
	}

	protected void LogWarning(object message)
	{
		PartModuleExtensions.LogWarning(this, message);
	}

	protected void LogError(object message)
	{
		PartModuleExtensions.LogError(this, message);
	}

	public override string ToString()
	{
		string text = GetType().Name;
		if (!moduleID.IsNullOrEmpty())
		{
			text = text + " (moduleID='" + moduleID + "')";
		}
		if (base.part != null)
		{
			text = text + " on part " + base.part.partInfo?.name;
		}
		return text;
	}
}
