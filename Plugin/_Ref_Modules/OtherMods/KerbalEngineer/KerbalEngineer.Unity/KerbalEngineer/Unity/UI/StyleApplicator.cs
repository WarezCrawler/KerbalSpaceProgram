using UnityEngine;
using UnityEngine.UI;

namespace KerbalEngineer.Unity.UI;

public class StyleApplicator : MonoBehaviour
{
	public enum ElementTypes
	{
		None,
		Window,
		Box,
		Button,
		ButtonToggle,
		Label
	}

	[SerializeField]
	private ElementTypes elementType = ElementTypes.None;

	public ElementTypes ElementType => elementType;

	public void SetImage(Sprite sprite, Type type)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Image component = ((Component)this).GetComponent<Image>();
		if (!((Object)(object)component == (Object)null))
		{
			component.sprite = sprite;
			component.type = type;
		}
	}

	public void SetSelectable(TextStyle textStyle, Sprite normal, Sprite highlight, Sprite pressed, Sprite disabled)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		SetText(textStyle, ((Component)this).GetComponentInChildren<Text>());
		Selectable component = ((Component)this).GetComponent<Selectable>();
		if ((Object)(object)component != (Object)null)
		{
			component.image.sprite = normal;
			component.image.type = (Type)1;
			component.transition = (Transition)2;
			SpriteState spriteState = component.spriteState;
			((SpriteState)(ref spriteState)).highlightedSprite = highlight;
			((SpriteState)(ref spriteState)).pressedSprite = pressed;
			((SpriteState)(ref spriteState)).disabledSprite = disabled;
			component.spriteState = spriteState;
		}
	}

	public void SetText(TextStyle textStyle)
	{
		SetText(textStyle, ((Component)this).GetComponent<Text>());
	}

	public void SetToggle(TextStyle textStyle, Sprite normal, Sprite highlight, Sprite pressed, Sprite disabled)
	{
		SetSelectable(textStyle, normal, highlight, pressed, disabled);
		Toggle component = ((Component)this).GetComponent<Toggle>();
		if ((Object)(object)component != (Object)null)
		{
			Graphic graphic = component.graphic;
			Image val = (Image)(object)((graphic is Image) ? graphic : null);
			if ((Object)(object)val != (Object)null)
			{
				val.sprite = pressed;
				val.type = (Type)1;
			}
		}
	}

	private static void SetText(TextStyle textStyle, Text textComponent)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (textStyle != null && !((Object)(object)textComponent == (Object)null))
		{
			if ((Object)(object)textStyle.Font != (Object)null)
			{
				textComponent.font = textStyle.Font;
			}
			textComponent.fontSize = textStyle.Size;
			textComponent.fontStyle = textStyle.Style;
			((Graphic)textComponent).color = textStyle.Colour;
		}
	}
}
