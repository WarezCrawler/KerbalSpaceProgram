using Expansions.Serenity;
using UnityEngine;
using ns9;

namespace ns10;

public class EditorActionController : EditorActionGroup_Base
{
	public uint PartId { get; private set; }

	public ModuleRoboticController controller { get; private set; }

	private void Setup(string groupName, bool contains)
	{
		base.groupName.text = Localizer.Format(groupName);
		if (contains)
		{
			base.groupName.color = Color.yellow;
		}
		else
		{
			base.groupName.color = Color.white;
		}
	}

	public void Setup(ModuleRoboticController controller, bool contains)
	{
		PartId = controller.PartPersistentId;
		this.controller = controller;
		_type = EditorActionGroupType.Controller;
		Setup(controller.displayName, contains);
	}
}
