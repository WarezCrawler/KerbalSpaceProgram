using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Expansions;
using Expansions.Missions.Runtime;
using SaveUpgradePipeline;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ns15;
using ns2;
using ns9;

namespace ns10;

public class CraftBrowserDialog : MonoBehaviour
{
	public delegate void SelectFileCallback(string fullPath, LoadType t);

	public delegate void CancelledCallback();

	private enum SteamQueryFilters
	{
		VOTE,
		FEATURED,
		NEWEST,
		SUBSCRIBERS
	}

	public enum LoadType
	{
		Normal,
		Merge
	}

	protected List<CraftEntry> craftList;

	protected string craftSubfolder;

	protected EditorFacility facility;

	protected CraftEntry selectedEntry;

	public SelectFileCallback OnFileSelected;

	public CancelledCallback OnBrowseCancelled;

	protected string profile;

	protected bool showMergeOption;

	protected string title;

	[SerializeField]
	private TextMeshProUGUI header;

	[SerializeField]
	private Toggle tabVAB;

	[SerializeField]
	private Toggle tabSPH;

	[SerializeField]
	private Toggle tabSteam;

	[SerializeField]
	private Button btnCancel;

	[SerializeField]
	private Button btnLoad;

	[SerializeField]
	private Button btnMerge;

	[SerializeField]
	private Button btnDelete;

	[SerializeField]
	private GameObject SteamButtons;

	[SerializeField]
	private Button btnSteamOverlay;

	[SerializeField]
	private Button btnSteamItem;

	[SerializeField]
	private Button btnSteamSubUnsub;

	[SerializeField]
	private TextMeshProUGUI btnSteamSubUnsubText;

	[SerializeField]
	private Button btnMainSteamSubUnsub;

	[SerializeField]
	private TextMeshProUGUI btnMainSteamSubUnsubText;

	[SerializeField]
	private TextMeshProUGUI SteamLoadingText;

	[SerializeField]
	private GameObject SteamFilterHolder;

	[SerializeField]
	private Toggle SteamFilterToggleOne;

	[SerializeField]
	private Toggle SteamFilterToggleTwo;

	[SerializeField]
	private Toggle SteamFilterToggleThree;

	[SerializeField]
	private Toggle SteamFilterToggleFour;

	[SerializeField]
	private UIPanelTransition SteamDetailsPanel;

	[SerializeField]
	private TextMeshProUGUI noSteamItemSelectedText;

	[SerializeField]
	private GameObject SteamItemContent;

	[SerializeField]
	private TextMeshProUGUI AuthorTitleText;

	[SerializeField]
	private TextMeshProUGUI steamItemKSPVersionText;

	[SerializeField]
	private TextMeshProUGUI steamItemVesselTypeText;

	[SerializeField]
	private TextMeshProUGUI steamItemDescriptionText;

	[SerializeField]
	private TextMeshProUGUI steamItemAuthorText;

	[SerializeField]
	private TextMeshProUGUI steamItemModsText;

	[SerializeField]
	private TextMeshProUGUI steamItemFavouriteText;

	[SerializeField]
	private TextMeshProUGUI steamItemFileSizeText;

	[SerializeField]
	private TextMeshProUGUI steamItemUpVotesText;

	[SerializeField]
	private TextMeshProUGUI steamItemDownVotesText;

	[SerializeField]
	private TextMeshProUGUI steamItemSubscribersText;

	[SerializeField]
	private RawImage steamItemPreview;

	[SerializeField]
	private RectTransform scrollView;

	[SerializeField]
	private ToggleGroup listGroup;

	[SerializeField]
	private RectTransform listContainer;

	[SerializeField]
	protected UISkinDefSO uiSkin;

	protected UISkinDef skin;

	private PopupDialog window;

	private PopupDialog unableToSubscribeToSteam;

	private PopupDialog steamError;

	private PopupDialog vesselIsIncompatible;

	private UIConfirmDialog steamWorkshopItemUnsubscribe;

	private List<Selectable> baseSelectables = new List<Selectable>();

	private List<Selectable> totalSelectables = new List<Selectable>();

	private List<Selectable> loadedVesselsSelectables = new List<Selectable>();

	private MenuNavigation menuNavigation;

	private Navigation tabVabNav;

	private Navigation tabSphNav;

	private Navigation secondVesselNav;

	private Navigation steamToggleOneNav;

	private Navigation steamToggleTwoNav;

	private Navigation steamToggleThreeNav;

	private Navigation steamToggleFourNav;

	private Navigation btnCancelNav;

	private Navigation btnLoadNav;

	private Navigation btnMergeNav;

	private Navigation btnDeleteNav;

	private Navigation btnSteamOverlayNav;

	[SerializeField]
	private PublishedFileId_t[] steamSubscribedItems;

	private SteamQueryFilters selectedSteamQueryFilter;

	private bool steamTabOpen;

	private string authorNormalTitle = "";

	private string authorSteamTitle = "";

	public static CraftBrowserDialog Spawn(EditorFacility facility, string profile, SelectFileCallback onFileSelected, CancelledCallback onCancel, bool showMergeOption)
	{
		CraftBrowserDialog component = UnityEngine.Object.Instantiate(AssetBase.GetPrefab("CraftBrowser")).GetComponent<CraftBrowserDialog>();
		component.transform.SetParent(DialogCanvasUtil.DialogCanvasRect, worldPositionStays: false);
		component.facility = facility;
		component.showMergeOption = showMergeOption;
		component.OnBrowseCancelled = onCancel;
		component.OnFileSelected = onFileSelected;
		component.title = Localizer.Format("#autoLOC_900537");
		component.profile = profile;
		return component;
	}

