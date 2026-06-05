using System.Collections.Generic;
using UnityEngine;

namespace KerbalEngineer.Unity.Flight;

public interface IFlightAppLauncher
{
	bool IsControlBarVisible { get; set; }

	bool IsDisplayStackVisible { get; set; }

	bool IsOn { get; }

	void ApplyTheme(GameObject gameObject);

	void ClampToScreen(RectTransform rectTransform);

	Vector3 GetAnchor();

	IList<ISectionModule> GetCustomSections();

	IList<ISectionModule> GetStockSections();

	ISectionModule NewCustomSection();
}
