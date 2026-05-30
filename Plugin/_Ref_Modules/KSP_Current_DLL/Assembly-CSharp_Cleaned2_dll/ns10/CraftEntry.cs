using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Expansions;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ns9;

namespace ns10;

public class CraftEntry : MonoBehaviour
{
	public string craftName;

	public string fullFilePath;

	private Callback<CraftEntry> OnSelected;

	public int partCount;

	public int stageCount;

	public bool isStock;

	public bool isValid;

	public bool steamItem;

	private ShipTemplate _template;

	public VersionCompareResult compatibility;

	public CraftProfileInfo craftProfileInfo;

	public Texture2D thumbnail;

	public string thumbURL;

	[SerializeField]
	private Toggle tgtCtrl;

	[SerializeField]
	private TextMeshProUGUI header1;

	[SerializeField]
	private TextMeshProUGUI header2;

	[SerializeField]
	private TextMeshProUGUI fieldStats;

	[SerializeField]
	private TextMeshProUGUI fieldCost;

	[SerializeField]
	private TextMeshProUGUI fieldMsg;

	[SerializeField]
	private RawImage craftThumbImg;

	[SerializeField]
	private GameObject imageSteam;

	private ConfigNode _configNode;

	public SteamCraftInfo steamCraftInfo;

	public ShipTemplate template
	{
		get
		{
			if (_template == null)
			{
				_template = new ShipTemplate();
				_template.LoadShip(configNode);
			}
			return _template;
		}
	}

	public Toggle Toggle => tgtCtrl;

	public ConfigNode configNode
	{
		get
		{
			if (_configNode == null)
			{
				_configNode = ConfigNode.Load(fullFilePath);
			}
			return _configNode;
		}
	}

	public static CraftEntry Create(FileInfo fInfo, bool stock, Callback<CraftEntry> OnSelected)
	{
		return Create(fInfo, stock, OnSelected, steamItem: false, null);
	}

	public static CraftEntry Create(FileInfo fInfo, bool stock, Callback<CraftEntry> OnSelected, bool steamItem, SteamCraftInfo steamCraftInfo)
	{
		CraftEntry component = UnityEngine.Object.Instantiate(AssetBase.GetPrefab("CraftBrowserWidget")).GetComponent<CraftEntry>();
		if (steamItem && fInfo != null)
		{
			component.imageSteam.SetActive(value: true);
		}
		component.OnSelected = OnSelected;
		component.Init(fInfo, stock, steamItem, steamCraftInfo);
		component.name = component.craftName + (stock ? Localizer.Format("#autoLOC_482705") : string.Empty);
		return component;
	}

	protected void Init(FileInfo fInfo, bool stock)
	{
		Init(fInfo, stock, steamItem: false, null);
	}

