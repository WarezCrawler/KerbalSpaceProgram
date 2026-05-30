using UnityEngine;
using ns9;

public class FlagDecal : PartModule
{
	[KSPField]
	public string textureQuadName = "";

	[KSPField(isPersistant = true)]
	public bool flagDisplayed = true;

	private Renderer textureQuadRenderer;

	private Texture2D flagTexture;

	[KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "#autoLOC_6001400")]
	public void ToggleFlag()
	{
		flagDisplayed = !flagDisplayed;
		UpdateDisplay();
	}

	public void UpdateDisplay()
	{
		if (textureQuadRenderer != null)
		{
			textureQuadRenderer.enabled = flagDisplayed;
		}
	}

	public override void OnAwake()
	{
		base.OnAwake();
	}

	public override void OnStart(StartState state)
	{
		UpdateFlagTexture();
		GameEvents.onMissionFlagSelect.Add(updateFlag);
		if (state == StartState.Editor)
		{
			updateFlag((EditorLogic.FlagURL == string.Empty) ? HighLogic.CurrentGame.flagURL : EditorLogic.FlagURL);
		}
		UpdateDisplay();
		if (base.part.variants != null)
		{
			GameEvents.onVariantApplied.Add(OnVariantApplied);
		}
	}

	private void OnDestroy()
	{
		GameEvents.onMissionFlagSelect.Remove(updateFlag);
		GameEvents.onVariantApplied.Remove(OnVariantApplied);
	}

	private void OnVariantApplied(Part appliedPart, PartVariant partVariant)
	{
		if (base.part == appliedPart)
		{
			if (HighLogic.LoadedSceneIsEditor)
			{
				updateFlag((EditorLogic.FlagURL == string.Empty) ? HighLogic.CurrentGame.flagURL : EditorLogic.FlagURL);
			}
			else if (HighLogic.LoadedSceneIsFlight)
			{
				UpdateFlagTexture();
			}
		}
	}

	private void updateFlag(string flagURL)
	{
		if (!HighLogic.LoadedSceneIsFlight)
		{
			flagTexture = GameDatabase.Instance.GetTexture(flagURL, asNormalMap: false);
			if (textureQuadRenderer != null && flagTexture != null)
			{
				textureQuadRenderer.material.mainTexture = flagTexture;
			}
		}
	}

	public void UpdateFlagTexture()
	{
		if (base.part.flagURL != string.Empty)
		{
			flagTexture = GameDatabase.Instance.GetTexture(base.part.flagURL, asNormalMap: false);
			if (flagTexture == null)
			{
				Debug.LogWarning("[FlagDecal Warning!]: Flag URL is given as " + base.part.flagURL + ", but no texture found in database with that name", base.gameObject);
			}
		}
		if (!(textureQuadName != string.Empty))
		{
			return;
		}
		Transform transform = base.part.FindModelTransform(textureQuadName);
		if (transform != null)
		{
			textureQuadRenderer = transform.GetComponent<Renderer>();
			if (textureQuadRenderer != null)
			{
				if (flagTexture != null)
				{
					textureQuadRenderer.material.mainTexture = flagTexture;
				}
			}
			else
			{
				Debug.LogWarning("[FlagDecal Warning!]: Flag quad object is given as " + textureQuadName + ", but has no renderer attached", transform.gameObject);
			}
		}
		else
		{
			Debug.LogWarning("[FlagDecal Warning!]: Flag quad object is given as " + textureQuadName + ", but no object found in model with that name", base.gameObject);
		}
	}

	public override string GetModuleDisplayName()
	{
		return Localizer.Format("#autoLoc_6003022");
	}
}
