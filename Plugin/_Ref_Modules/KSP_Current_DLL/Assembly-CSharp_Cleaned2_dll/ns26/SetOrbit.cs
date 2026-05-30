using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ns25;
using ns9;

namespace ns26;

public class SetOrbit : MonoBehaviour
{
	public Button bodyBackButton;

	public TextMeshProUGUI bodyNameText;

	public Button bodyForwardButton;

	public DebugScreenInputDouble eccInput;

	public DebugScreenInputDouble incInput;

	public DebugScreenInputDouble smaInput;

	public DebugScreenInputDouble mnaInput;

	public DebugScreenInputDouble lanInput;

	public DebugScreenInputDouble lpeInput;

	public DebugScreenInputDouble obtInput;

	public Button setOrbitButton;

	public TextMeshProUGUI errorText;

	public Button vesselBackButton;

	public TextMeshProUGUI vesselNameText;

	public Button vesselForwardButton;

	public Button rendezvousButton;

	private int selectedBody;

	private int selectedVessel;

	public static double safetyEnvelope = 1.025;

	public static float rendezvousDistance = 150f;

	public CelestialBody SelectedBody
	{
		get
		{
			if (FlightGlobals.Bodies != null && FlightGlobals.Bodies.Count > selectedBody)
			{
				return FlightGlobals.Bodies[selectedBody];
			}
			return null;
		}
	}

	public Vessel SelectedVessel
	{
		get
		{
			if (FlightGlobals.Vessels != null && FlightGlobals.Vessels.Count > selectedVessel)
			{
				return FlightGlobals.Vessels[selectedVessel];
			}
			return null;
		}
	}

	private void Awake()
	{
		bodyBackButton.onClick.AddListener(OnBodyBackClick);
		bodyForwardButton.onClick.AddListener(OnBodyForwardClick);
		vesselBackButton.onClick.AddListener(OnVesselBackClick);
		vesselForwardButton.onClick.AddListener(OnVesselForwardClick);
		setOrbitButton.onClick.AddListener(OnSetOrbitClick);
		rendezvousButton.onClick.AddListener(OnRendezvousClick);
	}

	private void Start()
	{
		if (FlightGlobals.fetch != null)
		{
			selectedBody = FlightGlobals.GetHomeBodyIndex();
			SetSelectedBodyText();
			selectedVessel = 0;
			SetSelectedVesselText();
			smaInput.Value = SelectedBody.minOrbitalDistance * safetyEnvelope;
		}
		else
		{
			bodyNameText.text = string.Empty;
			smaInput.Value = 140000.0;
		}
		errorText.text = string.Empty;
	}

	private void OnSetOrbitClick()
	{
		bool flag = false;
		string text = string.Empty;
		if (FlightGlobals.ActiveVessel == null)
		{
			text += Localizer.Format("#autoLOC_6001899");
			flag = true;
		}
		if (eccInput.Value == 1.0)
		{
			text += Localizer.Format("#autoLOC_6001900");
			eccInput.Value += 1E-10;
			flag = true;
		}
		double num = (1.0 - eccInput.Value) * smaInput.Value;
		if (num < FlightGlobals.Bodies[selectedBody].minOrbitalDistance * safetyEnvelope)
		{
			text += Localizer.Format("#autoLOC_6001901");
			num = FlightGlobals.Bodies[selectedBody].minOrbitalDistance * safetyEnvelope;
			smaInput.Value = Math.Ceiling(num / (1.0 - eccInput.Value));
			flag = true;
		}
		if (eccInput.Value < 0.0)
		{
			text += Localizer.Format("#autoLOC_6001902");
			flag = true;
		}
		if (incInput.Value > 180.0)
		{
			text += Localizer.Format("#autoLOC_6001903");
			flag = true;
		}
		else if (incInput.Value < -180.0)
		{
			text += Localizer.Format("#autoLOC_6001904");
			flag = true;
		}
		if (double.IsInfinity(smaInput.Value))
		{
			text += Localizer.Format("#autoLOC_6001905");
			flag = true;
		}
		errorText.text = text;
		if (!flag)
		{
			FlightGlobals.fetch.SetShipOrbit(selectedBody, eccInput.Value, smaInput.Value, incInput.Value, lanInput.Value, mnaInput.Value, lpeInput.Value, obtInput.Value);
			FloatingOrigin.ResetTerrainShaderOffset();
		}
	}

	private void OnRendezvousClick()
	{
		bool flag = false;
		string text = string.Empty;
		Vessel vessel = SelectedVessel;
		if (FlightGlobals.ActiveVessel == null)
		{
			text += Localizer.Format("#autoLOC_6001899");
			flag = true;
		}
		if (vessel == null)
		{
			text += Localizer.Format("#autoLOC_6001906");
			flag = true;
		}
		else
		{
			if (vessel == FlightGlobals.ActiveVessel)
			{
				text += Localizer.Format("#autoLOC_6001907");
				flag = true;
			}
			if ((vessel.loaded ? vessel.situation : vessel.protoVessel.situation) != Vessel.Situations.ORBITING)
			{
				text += Localizer.Format("#autoLOC_6001908");
				flag = true;
			}
		}
		errorText.text = text;
		if (!flag)
		{
			ScreenMessages.PostScreenMessage(Localizer.Format("#autoLOC_6001909", rendezvousDistance, vessel.vesselName), 5f, ScreenMessageStyle.UPPER_CENTER);
			Vector3 vector = UnityEngine.Random.onUnitSphere * rendezvousDistance;
			FlightGlobals.fetch.SetShipOrbitRendezvous(vessel, vector, Vector3.zero);
		}
	}

	private void OnBodyBackClick()
	{
		if (!(FlightGlobals.fetch == null))
		{
			selectedBody--;
			if (selectedBody < 0)
			{
				selectedBody = FlightGlobals.Bodies.Count - 1;
			}
			SetSelectedBodyText();
		}
	}

	private void OnBodyForwardClick()
	{
		if (!(FlightGlobals.fetch == null))
		{
			selectedBody++;
			if (selectedBody >= FlightGlobals.Bodies.Count)
			{
				selectedBody = 0;
			}
			SetSelectedBodyText();
		}
	}

	private void SetSelectedBodyText()
	{
		CelestialBody celestialBody = SelectedBody;
		bodyNameText.text = ((celestialBody != null) ? Localizer.Format("#autoLOC_7001301", celestialBody.displayName) : string.Empty);
	}

	private void OnVesselBackClick()
	{
		if (!(FlightGlobals.fetch == null))
		{
			selectedVessel--;
			if (selectedVessel < 0)
			{
				selectedVessel = FlightGlobals.Vessels.Count - 1;
			}
			SetSelectedVesselText();
		}
	}

	private void OnVesselForwardClick()
	{
		if (!(FlightGlobals.fetch == null))
		{
			selectedVessel++;
			if (selectedVessel >= FlightGlobals.Vessels.Count)
			{
				selectedVessel = 0;
			}
			SetSelectedVesselText();
		}
	}

	private void SetSelectedVesselText()
	{
		Vessel vessel = SelectedVessel;
		vesselNameText.text = ((vessel != null) ? vessel.vesselName : string.Empty);
	}
}