	protected void Init(FileInfo fInfo, bool stock, bool steamItem, SteamCraftInfo steamCraftInfo)
	{
		this.steamItem = steamItem;
		this.steamCraftInfo = steamCraftInfo;
		isStock = stock;
		bool canBeUsed = true;
		if (steamItem && steamCraftInfo != null)
		{
			craftProfileInfo = new CraftProfileInfo();
			craftName = (craftProfileInfo.shipName = steamCraftInfo.itemDetails.m_rgchTitle + " (" + Localizer.Format("#autoLOC_8002139") + ")");
			craftProfileInfo.steamPublishedFileId = steamCraftInfo.itemDetails.m_nPublishedFileId.m_PublishedFileId;
			craftProfileInfo.totalCost = steamCraftInfo.cost;
			craftProfileInfo.shipPartsExperimental = false;
			thumbURL = steamCraftInfo.previewURL;
			fullFilePath = steamCraftInfo.itemDetails.m_rgchURL;
			craftProfileInfo.partCount = steamCraftInfo.partCount;
			StartCoroutine(LoadSteamItemPreviewURL());
			craftProfileInfo.stageCount = steamCraftInfo.stageCount;
			craftProfileInfo.compatibility = KSPUtil.CheckVersion(steamCraftInfo.KSPversion, ShipConstruct.lastCompatibleMajor, ShipConstruct.lastCompatibleMinor, ShipConstruct.lastCompatibleRev);
			steamCraftInfo.UpdateSteamState();
			canBeUsed = steamCraftInfo.canBeUsed;
		}
		else
		{
			craftName = fInfo.Name.Replace(fInfo.Extension, "");
			fullFilePath = fInfo.FullName;
			string text = fInfo.FullName.Replace(fInfo.Extension, ".loadmeta");
			if (steamItem)
			{
				text = KSPSteamUtils.GetSteamCacheLocation(text);
			}
			craftProfileInfo = CraftProfileInfo.GetSaveData(fInfo.FullName, text);
			string text2 = new FileInfo(fullFilePath).Directory.Name;
			if (isStock)
			{
				if (fInfo.Directory.ToString().Contains("SquadExpansion"))
				{
					List<ExpansionsLoader.ExpansionInfo> installedExpansions = ExpansionsLoader.GetInstalledExpansions();
					string text3 = "";
					for (int i = 0; i < installedExpansions.Count; i++)
					{
						text3 = KSPExpansionsUtils.ExpansionsGameDataPath + installedExpansions[i].FolderName + "/Ships/@thumbs/" + text2 + "/" + KSPUtil.SanitizeFilename(Path.GetFileNameWithoutExtension(fullFilePath));
						if (fullFilePath.Contains(installedExpansions[i].FolderName))
						{
							thumbURL = text3;
						}
					}
				}
				else
				{
					thumbURL = "Ships/@thumbs/" + text2 + "/" + KSPUtil.SanitizeFilename(Path.GetFileNameWithoutExtension(fullFilePath));
				}
			}
			else
			{
				thumbURL = "thumbs/" + HighLogic.SaveFolder + "_" + text2 + "_" + KSPUtil.SanitizeFilename(Path.GetFileNameWithoutExtension(fullFilePath));
			}
			if (steamItem)
			{
				thumbURL = fInfo.DirectoryName + "/" + Path.GetFileNameWithoutExtension(fullFilePath);
				thumbnail = ShipConstruction.GetThumbnail(thumbURL, fullPath: true, addFileExt: true);
			}
			else
			{
				thumbnail = (fInfo.Directory.ToString().Contains("SquadExpansion") ? ShipConstruction.GetThumbnail(thumbURL, fullPath: true, addFileExt: true, fInfo) : ShipConstruction.GetThumbnail(thumbURL, fullPath: false, addFileExt: true, fInfo));
			}
			craftThumbImg.texture = thumbnail;
			if (steamItem)
			{
				string stateText = "";
				bool subscribed = false;
				SteamManager.Instance.GetItemState(new PublishedFileId_t(GetSteamFileId()), out stateText, out canBeUsed, out subscribed);
			}
		}
		isValid = craftProfileInfo.shipPartsUnlocked && craftProfileInfo.shipPartModulesAvailable;
		partCount = craftProfileInfo.partCount;
		stageCount = craftProfileInfo.stageCount;
		compatibility = craftProfileInfo.compatibility;
		tgtCtrl.interactable = canBeUsed;
		tgtCtrl.onValueChanged.AddListener(onValueChanged);
		UIUpdate(steamCraftInfo);
	}

