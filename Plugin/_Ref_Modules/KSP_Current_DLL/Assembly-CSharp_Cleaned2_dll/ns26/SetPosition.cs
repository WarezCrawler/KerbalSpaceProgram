using System;
using EdyCommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ns25;
using ns35;
using ns9;

namespace ns26;

public class SetPosition : MonoBehaviour
{
	public Button bodyBackButton;

	public TextMeshProUGUI bodyNameText;

	public Button bodyForwardButton;

	public DebugScreenInputDouble latitudeInput;

	public DebugScreenInputDouble longitudeInput;

	public DebugScreenInputDouble altitudeInput;

	public DebugScreenInputDouble pitchInput;

	public DebugScreenInputDouble headingInput;

	public Slider easeInMultiplier;

	public TextMeshProUGUI easeInAmount;

	public Toggle easeToGroundToggle;

	public GameObject easeInStatusObject;

	public Button easeInDisableButton;

	public Toggle doNotPlaceUnderwaterToggle;

	public Button setSurfaceButton;

	public TextMeshProUGUI errorText;

	private int selectedBody;

	private NavBall navBall;

	private ScreenMessage easingInScreenMessage;

	private double altitudeSuggestedValue;

	private bool error;

	private string errorMsg;

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

	private void Awake()
	{
		bodyBackButton.onClick.AddListener(OnBodyBackClick);
		bodyForwardButton.onClick.AddListener(OnBodyForwardClick);
		easeInMultiplier.onValueChanged.AddListener(UpdateSliderValue);
		easeInDisableButton.onClick.AddListener(DisableActiveVesselEaseIn);
		setSurfaceButton.onClick.AddListener(OnSetPositionClick);
		altitudeInput.inputField.onEndEdit.AddListener(AltitudeModified);
		altitudeInput.inputField.onEndEdit.AddListener(FormatValues);
		pitchInput.inputField.onEndEdit.AddListener(FormatValues);
		headingInput.inputField.onEndEdit.AddListener(FormatValues);
		GameEvents.onLevelWasLoaded.Add(OnSceneExit);
		GameEvents.onVesselChange.Add(OnVesselChanged);
		GameEvents.onVesselSituationChange.Add(OnVesselSituationChanged);
	}

	private void Start()
	{
		if (FlightGlobals.fetch != null)
		{
			selectedBody = FlightGlobals.GetHomeBodyIndex();
			SetSelectedBodyText();
			InitializeValues();
			navBall = UnityEngine.Object.FindObjectOfType<NavBall>();
		}
		else
		{
			bodyNameText.text = string.Empty;
		}
		errorText.text = string.Empty;
		easingInScreenMessage = new ScreenMessage(Localizer.Format("#autoLOC_6003101"), 3000f, ScreenMessageStyle.UPPER_CENTER);
	}

	private void OnEnable()
	{
		InitializeValues();
	}

	private void OnDestroy()
	{
		altitudeInput.inputField.onEndEdit.RemoveListener(FormatValues);
		altitudeInput.inputField.onEndEdit.RemoveListener(AltitudeModified);
		pitchInput.inputField.onEndEdit.RemoveListener(FormatValues);
		headingInput.inputField.onEndEdit.RemoveListener(FormatValues);
		easeInMultiplier.onValueChanged.RemoveListener(UpdateSliderValue);
		easeInDisableButton.onClick.RemoveListener(DisableActiveVesselEaseIn);
		GameEvents.onLevelWasLoaded.Remove(OnSceneExit);
		GameEvents.onVesselChange.Remove(OnVesselChanged);
		GameEvents.onVesselSituationChange.Remove(OnVesselSituationChanged);
	}

	private void InitializeValues()
	{
		if (!(FlightGlobals.fetch == null) && !(FlightGlobals.ActiveVessel == null))
		{
			latitudeInput.Value = FlightGlobals.ActiveVessel.latitude;
			longitudeInput.Value = FlightGlobals.ActiveVessel.longitude;
			altitudeInput.Value = Math.Round(FlightGlobals.ActiveVessel.radarAltitude, 2);
			pitchInput.Value = Math.Round(FlightGlobals.ActiveVessel.ctrlState.pitch, 2);
			navBall = UnityEngine.Object.FindObjectOfType<NavBall>();
			if (navBall != null)
			{
				headingInput.Value = Math.Round(Quaternion.Inverse(navBall.relativeGymbal).eulerAngles.y, 2);
			}
			easeInAmount.text = easeInMultiplier.value.ToString();
		}
	}