	public void Dismiss()
	{
		if (SteamManager.Initialized)
		{
			SteamManager.Instance.RemoveRemoteSubUnsubEvents();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Start()
	{
		header.text = title;
		tabVAB.isOn = facility == EditorFacility.const_1;
		tabVAB.onValueChanged.AddListener(onVABtabToggle);
		tabSPH.isOn = facility == EditorFacility.const_2;
		tabSPH.onValueChanged.AddListener(onSPHtabToggle);
		if (SteamManager.Initialized)
		{
			tabSteam.gameObject.SetActive(value: true);
			tabSteam.isOn = false;
			tabSteam.onValueChanged.AddListener(onSteamtabToggle);
			btnSteamSubUnsub.onClick.AddListener(onButtonSteamSubUnsub);
			btnMainSteamSubUnsub.onClick.AddListener(onButtonSteamSubUnsub);
			SteamFilterToggleOne.onValueChanged.AddListener(onSteamFilterOne);
			SteamFilterToggleTwo.onValueChanged.AddListener(onSteamFilterTwo);
			SteamFilterToggleThree.onValueChanged.AddListener(onSteamFilterThree);
			SteamFilterToggleFour.onValueChanged.AddListener(onSteamFilterFour);
			btnSteamOverlay.onClick.AddListener(OnBtnSteamOverlay);
			btnSteamItem.onClick.AddListener(onBtnSteamItemDetails);
			UpdateSteamSubscribedItems();
		}
		if (showMergeOption)
		{
			btnMerge.onClick.AddListener(onButtonMerge);
		}
		else
		{
			btnMerge.gameObject.SetActive(value: false);
		}
		btnDelete.onClick.AddListener(onButtonDelete);
		btnCancel.onClick.AddListener(onButtonCancel);
		btnLoad.onClick.AddListener(onButtonLoad);
		craftSubfolder = ShipConstruction.GetShipsSubfolderFor(facility);
		Selectable[] componentsInChildren = base.gameObject.GetComponentsInChildren<Selectable>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			baseSelectables.Add(componentsInChildren[i]);
		}
		baseSelectables.Add(btnDelete);
		baseSelectables.Add(btnSteamOverlay);
		BuildCraftList();
		authorNormalTitle = Localizer.Format("#autoLOC_8006112");
		authorSteamTitle = Localizer.Format("#autoLOC_8002161");
		InitializeMenuNavigation();
		UpdateSelectables();
		UpdateExplicitNavigation();
	}

	public void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape) && !unableToSubscribeToSteam && !steamError && !vesselIsIncompatible && !steamWorkshopItemUnsubscribe)
		{
			onButtonCancel();
		}
	}

	private void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	protected void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	private void BuildCraftList()
	{
		SteamLoadingText.gameObject.SetActive(value: false);
		ClearCraftList();
		CraftProfileInfo.PrepareCraftMetaFileLoad();
		craftList = new List<CraftEntry>();
		if (HighLogic.CurrentGame.Parameters.Difficulty.AllowStockVessels && HighLogic.CurrentGame.Mode != Game.Modes.MISSION && HighLogic.CurrentGame.Mode != Game.Modes.MISSION_BUILDER)
		{
			string path = KSPUtil.ApplicationRootPath + "Ships/" + craftSubfolder;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			FileInfo[] files = new DirectoryInfo(path).GetFiles("*.craft");
			int num = files.Length;
			for (int i = 0; i < num; i++)
			{
				CraftEntry item = CraftEntry.Create(files[i], stock: true, OnEntrySelected);
				craftList.Add(item);
			}
			if (ExpansionsLoader.IsAnyExpansionInstalled())
			{
				List<ExpansionsLoader.ExpansionInfo> installedExpansions = ExpansionsLoader.GetInstalledExpansions();
				string text = "";
				for (int j = 0; j < installedExpansions.Count; j++)
				{
					text = KSPExpansionsUtils.ExpansionsGameDataPath + installedExpansions[j].FolderName + "/Ships/" + craftSubfolder;
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					FileInfo[] files2 = new DirectoryInfo(text).GetFiles("*.craft");
					num = files2.Length;
					for (int k = 0; k < num; k++)
					{
						CraftEntry item2 = CraftEntry.Create(files2[k], stock: true, OnEntrySelected);
						craftList.Add(item2);
					}
				}
			}
		}
		if (profile != null && profile != string.Empty)
		{
			string path2 = KSPUtil.ApplicationRootPath + "saves/" + profile + "/Ships/" + craftSubfolder;
			if (HighLogic.CurrentGame.Mode == Game.Modes.MISSION_BUILDER && ExpansionsLoader.IsExpansionInstalled("MakingHistory") && MissionSystem.missions.Count > 0)
			{
				path2 = MissionSystem.missions[0].MissionInfo.ShipFolderPath + craftSubfolder;
			}
			if (!Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
			}
			FileInfo[] files3 = new DirectoryInfo(path2).GetFiles("*.craft");
			int num2 = files3.Length;
			for (int l = 0; l < num2; l++)
			{
				CraftEntry item3 = CraftEntry.Create(files3[l], stock: false, OnEntrySelected);
				craftList.Add(item3);
			}
		}
		if (SteamManager.Initialized && !tabSteam.isOn)
		{
			craftList.AddRange(KSPSteamUtils.GatherCraftFiles(OnEntrySelected, excludeSteamUnsubscribed: true));
		}
		craftList.Sort(CraftCompare);
		int count = craftList.Count;
		for (int m = 0; m < count; m++)
		{
			AddCraftEntryWidget(craftList[m], listContainer);
		}
		UpdateSelectables();
	}

	private void UpdateSelectables()
	{
		if (menuNavigation != null)
		{
			menuNavigation.ResetSelectablesOnly();
			totalSelectables.Clear();
			totalSelectables.AddRange(baseSelectables);
			totalSelectables.AddRange(loadedVesselsSelectables);
			if (SteamManager.Initialized && steamTabOpen)
			{
				totalSelectables.Add(SteamFilterToggleOne);
				totalSelectables.Add(SteamFilterToggleTwo);
				totalSelectables.Add(SteamFilterToggleThree);
				totalSelectables.Add(SteamFilterToggleFour);
			}
			if (totalSelectables.Count > 0)
			{
				menuNavigation.SetSelectableItems(totalSelectables.ToArray(), Navigation.Mode.Automatic, hasText: false, resetNavMode: true);
			}
			UpdateExplicitNavigation();
		}
	}

	private void InitializeMenuNavigation()
	{
		menuNavigation = MenuNavigation.SpawnMenuNavigation(base.gameObject, Navigation.Mode.Automatic, SliderFocusType.Scrollbar, hasText: false, limitCheck: true);
		tabVabNav = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnRight = tabSPH,
			selectOnUp = btnCancel
		};
		tabVAB.navigation = tabVabNav;
		tabSphNav = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnLeft = tabVAB,
			selectOnRight = tabSteam,
			selectOnUp = btnCancel
		};
		tabVAB.navigation = tabSphNav;
		secondVesselNav = new Navigation
		{
			mode = Navigation.Mode.Explicit
		};
		btnLoadNav = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = tabSPH,
			selectOnLeft = btnCancel
		};
		btnCancelNav = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = tabSPH,
			selectOnLeft = btnMerge,
			selectOnRight = btnLoad
		};
		btnMergeNav = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = tabVAB,
			selectOnRight = btnCancel
		};
		btnDeleteNav = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = tabVAB,
			selectOnRight = btnCancel,
			selectOnLeft = btnMerge
		};
		if (SteamManager.Initialized)
		{
			btnSteamOverlayNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnDown = tabSPH,
				selectOnRight = btnCancel
			};
			steamToggleOneNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnRight = SteamFilterToggleTwo,
				selectOnUp = tabVAB
			};
			SteamFilterToggleOne.navigation = steamToggleOneNav;
			steamToggleTwoNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnLeft = SteamFilterToggleOne,
				selectOnRight = SteamFilterToggleThree,
				selectOnUp = tabSPH
			};
			SteamFilterToggleTwo.navigation = steamToggleTwoNav;
			steamToggleThreeNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnLeft = SteamFilterToggleTwo,
				selectOnRight = SteamFilterToggleFour,
				selectOnUp = tabSPH
			};
			SteamFilterToggleThree.navigation = steamToggleThreeNav;
			steamToggleFourNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnLeft = SteamFilterToggleThree,
				selectOnUp = tabSteam
			};
			SteamFilterToggleFour.navigation = steamToggleFourNav;
		}
	}

	private void UpdateExplicitNavigation()
	{
		if (loadedVesselsSelectables.Count <= 0)
		{
			return;
		}
		if (SteamManager.Initialized && steamTabOpen)
		{
			steamToggleOneNav.selectOnDown = loadedVesselsSelectables[0];
			steamToggleTwoNav.selectOnDown = loadedVesselsSelectables[0];
			steamToggleThreeNav.selectOnDown = loadedVesselsSelectables[0];
			steamToggleFourNav.selectOnDown = loadedVesselsSelectables[0];
			btnSteamOverlayNav.selectOnUp = loadedVesselsSelectables[loadedVesselsSelectables.Count - 1];
			btnCancelNav.selectOnUp = loadedVesselsSelectables[loadedVesselsSelectables.Count - 1];
			btnCancelNav.selectOnLeft = btnSteamOverlay;
			btnCancelNav.selectOnRight = btnSteamItem;
			SteamFilterToggleOne.navigation = steamToggleOneNav;
			SteamFilterToggleTwo.navigation = steamToggleTwoNav;
			SteamFilterToggleThree.navigation = steamToggleThreeNav;
			SteamFilterToggleFour.navigation = steamToggleFourNav;
			tabVabNav.selectOnDown = SteamFilterToggleOne;
			tabVabNav.selectOnUp = btnSteamOverlay;
			tabSphNav.selectOnDown = SteamFilterToggleTwo;
			tabSphNav.selectOnUp = btnCancel;
			tabSphNav.selectOnLeft = btnSteamOverlay;
			btnSteamOverlay.navigation = btnSteamOverlayNav;
			btnCancel.navigation = btnCancelNav;
			tabVAB.navigation = tabVabNav;
			tabSPH.navigation = tabSphNav;
		}
		else
		{
			tabVabNav.selectOnDown = loadedVesselsSelectables[0];
			tabVabNav.selectOnUp = btnMerge;
			tabSphNav.selectOnDown = loadedVesselsSelectables[0];
			tabSphNav.selectOnUp = btnCancel;
			btnCancelNav.selectOnUp = loadedVesselsSelectables[loadedVesselsSelectables.Count - 1];
			btnCancelNav.selectOnDown = tabSPH;
			btnCancelNav.selectOnLeft = btnMerge;
			btnCancelNav.selectOnRight = btnLoad;
			btnLoadNav.selectOnUp = loadedVesselsSelectables[loadedVesselsSelectables.Count - 1];
			btnMergeNav.selectOnUp = loadedVesselsSelectables[loadedVesselsSelectables.Count - 1];
			btnDeleteNav.selectOnUp = loadedVesselsSelectables[loadedVesselsSelectables.Count - 1];
			if (btnDelete.gameObject.activeInHierarchy)
			{
				btnMergeNav.selectOnRight = btnDelete;
				btnCancelNav.selectOnLeft = btnDelete;
			}
			tabVAB.navigation = tabVabNav;
			tabSPH.navigation = tabSphNav;
			btnLoad.navigation = btnLoadNav;
			btnCancel.navigation = btnCancelNav;
			btnMerge.navigation = btnMergeNav;
			btnDelete.navigation = btnDeleteNav;
		}
		if (loadedVesselsSelectables.Count >= 2)
		{
			secondVesselNav.selectOnUp = loadedVesselsSelectables[0];
			if (loadedVesselsSelectables.Count >= 3)
			{
				secondVesselNav.selectOnDown = loadedVesselsSelectables[2];
			}
			loadedVesselsSelectables[1].navigation = secondVesselNav;
		}
	}

	private static int CraftCompare(CraftEntry a, CraftEntry b)
	{
		return string.Compare(a.name, b.name, StringComparison.CurrentCulture);
	}

	protected void AddCraftEntryWidget(CraftEntry entry, RectTransform listParent)
	{
		listGroup.RegisterToggle(entry.Toggle);
		entry.Toggle.group = listGroup;
		entry.transform.SetParent(listParent, worldPositionStays: false);
		loadedVesselsSelectables.Add(entry.Toggle);
	}

	protected void ClearCraftList()
	{
		loadedVesselsSelectables.Clear();
		if (craftList != null)
		{
			int count = craftList.Count;
			while (count-- > 0)
			{
				CraftEntry craftEntry = craftList[count];
				listGroup.UnregisterToggle(craftEntry.Toggle);
				craftEntry.Terminate();
			}
			craftList.Clear();
			selectedEntry = null;
		}
		OnSelectionChanged(null);
	}

	protected void ClearSelection()
	{
		selectedEntry = null;
		OnSelectionChanged(null);
		listGroup.SetAllTogglesOff();
	}

	protected void OnEntrySelected(CraftEntry entry)
	{
		OnSelectionChanged(entry);
		if (selectedEntry == entry && Mouse.Left.GetDoubleClick(isDelegate: true))
		{
			pipeSelectedItem(selectedEntry, LoadType.Normal);
			Dismiss();
		}
		else
		{
			selectedEntry = entry;
		}
	}

	protected void OnSelectionChanged(CraftEntry selectedEntry)
	{
		setbottomButtons(selectedEntry, steamTabOpen);
		if (selectedEntry != null)
		{
			btnMerge.interactable = true;
			btnLoad.interactable = true;
			btnSteamSubUnsub.interactable = false;
			btnSteamItem.interactable = false;
			AuthorTitleText.text = authorNormalTitle;
			if (selectedEntry.steamItem && selectedEntry.steamCraftInfo != null)
			{
				AuthorTitleText.text = authorSteamTitle;
				noSteamItemSelectedText.gameObject.SetActive(value: false);
				SteamItemContent.SetActive(value: true);
				steamItemPreview.gameObject.SetActive(value: false);
				StartCoroutine(LoadSteamItemPreviewURL(selectedEntry));
				bool flag = SteamItemSubscribed(selectedEntry.steamCraftInfo.itemDetails.m_nPublishedFileId.m_PublishedFileId);
				btnSteamSubUnsub.interactable = true;
				btnSteamItem.interactable = true;
				btnSteamSubUnsubText.text = (flag ? Localizer.Format("#autoLOC_8002135") : Localizer.Format("#autoLOC_8002134"));
				steamItemKSPVersionText.text = selectedEntry.steamCraftInfo.KSPversion;
				steamItemVesselTypeText.text = selectedEntry.steamCraftInfo.vesselType;
				steamItemDescriptionText.text = selectedEntry.steamCraftInfo.itemDetails.m_rgchDescription;
				steamItemAuthorText.text = SteamManager.Instance.getUserName(selectedEntry.steamCraftInfo.itemDetails.m_ulSteamIDOwner, steamItemAuthorText);
				steamItemFavouriteText.text = selectedEntry.steamCraftInfo.totalFavorites.ToString();
				float num = (float)selectedEntry.steamCraftInfo.itemDetails.m_nFileSize / 1024f / 1024f;
				steamItemFileSizeText.text = num.ToString("N2");
				steamItemModsText.text = selectedEntry.steamCraftInfo.modsBriefing;
				steamItemDownVotesText.text = selectedEntry.steamCraftInfo.itemDetails.m_unVotesDown.ToString();
				steamItemUpVotesText.text = selectedEntry.steamCraftInfo.itemDetails.m_unVotesUp.ToString();
				steamItemSubscribersText.text = selectedEntry.steamCraftInfo.totalSubscriptions.ToString();
			}
			if (selectedEntry.steamItem && !steamTabOpen)
			{
				btnMainSteamSubUnsub.gameObject.SetActive(value: true);
				ulong steamFileId = selectedEntry.GetSteamFileId();
				if (steamFileId == 0L)
				{
					btnMainSteamSubUnsub.interactable = false;
					return;
				}
				btnMainSteamSubUnsub.interactable = true;
				bool flag2 = SteamItemSubscribed(steamFileId);
				btnMainSteamSubUnsubText.text = (flag2 ? Localizer.Format("#autoLOC_8002135") : Localizer.Format("#autoLOC_8002134"));
			}
			else
			{
				btnMainSteamSubUnsub.gameObject.SetActive(value: false);
				btnMainSteamSubUnsub.interactable = false;
			}
		}
		else
		{
			btnMerge.interactable = false;
			btnLoad.interactable = false;
			btnSteamSubUnsub.interactable = false;
			noSteamItemSelectedText.gameObject.SetActive(value: true);
			SteamItemContent.SetActive(value: false);
			btnSteamItem.interactable = false;
			btnMainSteamSubUnsub.gameObject.SetActive(value: false);
			btnMainSteamSubUnsub.interactable = false;
		}
	}

	private IEnumerator LoadSteamItemPreviewURL(CraftEntry selectedEntry)
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(selectedEntry.steamCraftInfo.previewURL);
		yield return www.SendWebRequest();
		while (!www.isDone)
		{
			yield return null;
		}
		if (!www.isNetworkError && !www.isHttpError)
		{
			steamItemPreview.texture = DownloadHandlerTexture.GetContent(www);
			steamItemPreview.gameObject.SetActive(value: true);
		}
		else
		{
			Debug.LogWarning("Texture load error in '" + selectedEntry.craftName + "': " + www.error);
		}
	}

	protected void onButtonLoad()
	{
		if (selectedEntry != null)
		{
			if (selectedEntry.compatibility == VersionCompareResult.COMPATIBLE && selectedEntry.isValid)
			{
				ConfirmLoadCraft();
				return;
			}
			if (selectedEntry.craftProfileInfo.shipPartsUnlocked && selectedEntry.craftProfileInfo.shipPartModulesAvailable)
			{
				vesselIsIncompatible = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("Confirmation Needed", Localizer.Format("#autoLOC_8004239"), Localizer.Format("#autoLOC_464288"), skin, 350f, new DialogGUIButton(Localizer.Format("#autoLOC_417274"), ConfirmLoadCraft, dismissOnSelect: true), new DialogGUIButton(Localizer.Format("#autoLOC_226976"), null, dismissOnSelect: true)), persistAcrossScenes: false, skin);
				return;
			}
			DialogGUILabel dialogGUILabel = new DialogGUILabel(selectedEntry.craftProfileInfo.GetErrorMessage());
			dialogGUILabel.bypassTextStyleColor = true;
			DialogGUIVerticalLayout dialogGUIVerticalLayout = new DialogGUIVerticalLayout(true, false, 0f, new RectOffset(4, 11, 4, 4), TextAnchor.UpperLeft, dialogGUILabel);
			dialogGUIVerticalLayout.AddChild(new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize));
			PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("Confirmation Needed", Localizer.Format("#autoLOC_8004267"), Localizer.Format("#autoLOC_464288"), skin, 350f, new DialogGUISpace(6f), new DialogGUIScrollList(new Vector2(-1f, 80f), hScroll: false, vScroll: true, dialogGUIVerticalLayout), new DialogGUISpace(6f), new DialogGUIHorizontalLayout(TextAnchor.UpperLeft, new DialogGUIFlexibleSpace(), new DialogGUIButton(Localizer.Format("#autoLOC_417274"), ConfirmLoadCraft, 100f, -1f, true), new DialogGUIButton(Localizer.Format("#autoLOC_226976"), null, 150f, -1f, true))), persistAcrossScenes: false, skin);
		}
	}

	protected void ConfirmLoadCraft()
	{
		if (!selectedEntry.craftProfileInfo.shipPartsUnlocked || !selectedEntry.craftProfileInfo.shipPartModulesAvailable)
		{
			Debug.LogWarning("Loading craft with the following issues:\n" + selectedEntry.craftProfileInfo.GetErrorMessage());
		}
		pipeSelectedItem(selectedEntry, LoadType.Normal);
		Dismiss();
	}

	private void onButtonMerge()
	{
		if (selectedEntry != null)
		{
			pipeSelectedItem(selectedEntry, LoadType.Merge);
			Dismiss();
		}
	}

	protected void onButtonCancel()
	{
		OnBrowseCancelled();
		Dismiss();
	}

	private void UpdateSteamSubscribedItems()
	{
		if (SteamManager.Initialized)
		{
			uint numberSubscribedItems = SteamManager.Instance.GetNumberSubscribedItems();
			steamSubscribedItems = SteamManager.Instance.GetSubscribedItems(numberSubscribedItems);
		}
	}

	private bool SteamItemSubscribed(ulong fileId)
	{
		int num = 0;
		while (true)
		{
			if (num < steamSubscribedItems.Length)
			{
				if (steamSubscribedItems[num].m_PublishedFileId == fileId)
				{
					break;
				}
				num++;
				continue;
			}
			return false;
		}
		return true;
	}

	private void UpdateSteamListOfCraft(SteamQueryFilters selectedQueryType)
	{
		ClearCraftList();
		EUGCQuery eUGCQuery = EUGCQuery.k_EUGCQuery_RankedByVote;
		string[] tags = new string[1] { "Craft" };
		switch (selectedQueryType)
		{
		default:
			eUGCQuery = EUGCQuery.k_EUGCQuery_RankedByVote;
			break;
		case SteamQueryFilters.FEATURED:
			eUGCQuery = EUGCQuery.k_EUGCQuery_RankedByVote;
			tags = new string[2] { "Craft", "Featured" };
			break;
		case SteamQueryFilters.NEWEST:
			eUGCQuery = EUGCQuery.k_EUGCQuery_RankedByPublicationDate;
			break;
		case SteamQueryFilters.SUBSCRIBERS:
			eUGCQuery = EUGCQuery.k_EUGCQuery_RankedByTotalUniqueSubscriptions;
			break;
		}
		getSteamWorkshopCraft(eUGCQuery, tags);
	}

	private void getSteamWorkshopCraft(EUGCQuery queryType, string[] tags)
	{
		SteamManager.Instance.QueryUGCItems(onSteamQueryItemsCallback, queryType, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, tags, 1u, returnMetadata: true);
	}

	private void onSteamQueryItemsCallback(SteamUGCQueryCompleted_t results, bool bIOFailure)
	{
		if (bIOFailure && results.m_eResult == EResult.k_EResultOK)
		{
			AnalyticsUtil.LogSteamError(AnalyticsUtil.steamActions.query, AnalyticsUtil.steamItemTypes.craft, 0uL, results.m_eResult);
			steamError = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("SteamError", Localizer.Format("#autoLOC_8002168", SteamManager.Instance.GetUGCFailureReason(results.m_eResult)), Localizer.Format("#autoLOC_8002125"), skin, 350f, new DialogGUIButton(Localizer.Format("#autoLOC_226975"), null, dismissOnSelect: true)), persistAcrossScenes: false, skin);
			SteamUGC.ReleaseQueryUGCRequest(results.m_handle);
			return;
		}
		SteamLoadingText.gameObject.SetActive(value: false);
		if (steamTabOpen)
		{
			loadedVesselsSelectables.Clear();
			for (int i = 0; i < results.m_unNumResultsReturned; i++)
			{
				SteamUGCDetails_t uGCItemDetails = SteamManager.Instance.GetUGCItemDetails(results.m_handle, (uint)i);
				if (uGCItemDetails.m_rgchTags.Contains("Craft") && (selectedSteamQueryFilter != SteamQueryFilters.FEATURED || uGCItemDetails.m_rgchTags.Contains("Featured")))
				{
					SteamCraftInfo steamCraftInfo = new SteamCraftInfo(uGCItemDetails);
					SteamUGC.GetQueryUGCStatistic(results.m_handle, (uint)i, EItemStatistic.k_EItemStatistic_NumFavorites, out steamCraftInfo.totalFavorites);
					SteamUGC.GetQueryUGCStatistic(results.m_handle, (uint)i, EItemStatistic.k_EItemStatistic_NumFollowers, out steamCraftInfo.totalFollowers);
					SteamUGC.GetQueryUGCStatistic(results.m_handle, (uint)i, EItemStatistic.k_EItemStatistic_NumSubscriptions, out steamCraftInfo.totalSubscriptions);
					SteamUGC.GetQueryUGCPreviewURL(results.m_handle, (uint)i, out steamCraftInfo.previewURL, (uint)uGCItemDetails.m_nPreviewFileSize);
					string pchMetadata = "";
					SteamUGC.GetQueryUGCMetadata(results.m_handle, (uint)i, out pchMetadata, 5000u);
					steamCraftInfo.ProcessMetaData(pchMetadata);
					steamCraftInfo.UpdateSteamState();
					CraftEntry item = CraftEntry.Create(null, stock: false, OnEntrySelected, steamItem: true, steamCraftInfo);
					craftList.Add(item);
				}
			}
			int count = craftList.Count;
			for (int j = 0; j < count; j++)
			{
				AddCraftEntryWidget(craftList[j], listContainer);
			}
			UpdateSelectables();
		}
		OnSelectionChanged(null);
		SteamUGC.ReleaseQueryUGCRequest(results.m_handle);
	}

	private void BuildSteamCraftList()
	{
		ClearCraftList();
		craftList = new List<CraftEntry>();
		UpdateSteamListOfCraft(selectedSteamQueryFilter);
		UpdateSteamSubscribedItems();
	}

	private void onButtonSteamSubUnsub()
	{
		if (!(selectedEntry != null))
		{
			return;
		}
		if (SteamItemSubscribed(selectedEntry.GetSteamFileId()))
		{
			steamWorkshopItemUnsubscribe = UIConfirmDialog.Spawn(Localizer.Format("#autoLOC_8002132"), Localizer.Format("#autoLOC_8002133"), Localizer.Format("#autoLOC_226976"), Localizer.Format("#autoLOC_226975"), Localizer.Format("#autoLOC_360842"), delegate(bool b)
			{
				SaveCraftSteamUnsubscribeWarning(b);
				SteamManager.Instance.unsubscribeItem(new PublishedFileId_t(selectedEntry.GetSteamFileId()), onUnsubscribeItemCallback);
			}, delegate
			{
			});
		}
		else
		{
			SteamManager.Instance.subscribeItem(new PublishedFileId_t(selectedEntry.GetSteamFileId()), onSubscribeItemCallback);
		}
	}

	private void onUnsubscribeItemCallback(RemoteStorageUnsubscribePublishedFileResult_t callResult, bool bIOFailure)
	{
		if (!bIOFailure && callResult.m_eResult == EResult.k_EResultOK)
		{
			AnalyticsUtil.LogSteamItemUnsubscribed(AnalyticsUtil.steamItemTypes.craft, callResult.m_nPublishedFileId);
			btnSteamSubUnsubText.text = Localizer.Format("#autoLOC_8002134");
			if (steamTabOpen)
			{
				UpdateSteamListOfCraft(selectedSteamQueryFilter);
			}
			else
			{
				BuildCraftList();
			}
			UpdateSteamSubscribedItems();
		}
		else
		{
			AnalyticsUtil.LogSteamError(AnalyticsUtil.steamActions.unsubscribe, AnalyticsUtil.steamItemTypes.craft, callResult.m_nPublishedFileId, callResult.m_eResult);
			unableToSubscribeToSteam = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("SteamError", Localizer.Format("#autoLOC_8002136", SteamManager.Instance.GetUGCFailureReason(callResult.m_eResult)), Localizer.Format("#autoLOC_8002125"), skin, 350f, new DialogGUIButton(Localizer.Format("#autoLOC_226975"), null, dismissOnSelect: true)), persistAcrossScenes: false, skin);
		}
	}

	private void onSubscribeItemCallback(RemoteStorageSubscribePublishedFileResult_t callResult, bool bIOFailure)
	{
		if (!bIOFailure && callResult.m_eResult == EResult.k_EResultOK)
		{
			AnalyticsUtil.LogSteamItemSubscribed(AnalyticsUtil.steamItemTypes.craft, callResult.m_nPublishedFileId);
			btnSteamSubUnsubText.text = Localizer.Format("#autoLOC_8002135");
			if (steamTabOpen)
			{
				UpdateSteamListOfCraft(selectedSteamQueryFilter);
			}
			else
			{
				BuildCraftList();
			}
			UpdateSteamSubscribedItems();
		}
		else
		{
			AnalyticsUtil.LogSteamError(AnalyticsUtil.steamActions.subscribe, AnalyticsUtil.steamItemTypes.craft, callResult.m_nPublishedFileId, callResult.m_eResult);
			unableToSubscribeToSteam = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("SteamError", Localizer.Format("#autoLOC_8002137", SteamManager.Instance.GetUGCFailureReason(callResult.m_eResult)), Localizer.Format("#autoLOC_8002125"), skin, 350f, new DialogGUIButton(Localizer.Format("#autoLOC_226975"), null, dismissOnSelect: true)), persistAcrossScenes: false, skin);
		}
	}

	private void SaveCraftSteamUnsubscribeWarning(bool dontShowAgain)
	{
		if (dontShowAgain == GameSettings.CRAFT_STEAM_UNSUBSCRIBE_WARNING)
		{
			GameSettings.CRAFT_STEAM_UNSUBSCRIBE_WARNING = !dontShowAgain;
			GameSettings.SaveGameSettingsOnly();
		}
	}

	private void onSteamFilterOne(bool value)
	{
		if (selectedSteamQueryFilter != 0)
		{
			SteamLoadingText.gameObject.SetActive(value: true);
			ClearCraftList();
			selectedSteamQueryFilter = SteamQueryFilters.VOTE;
			UpdateSteamListOfCraft(selectedSteamQueryFilter);
			UpdateSteamSubscribedItems();
		}
	}

	private void onSteamFilterTwo(bool value)
	{
		if (selectedSteamQueryFilter != SteamQueryFilters.FEATURED)
		{
			SteamLoadingText.gameObject.SetActive(value: true);
			ClearCraftList();
			selectedSteamQueryFilter = SteamQueryFilters.FEATURED;
			UpdateSteamListOfCraft(selectedSteamQueryFilter);
			UpdateSteamSubscribedItems();
		}
	}

	private void onSteamFilterThree(bool value)
	{
		if (selectedSteamQueryFilter != SteamQueryFilters.NEWEST)
		{
			SteamLoadingText.gameObject.SetActive(value: true);
			ClearCraftList();
			selectedSteamQueryFilter = SteamQueryFilters.NEWEST;
			UpdateSteamListOfCraft(selectedSteamQueryFilter);
			UpdateSteamSubscribedItems();
		}
	}

	private void onSteamFilterFour(bool value)
	{
		if (selectedSteamQueryFilter != SteamQueryFilters.SUBSCRIBERS)
		{
			SteamLoadingText.gameObject.SetActive(value: true);
			ClearCraftList();
			selectedSteamQueryFilter = SteamQueryFilters.SUBSCRIBERS;
			UpdateSteamListOfCraft(selectedSteamQueryFilter);
			UpdateSteamSubscribedItems();
		}
	}

	private void OnBtnSteamOverlay()
	{
		string text = "https://steamcommunity.com/workshop/browse/?appid=220200&searchtext=&childpublishedfileid=0&section=readytouseitems&days=-1";
		switch (selectedSteamQueryFilter)
		{
		case SteamQueryFilters.VOTE:
			text += "&section=readytouseitems&days=-1&browsesort=trend&requiredtags%5B%5D=Craft";
			break;
		case SteamQueryFilters.FEATURED:
			text += "&browsesort=trend&requiredtags%5B%5D=Craft&requiredtags%5B%5D=Featured";
			break;
		case SteamQueryFilters.NEWEST:
			text += "&section=readytouseitems&days=-1&browsesort=mostrecent&requiredtags%5B%5D=Craft";
			break;
		case SteamQueryFilters.SUBSCRIBERS:
			text += "&section=readytouseitems&days=-1&browsesort=totaluniquesubscribers&requiredtags%5B%5D=Craft";
			break;
		}
		SteamFriends.ActivateGameOverlayToWebPage(text);
	}

	private void onBtnSteamItemDetails()
	{
		string text = "steam://url/CommunityFilePage/" + selectedEntry.steamCraftInfo.itemDetails.m_nPublishedFileId.m_PublishedFileId;
		Debug.Log("Opening: " + text);
		SteamFriends.ActivateGameOverlayToWebPage(text);
	}

	private void remoteFileSubscribedCallback(RemoteStoragePublishedFileSubscribed_t result, bool bIOFailure)
	{
		if (!bIOFailure)
		{
			BuildCraftList();
		}
	}

	private void remoteFileUnsubscribedCallback(RemoteStoragePublishedFileUnsubscribed_t result, bool bIOFailure)
	{
		if (!bIOFailure)
		{
			BuildCraftList();
		}
	}

	private void pipeSelectedItem(CraftEntry sItem, LoadType loadType)
	{
		KSPUpgradePipeline.Process(sItem.configNode, sItem.name, LoadContext.Craft, delegate(ConfigNode n)
		{
			onPipelineFinished(n, sItem, loadType);
		}, delegate(KSPUpgradePipeline.UpgradeFailOption opt, ConfigNode n)
		{
			onPipelineFailed(opt, n, sItem, loadType);
		});
	}

	private void onPipelineFailed(KSPUpgradePipeline.UpgradeFailOption opt, ConfigNode n, CraftEntry sItem, LoadType loadType)
	{
		switch (opt)
		{
		case KSPUpgradePipeline.UpgradeFailOption.LoadAnyway:
			onPipelineFinished(n, sItem, loadType);
			break;
		case KSPUpgradePipeline.UpgradeFailOption.Cancel:
			OnBrowseCancelled();
			break;
		}
	}

	private void onPipelineFinished(ConfigNode n, CraftEntry sItem, LoadType loadType)
	{
		if (n != sItem.configNode)
		{
			sItem.configNode.Save(sItem.fullFilePath + ".original");
			n.Save(sItem.fullFilePath);
		}
		OnFileSelected(sItem.fullFilePath, loadType);
	}

	protected void onButtonDelete()
	{
		if (selectedEntry != null)
		{
			PromptDeleteFileConfirm();
		}
	}

	private void PromptDeleteFileConfirm()
	{
		Hide();
		if (window != null)
		{
			window.Dismiss();
		}
		DialogGUIHorizontalLayout dialogGUIHorizontalLayout = new DialogGUIHorizontalLayout();
		dialogGUIHorizontalLayout.AddChild(new DialogGUIFlexibleSpace());
		dialogGUIHorizontalLayout.AddChild(new DialogGUIButton("<color=orange>" + Localizer.Format("#autoLOC_129950") + "</color>", delegate
		{
			OnDeleteConfirm();
			ClearSelection();
			Show();
		}, 80f, 30f, true));
		dialogGUIHorizontalLayout.AddChild(new DialogGUIButton(Localizer.Format("#autoLOC_129951"), Show, 80f, 30f, true));
		DialogGUIVerticalLayout dialogGUIVerticalLayout = new DialogGUIVerticalLayout();
		dialogGUIVerticalLayout.AddChild(new DialogGUILabel(Localizer.Format("#autoLOC_7003238"), expandW: true));
		dialogGUIVerticalLayout.AddChild(dialogGUIHorizontalLayout);
		window = PopupDialog.SpawnPopupDialog(new MultiOptionDialog("DeleteFileConfirmation", "", Localizer.Format("#autoLOC_7003239"), skin, dialogGUIVerticalLayout), persistAcrossScenes: false, skin);
		window.OnDismiss = onButtonCancel;
	}

	private void OnDeleteConfirm()
	{
		string path = KSPUtil.ApplicationRootPath + selectedEntry.thumbURL + ".png";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		string path2 = selectedEntry.fullFilePath.Replace(".craft", ".loadmeta");
		if (File.Exists(path2))
		{
			File.Delete(path2);
		}
		File.Delete(selectedEntry.fullFilePath);
		BuildCraftList();
	}

	protected void onVABtabToggle(bool st)
	{
		if (st)
		{
			steamTabOpen = false;
			SteamLoadingText.gameObject.SetActive(value: false);
			SteamFilterHolder.SetActive(value: false);
			scrollView.offsetMax = new Vector2(scrollView.offsetMax.x, -56f);
			facility = EditorFacility.const_1;
			if (SteamManager.Initialized)
			{
				SteamDetailsPanel.Transition(0);
				SteamManager.Instance.RegisterRemoteSubUnsubEvents(remoteFileSubscribedCallback, remoteFileUnsubscribedCallback);
			}
			craftSubfolder = ShipConstruction.GetShipsSubfolderFor(facility);
			BuildCraftList();
		}
	}

	protected void onSPHtabToggle(bool st)
	{
		if (st)
		{
			steamTabOpen = false;
			SteamLoadingText.gameObject.SetActive(value: false);
			SteamFilterHolder.SetActive(value: false);
			scrollView.offsetMax = new Vector2(scrollView.offsetMax.x, -56f);
			facility = EditorFacility.const_2;
			if (SteamManager.Initialized)
			{
				SteamDetailsPanel.Transition(0);
				SteamManager.Instance.RegisterRemoteSubUnsubEvents(remoteFileSubscribedCallback, remoteFileUnsubscribedCallback);
			}
			craftSubfolder = ShipConstruction.GetShipsSubfolderFor(facility);
			BuildCraftList();
		}
	}

	protected void onSteamtabToggle(bool st)
	{
		if (st)
		{
			steamTabOpen = true;
			SteamLoadingText.gameObject.SetActive(value: true);
			SteamDetailsPanel.Transition(1);
			scrollView.offsetMax = new Vector2(scrollView.offsetMax.x, -82f);
			SteamFilterHolder.SetActive(value: true);
			BuildSteamCraftList();
			SteamManager.Instance.RemoveRemoteSubUnsubEvents();
		}
	}

	protected void setbottomButtons(CraftEntry selectedEntry, bool steamMode)
	{
		btnDelete.gameObject.SetActive(!steamMode && selectedEntry != null && !selectedEntry.isStock && !selectedEntry.steamItem);
		btnLoad.gameObject.SetActive(!steamMode);
		btnMerge.gameObject.SetActive(!steamMode);
		SteamButtons.SetActive(steamMode);
		UpdateExplicitNavigation();
	}
}
