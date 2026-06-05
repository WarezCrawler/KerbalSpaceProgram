using UnityEngine;
using UnityEngine.UI;

namespace KerbalEngineer.Unity.Flight;

public class FlightMenuSection : MonoBehaviour
{
	[SerializeField]
	private Toggle displayToggle = null;

	[SerializeField]
	private Text displayText = null;

	[SerializeField]
	private Toggle editToggle = null;

	private ISectionModule section;

	public bool IsEditorVisible
	{
		get
		{
			if ((Object)(object)editToggle != (Object)null)
			{
				return editToggle.isOn;
			}
			return true;
		}
		set
		{
			if ((Object)(object)editToggle != (Object)null)
			{
				editToggle.isOn = value;
			}
		}
	}

	public void SetAssignedSection(ISectionModule section)
	{
		if (section != null)
		{
			this.section = section;
		}
	}

	public void SetDisplayVisible(bool visible)
	{
		if (section != null)
		{
			section.IsVisible = visible;
		}
	}

	public void SetEditorVisible(bool visible)
	{
		if (section != null)
		{
			section.IsEditorVisible = visible;
		}
	}

	protected virtual void Update()
	{
		UpdateControls();
	}

	private void UpdateControls()
	{
		if (section == null || section.IsDeleted)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return;
		}
		if ((Object)(object)displayToggle != (Object)null)
		{
			displayToggle.isOn = section.IsVisible;
		}
		if ((Object)(object)displayText != (Object)null)
		{
			displayText.text = section.Name.ToUpperInvariant();
		}
		if ((Object)(object)editToggle != (Object)null)
		{
			editToggle.isOn = section.IsEditorVisible;
		}
	}
}
