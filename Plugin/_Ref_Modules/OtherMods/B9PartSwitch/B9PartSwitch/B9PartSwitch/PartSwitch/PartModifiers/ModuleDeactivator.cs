namespace B9PartSwitch.PartSwitch.PartModifiers;

public class ModuleDeactivator : PartModifierBase
{
	public readonly PartModule module;

	protected readonly PartModule parent;

	public override string Description => "module " + module.ToString() + " activated status";

	public ModuleDeactivator(PartModule module, PartModule parent)
	{
		module.ThrowIfNullArgument("module");
		parent.ThrowIfNullArgument("parent");
		this.module = module;
		this.parent = parent;
	}

	public override void ActivateOnStartEditor()
	{
		Activate();
	}

	public override void ActivateOnStartFlight()
	{
		Activate();
	}

	public override void DeactivateOnSwitchEditor()
	{
		MaybeDeactivate();
	}

	public override void DeactivateOnSwitchFlight()
	{
		MaybeDeactivate();
	}

	public override void ActivateOnSwitchEditor()
	{
		Activate();
	}

	public override void ActivateOnSwitchFlight()
	{
		Activate();
	}

	public override void OnWillBeCopiedActiveSubtype()
	{
		Deactivate();
	}

	public override void OnWasCopiedActiveSubtype()
	{
		Activate();
	}

	protected virtual void Activate()
	{
		module.enabled = false;
		module.isEnabled = false;
	}

	protected virtual void MaybeDeactivate()
	{
		foreach (PartModule module in module.part.Modules)
		{
			if (!(module == parent) && module is ModuleB9PartSwitch moduleB9PartSwitch && moduleB9PartSwitch.ModuleShouldBeEnabled(this.module))
			{
				return;
			}
		}
		Deactivate();
	}

	protected virtual void Deactivate()
	{
		module.enabled = true;
		module.isEnabled = true;
	}
}
