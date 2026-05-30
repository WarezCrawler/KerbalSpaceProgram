using System;
using System.Collections.Generic;
using System.IO;
using Contracts.Agents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ns9;

public class FlagBrowser : MonoBehaviour
{
	[Serializable]
	public class FlagEntry
	{
		public GameDatabase.TextureInfo textureInfo;

		public string name;
	}

	public delegate void FlagSelectedCallback(FlagEntry selected);

	public List<FlagEntry> Flags;

	public FlagEntry selected;

	public string uiSkinName = "FlagBrowserSkin";

	private UISkinDef skin;

	private PopupDialog dialog;

	private MenuNavigation menuNav;

	public Rect windowRect;

	private Vector2 scrollPos;

	public float width = 360f;

	public float height = 400f;

	public float iconSize = 64f;

	public float iconSpacing = 8f;

	public FlagSelectedCallback OnFlagSelected = delegate
	{
	};

	public Callback OnDismiss = delegate
	{
	};

	private void Start()
	{
		skin = UISkinManager.GetSkin(uiSkinName);
		windowRect.width = width;
		windowRect.height = height;
		if (GameDatabase.Instance != null)
		{
			List<GameDatabase.TextureInfo> list = new List<GameDatabase.TextureInfo>();
			list = GameDatabase.Instance.GetAllTexturesInFolderType("Flags", caseInsensitive: true);
			Flags.Clear();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				GameDatabase.TextureInfo textureInfo = list[i];
				FlagEntry flagEntry = new FlagEntry();
				flagEntry.textureInfo = textureInfo;
				flagEntry.name = Path.GetFileNameWithoutExtension(textureInfo.name);
				Flags.Add(flagEntry);
			}
		}
		if (AgentList.Instance != null)
		{
			int count2 = AgentList.Instance.Agencies.Count;
			for (int j = 0; j < count2; j++)
			{
				Agent agent = AgentList.Instance.Agencies[j];
				bool flag = false;
				int count3 = Flags.Count;
				while (count3-- > 0)
				{
					if (Flags[count3].textureInfo.texture == agent.Logo)
					{
						flag = true;
					}
					if (Flags[count3].textureInfo.texture == agent.LogoScaled)
					{
						Flags.RemoveAt(count3);
					}
				}
				if (!flag)
				{
					FlagEntry flagEntry2 = new FlagEntry();
					flagEntry2.textureInfo = GameDatabase.Instance.GetTextureInfo(agent.LogoURL);
					flagEntry2.name = agent.Name;
					if (flagEntry2.textureInfo != null)
					{
						Flags.Add(flagEntry2);
					}
				}
			}
		}
		selected = null;
		dialog = CreateFlagBrowser();
		dialog.OnDismiss = Dismiss;
		menuNav = MenuNavigation.SpawnMenuNavigation(dialog.gameObject, Navigation.Mode.Automatic, hasText: false, limitCheck: true, SliderFocusType.Scrollbar);
	}

	private PopupDialog CreateFlagBrowser()
	{
		List<DialogGUIToggleButton> list = new List<DialogGUIToggleButton>();
		int i = 0;
		for (int count = Flags.Count; i < count; i++)
		{
			FlagEntry f = Flags[i];
			DialogGUIToggleButton dialogGUIToggleButton = new DialogGUIToggleButton(set: false, "", delegate
			{
				selected = f;
				if (Mouse.Left.GetDoubleClick(isDelegate: true) || menuNav.SumbmitOnSelectedToggle())
				{
					Accept(selected);
				}
			});
			DialogGUILabel dialogGUILabel = new DialogGUILabel(Flags[i].name, skin.toggle);
			dialogGUILabel.textLabelOptions = new DialogGUILabel.TextLabelOptions();
			dialogGUILabel.textLabelOptions.enableWordWrapping = false;
			dialogGUILabel.textLabelOptions.OverflowMode = TextOverflowModes.Ellipsis;
			DialogGUIVerticalLayout image = new DialogGUIVerticalLayout(true, false, 0f, new RectOffset(4, 4, 8, 8), TextAnchor.MiddleCenter, new DialogGUIImage(new Vector2(-1f, -1f), Vector2.zero, Color.white, Flags[i].textureInfo.texture), dialogGUILabel);
			image.children[1].OnResize = delegate
			{
				image.children[1].uiItem.GetComponent<LayoutElement>().preferredHeight = 18f;
			};
			dialogGUIToggleButton.AddChild(image);
			list.Add(dialogGUIToggleButton);
		}
		Vector2 size = new Vector2(445f, 485f);
		RectOffset padding = new RectOffset(2, 8, 4, 4);
		Vector2 cellSize = new Vector2(iconSize, iconSize);
		Vector2 spacing = new Vector2(4f, 4f);
		DialogGUIBase[] obj = new DialogGUIBase[2]
		{
			new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, useParentSize: true),
			null
		};
		DialogGUIToggle[] toggles = list.ToArray();
		obj[1] = new DialogGUIToggleGroup(toggles);
		DialogGUIScrollList dialogGUIScrollList = new DialogGUIScrollList(size, hScroll: false, vScroll: true, new DialogGUIGridLayout(padding, cellSize, spacing, GridLayoutGroup.Corner.UpperLeft, GridLayoutGroup.Axis.Horizontal, TextAnchor.UpperLeft, GridLayoutGroup.Constraint.FixedColumnCount, 6, obj));
		return PopupDialog.SpawnPopupDialog(Vector2.one * 0.5f, Vector2.one * 0.5f, new MultiOptionDialog("FlagBrowser", "", Localizer.Format("#autoLOC_364495"), skin, new Rect(0.5f, 0.5f, 455f, height), dialogGUIScrollList, new DialogGUIHorizontalLayout(false, false, 8f, new RectOffset(), TextAnchor.MiddleRight, new DialogGUIButton(Localizer.Format("#autoLOC_190768"), delegate
		{
			Dismiss();
		}, 90f, 30f, true), new DialogGUIButton(Localizer.Format("#autoLOC_190328"), delegate
		{
			Accept(selected);
		}, () => selected != null, 90f, 30f, dismissOnSelect: true))), persistAcrossScenes: false, skin);
	}

	public void Dismiss()
	{
		OnDismiss();
		dialog.Dismiss();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Accept(FlagEntry sel)
	{
		OnFlagSelected(sel);
		Debug.Log("Selected Flag " + sel.name);
		dialog.Dismiss();
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
