using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Expansions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ns9;

public class WhatsNewDialog : MonoBehaviour
{
	[SerializeField]
	private RawImage bannerIcon;

	[SerializeField]
	private Button moreInfoButton;

	[SerializeField]
	private Button closeButton;

	[SerializeField]
	private Toggle dontShowAgainToggle;

	[SerializeField]
	private TMP_Text textBody;

	[SerializeField]
	private ScrollRect bodyScroll;

	[SerializeField]
	private float scrollStep;

	[SerializeField]
	private Button merchButton;

	public WhatsNewModes currentMode;

	private bool dontShowAgain = true;

	private TMP_Text moreInfoText;

	[SerializeField]
	private string merchandiseURL;

	private void Start()
	{
		moreInfoText = moreInfoButton.GetComponentInChildren<TMP_Text>();
		moreInfoButton.onClick.AddListener(OnMoreInfoButton);
		closeButton.onClick.AddListener(OnCloseButton);
		dontShowAgainToggle.onValueChanged.AddListener(OnDontShowAgainToggle);
		currentMode = WhatsNewModes.WhatsNew;
		dontShowAgainToggle.isOn = true;
		merchButton.onClick.AddListener(OnMerchButton);
		MenuNavigation.SpawnMenuNavigation(base.gameObject, Navigation.Mode.Automatic, hasText: true, limitCheck: true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Dismiss();
		}
		if (Input.GetKey(KeyCode.PageDown))
		{
			bodyScroll.verticalNormalizedPosition -= scrollStep;
		}
		else if (Input.GetKey(KeyCode.PageUp))
		{
			bodyScroll.verticalNormalizedPosition += scrollStep;
		}
	}

	private void OnDestroy()
	{
		moreInfoButton.onClick.RemoveListener(OnMoreInfoButton);
		closeButton.onClick.RemoveListener(OnCloseButton);
		dontShowAgainToggle.onValueChanged.RemoveListener(OnDontShowAgainToggle);
		merchButton.onClick.RemoveListener(OnMerchButton);
	}

	public WhatsNewDialog Create()
	{
		WhatsNewDialog whatsNewDialog = UnityEngine.Object.Instantiate(this);
		whatsNewDialog.gameObject.SetActive(value: true);
		whatsNewDialog.transform.position = Vector3.zero;
		whatsNewDialog.transform.SetParent(DialogCanvasUtil.DialogCanvasRect, worldPositionStays: false);
		InputLockManager.SetControlLock(ControlTypes.MAIN_MENU, "whatsNewDialog");
		return whatsNewDialog;
	}

	protected void Dismiss()
	{
		InputLockManager.RemoveControlLock("whatsNewDialog");
		if (dontShowAgain)
		{
			GameSettings.SHOW_WHATSNEW_DIALOG = !dontShowAgain;
			GameSettings.SaveSettings();
		}
		if (GameSettings.SHOW_WHATSNEW_DIALOG_VersionsShown.Split(',').IndexOf(VersioningBase.GetVersionString()) < 0)
		{
			GameSettings.SHOW_WHATSNEW_DIALOG_VersionsShown = GameSettings.SHOW_WHATSNEW_DIALOG_VersionsShown + ((GameSettings.SHOW_WHATSNEW_DIALOG_VersionsShown != "") ? "," : "") + VersioningBase.GetVersionString();
			GameSettings.SaveGameSettingsOnly();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDontShowAgainToggle(bool value)
	{
		dontShowAgain = value;
	}

	private void OnCloseButton()
	{
		Dismiss();
	}

	private void OnMoreInfoButton()
	{
		if (currentMode == WhatsNewModes.WhatsNew)
		{
			currentMode = WhatsNewModes.ChangeLog;
			bannerIcon.gameObject.SetActive(value: false);
			textBody.text = "<b><color=#ffbc00ff><size=18>" + Localizer.Format("#autoLOC_6005010") + ":</size></color></b>\n" + GetChangeLogText();
			moreInfoText.text = "#autoLOC_900211";
		}
		else
		{
			currentMode = WhatsNewModes.WhatsNew;
			bannerIcon.gameObject.SetActive(value: true);
			textBody.text = "#autoLOC_8003363";
			moreInfoText.text = "#autoLOC_6005010";
		}
		bodyScroll.verticalNormalizedPosition = 1f;
	}

	private void OnMerchButton()
	{
		Process.Start(merchandiseURL);
	}

	public string GetChangeLogText()
	{
		string text = "";
		string readmePath = Path.Combine(KSPUtil.ApplicationRootPath, "readme.txt");
		text = ParseReadMeFile(readmePath, VersioningBase.GetVersionString());
		List<ExpansionsLoader.ExpansionInfo> installedExpansions = ExpansionsLoader.GetInstalledExpansions();
		for (int i = 0; i < installedExpansions.Count; i++)
		{
			string readmePath2 = Path.Combine(KSPUtil.ApplicationRootPath, "GameData/SquadExpansion/" + installedExpansions[i].FolderName + "/readme.txt");
			text = text + "<b><color=#ffbc00ff><size=16>" + installedExpansions[i].DisplayName + " " + Localizer.Format("#autoLOC_6005012") + ":</size></color></b>\n";
			text += ParseReadMeFile(readmePath2, installedExpansions[i].Version);
		}
		return text;
	}

	public string ParseReadMeFile(string readmePath, string relevantVersion)
	{
		int num = 0;
		string text = "";
		Version versionFromString = Versioning.GetVersionFromString(relevantVersion);
		int num2 = versionFromString.Build;
		if (File.Exists(readmePath))
		{
			StreamReader streamReader = File.OpenText(readmePath);
			int num3 = 0;
			string text2;
			while ((text2 = streamReader.ReadLine()) != null)
			{
				if (text2.StartsWith("===="))
				{
					if (text2.IndexOf("= v" + versionFromString.Major + "." + versionFromString.Minor) <= 0)
					{
						if (num2 < 0)
						{
							break;
						}
						continue;
					}
					int num4 = text2.IndexOf("= v" + versionFromString.Major + "." + versionFromString.Minor);
					string text3 = text2.Substring(num4 + 7, 1);
					if (text3 == num2.ToString())
					{
						text = text + "<color=#ff9600ff>" + Localizer.Format("#autoLOC_6005015", versionFromString.Major.ToString(), versionFromString.Minor.ToString(), num2.ToString()) + "</color>";
						text += ((text3 != "0") ? Localizer.Format("#autoLOC_6005013", versionFromString.Major.ToString(), versionFromString.Minor.ToString()) : "");
						num = num3;
						num2--;
					}
				}
				if (num > 0 && num3 > num)
				{
					if (text2.Contains("+++"))
					{
						text2 = text2.Replace("+++", "<b>");
						text2 += "</b>";
					}
					text = text + text2 + "\n";
				}
				num3++;
			}
		}
		else
		{
			text = Localizer.Format("#autoLOC_6005009") + "\n\n";
		}
		return text;
	}
}
