using TMPro;
using UnityEngine;
using ns9;

[UI_Label]
public class UIPartActionAeroDisplay : UIPartActionItem
{
	public int rows = 10;

	internal BasePAWGroup pawGroup;

	[SerializeField]
	private TextMeshProUGUI txtMach;

	[SerializeField]
	private TextMeshProUGUI txtDragVector;

	[SerializeField]
	private TextMeshProUGUI txtDragCube0;

	[SerializeField]
	private TextMeshProUGUI txtDragCube1;

	[SerializeField]
	private TextMeshProUGUI txtDragCube2;

	[SerializeField]
	private TextMeshProUGUI txtDragCube3;

	[SerializeField]
	private TextMeshProUGUI txtDragCube4;

	[SerializeField]
	private TextMeshProUGUI txtDragCube5;

	[SerializeField]
	private TextMeshProUGUI txtDragScalar;

	[SerializeField]
	private TextMeshProUGUI txtAreaDrag;

	public virtual void Setup(UIPartActionWindow window, Part part, UI_Scene scene)
	{
		SetupItem(window, part, null, scene, null);
	}

	private void Awake()
	{
		pawGroup = new BasePAWGroup("Debug", "#autoLOC_8320010", startCollapsed: false);
	}

	public override void UpdateItem()
	{
		txtMach.text = Localizer.Format("#autoLOC_357350", part.machNumber.ToString("F2"));
		txtDragCube0.text = "XP : " + KSPUtil.LocalizeNumber(part.DragCubes.AreaOccluded[0], "F2") + " : " + KSPUtil.LocalizeNumber(part.DragCubes.WeightedDrag[0], "F2");
		txtDragCube1.text = "XN : " + KSPUtil.LocalizeNumber(part.DragCubes.AreaOccluded[1], "F2") + " : " + KSPUtil.LocalizeNumber(part.DragCubes.WeightedDrag[1], "F2");
		txtDragCube2.text = "YP : " + KSPUtil.LocalizeNumber(part.DragCubes.AreaOccluded[2], "F2") + " : " + KSPUtil.LocalizeNumber(part.DragCubes.WeightedDrag[2], "F2");
		txtDragCube3.text = "YN : " + KSPUtil.LocalizeNumber(part.DragCubes.AreaOccluded[3], "F2") + " : " + KSPUtil.LocalizeNumber(part.DragCubes.WeightedDrag[3], "F2");
		txtDragCube4.text = "ZP : " + KSPUtil.LocalizeNumber(part.DragCubes.AreaOccluded[4], "F2") + " : " + KSPUtil.LocalizeNumber(part.DragCubes.WeightedDrag[4], "F2");
		txtDragCube5.text = "ZN : " + KSPUtil.LocalizeNumber(part.DragCubes.AreaOccluded[5], "F2") + " : " + KSPUtil.LocalizeNumber(part.DragCubes.WeightedDrag[5], "F2");
		txtDragVector.text = Localizer.Format("#autoLOC_6001917", part.DragCubes.DragVector.ToString());
		txtAreaDrag.text = "A.Cd: " + KSPUtil.LocalizeNumber(part.DragCubes.AreaDrag, "F2");
		txtDragScalar.text = Localizer.Format("#autoLOC_6001916", part.dragScalar.ToString("F2"));
	}
}
