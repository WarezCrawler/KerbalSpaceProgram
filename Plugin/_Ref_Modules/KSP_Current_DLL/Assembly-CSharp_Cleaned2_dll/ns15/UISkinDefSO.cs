using UnityEngine;

namespace ns15;

public class UISkinDefSO : ScriptableObject
{
	[SerializeField]
	private UISkinDef skinDef;

	public UISkinDef SkinDef => skinDef;
}
