using UnityEngine;

namespace KerbalEngineer.Extensions;

public static class RectExtensions
{
	public static Rect ClampInsideScreen(this Rect value)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref value)).x = Mathf.Clamp(((Rect)(ref value)).x, 0f, (float)Screen.width - ((Rect)(ref value)).width);
		((Rect)(ref value)).y = Mathf.Clamp(((Rect)(ref value)).y, 0f, (float)Screen.height - ((Rect)(ref value)).height);
		return value;
	}

	public static Rect ClampToScreen(this Rect value, float margin = 25f)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref value)).x = Mathf.Clamp(((Rect)(ref value)).x, 0f - (((Rect)(ref value)).width - margin), (float)Screen.width - margin);
		((Rect)(ref value)).y = Mathf.Clamp(((Rect)(ref value)).y, 0f - (((Rect)(ref value)).height - margin), (float)Screen.height - margin);
		return value;
	}

	public static bool MouseIsOver(this Rect value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		return ((Rect)(ref value)).Contains(new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y));
	}

	public static Rect Translate(this Rect value, Rect rectangle)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref value)).x = ((Rect)(ref value)).x + ((Rect)(ref rectangle)).x;
		((Rect)(ref value)).y = ((Rect)(ref value)).y + ((Rect)(ref rectangle)).y;
		return value;
	}
}