	private IEnumerator LoadSteamItemPreviewURL()
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(steamCraftInfo.previewURL);
		try
		{
			yield return www.SendWebRequest();
			while (!www.isDone)
			{
				yield return null;
			}
			if (!www.isNetworkError && !www.isHttpError && string.IsNullOrEmpty(www.error))
			{
				thumbnail = DownloadHandlerTexture.GetContent(www);
				craftThumbImg.texture = thumbnail;
				yield break;
			}
			Debug.LogWarning("Texture load error in " + steamCraftInfo.previewURL + "': " + www.error);
		}
		finally
		{
			((IDisposable)www)?.Dispose();
		}
	}

	public void UIUpdate()
	{
		UIUpdate(null);
	}

	public void UIUpdate(SteamCraftInfo steamCraftInfo)
	{
		if (craftProfileInfo.shipName != craftName)
		{
			TextMeshProUGUI textMeshProUGUI = header1;
			string text2 = (header2.text = Localizer.Format(craftProfileInfo.shipName) + (isStock ? Localizer.Format("#autoLOC_482705") : string.Empty) + " [" + craftName + "]");
			textMeshProUGUI.text = text2;
		}
		else
		{
			TextMeshProUGUI textMeshProUGUI2 = header1;
			string text2 = (header2.text = Localizer.Format(craftProfileInfo.shipName) + (isStock ? Localizer.Format("#autoLOC_482705") : string.Empty));
			textMeshProUGUI2.text = text2;
		}
		fieldStats.text = Localizer.Format("#autoLOC_452442", partCount, stageCount);
		fieldCost.text = GetCostTextColorTag(craftProfileInfo.totalCost) + Localizer.Format("#autoLOC_6003099", craftProfileInfo.totalCost.ToString("N2")) + "</color>";
		fieldMsg.text = string.Empty;
		if (compatibility == VersionCompareResult.COMPATIBLE)
		{
			if (isValid)
			{
				if (craftProfileInfo.shipPartsExperimental)
				{
					fieldMsg.text = "<color=#8dffec>  *** " + Localizer.Format("#autoLOC_6003094") + " ***</color>";
				}
				else
				{
					fieldMsg.text = string.Empty;
				}
			}
			else if (!craftProfileInfo.shipPartModulesAvailable && craftProfileInfo.shipPartsUnlocked)
			{
				fieldMsg.text = Localizer.Format("#autoLOC_8004266");
			}
			else
			{
				fieldMsg.text = "<color=#db6227>  *** " + Localizer.Format("#autoLOC_6003097") + " ***</color>";
			}
		}
		else
		{
			fieldMsg.text = "<color=#db6227>  *** " + Localizer.Format("#autoLOC_8004246") + " ***</color>";
		}
		if (steamItem)
		{
			UIUpdateSteamField(steamCraftInfo);
		}
	}

	public void UIUpdateSteamField(SteamCraftInfo steamCraftInfo)
	{
		if (string.IsNullOrEmpty(fieldMsg.text))
		{
			string stateText = "";
			bool canBeUsed = false;
			bool subscribed = false;
			if (steamCraftInfo != null)
			{
				stateText = steamCraftInfo.steamStateText;
				canBeUsed = steamCraftInfo.canBeUsed;
				subscribed = steamCraftInfo.subscribed;
			}
			else
			{
				SteamManager.Instance.GetItemState(new PublishedFileId_t(GetSteamFileId()), out stateText, out canBeUsed, out subscribed);
			}
			if (subscribed)
			{
				fieldMsg.text = Localizer.Format("#autoLOC_8002130");
			}
			else
			{
				fieldMsg.text = Localizer.Format("#autoLOC_8002131");
			}
			if (!canBeUsed)
			{
				fieldMsg.text = stateText;
			}
		}
	}

	private string GetCostTextColorTag(float cost)
	{
		if (Funding.CanAfford(cost))
		{
			return "<color=" + XKCDColors.HexFormat.KSPBadassGreen + ">";
		}
		return "<color=" + XKCDColors.HexFormat.KSPNotSoGoodOrange + ">";
	}

	protected void onValueChanged(bool st)
	{
		if (st)
		{
			OnSelected(this);
		}
	}

	public void Terminate()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public ulong GetSteamFileId()
	{
		if (steamCraftInfo != null)
		{
			return steamCraftInfo.itemDetails.m_nPublishedFileId.m_PublishedFileId;
		}
		if (craftProfileInfo != null)
		{
			return craftProfileInfo.steamPublishedFileId;
		}
		ulong value = 0uL;
		if (configNode != null)
		{
			configNode.TryGetValue("steamPublishedFileId", ref value);
			if (value == 0L)
			{
				value = KSPSteamUtils.GetSteamIDFromSteamFolder(fullFilePath);
			}
		}
		return value;
	}
}