	private void CheckForErrors()
	{
		error = false;
		string text = string.Empty;
		if (!(FlightGlobals.ActiveVessel == null) && FlightGlobals.ActiveVessel.state != Vessel.State.DEAD)
		{
			if (altitudeInput.Value < 0.0)
			{
				altitudeInput.Value = 0.0;
			}
			altitudeSuggestedValue = Mathf.Max(FlightGlobals.fetch.activeVessel.vesselSize.x, FlightGlobals.fetch.activeVessel.vesselSize.y, FlightGlobals.fetch.activeVessel.vesselSize.z);
			if (altitudeInput.Value <= altitudeSuggestedValue)
			{
				text += Localizer.Format("#autoLOC_6006032", Math.Round(FlightGlobals.fetch.activeVessel.vesselSize.x, 2), Math.Round(FlightGlobals.fetch.activeVessel.vesselSize.y, 2), Math.Round(FlightGlobals.fetch.activeVessel.vesselSize.z, 2), Math.Round(altitudeSuggestedValue, 2));
			}
			if (latitudeInput.Value > 90.0)
			{
				text += Localizer.Format("#autoLOC_8003379");
				latitudeInput.Value = 90.0;
			}
			else if (latitudeInput.Value < -90.0)
			{
				text += Localizer.Format("#autoLOC_8003380");
				latitudeInput.Value = -90.0;
			}
			if (longitudeInput.Value > 180.0)
			{
				text += Localizer.Format("#autoLOC_8003381");
				longitudeInput.Value = 180.0;
			}
			else if (longitudeInput.Value < -180.0)
			{
				text += Localizer.Format("#autoLOC_8003382");
				longitudeInput.Value = -180.0;
			}
			if (pitchInput.Value > 180.0)
			{
				text += Localizer.Format("#autoLOC_6006028");
				pitchInput.Value = MathUtility.ClampAngle((float)pitchInput.Value);
			}
			else if (pitchInput.Value < -180.0)
			{
				text += Localizer.Format("#autoLOC_6006029");
				pitchInput.Value = MathUtility.ClampAngle((float)pitchInput.Value);
			}
			if (headingInput.Value > 360.0)
			{
				text += Localizer.Format("#autoLOC_6006023");
				headingInput.Value = MathUtility.ClampAngle360((float)headingInput.Value);
			}
			else if (headingInput.Value < -360.0)
			{
				text += Localizer.Format("#autoLOC_6006024");
				headingInput.Value = MathUtility.ClampAngle360((float)headingInput.Value);
			}
			errorText.text = text;
		}
		else
		{
			text += Localizer.Format("#autoLOC_6001899");
			error = true;
		}
	}

	private void OnSetPositionClick()
	{
		CheckForErrors();
		if (!error)
		{
			FlightGlobals.fetch.SetVesselPosition(selectedBody, latitudeInput.Value, longitudeInput.Value, altitudeInput.Value, pitchInput.Value, headingInput.Value, doNotPlaceUnderwaterToggle.isOn, easeToGroundToggle.isOn, easeInMultiplier.value);
			CheckForHiddenElements();
			FloatingOrigin.ResetTerrainShaderOffset();
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
			CheckForHiddenElements();
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
			CheckForHiddenElements();
			SetSelectedBodyText();
		}
	}

	private void SetSelectedBodyText()
	{
		CelestialBody celestialBody = SelectedBody;
		bodyNameText.text = ((celestialBody != null) ? Localizer.Format("#autoLOC_7001301", celestialBody.displayName) : string.Empty);
	}

	private void OnVesselChanged(Vessel vessel)
	{
		CheckForHiddenElements();
		CheckForErrors();
	}

	private void OnVesselSituationChanged(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> data)
	{
		CheckForHiddenElements();
	}

	private void OnSceneExit(GameScenes scene)
	{
		CheckForHiddenElements();
	}

	private void CheckForHiddenElements()
	{
		if (FlightGlobals.Bodies[selectedBody].pqsController == null)
		{
			easeToGroundToggle.gameObject.SetActive(value: false);
		}
		else
		{
			easeToGroundToggle.gameObject.SetActive(value: true);
		}
		if (FlightGlobals.fetch != null && FlightGlobals.fetch.activeVessel != null && FlightGlobals.fetch.activeVessel.easingInToSurface)
		{
			easeInStatusObject.gameObject.SetActive(value: true);
			ScreenMessages.PostScreenMessage(easingInScreenMessage);
		}
		else
		{
			easeInStatusObject.gameObject.SetActive(value: false);
			ScreenMessages.RemoveMessage(easingInScreenMessage);
		}
	}

	private void FormatValues(string value)
	{
		altitudeInput.Value = Math.Round(altitudeInput.Value, 2);
		pitchInput.Value = Math.Round(pitchInput.Value, 2);
		headingInput.Value = Math.Round(headingInput.Value, 2);
	}

	private void UpdateSliderValue(float value)
	{
		easeInAmount.text = easeInMultiplier.value.ToString();
	}

	private void DisableActiveVesselEaseIn()
	{
		if (FlightGlobals.fetch != null && FlightGlobals.fetch.activeVessel != null)
		{
			FlightGlobals.fetch.ToggleVesselEaseIn(FlightGlobals.fetch.activeVessel, enableEaseIn: false);
		}
		CheckForHiddenElements();
	}

	private void AltitudeModified(string value)
	{
		CheckForErrors();
	}
}
