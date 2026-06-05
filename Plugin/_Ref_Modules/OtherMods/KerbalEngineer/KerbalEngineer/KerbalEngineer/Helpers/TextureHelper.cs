using UnityEngine;

namespace KerbalEngineer.Helpers;

public static class TextureHelper
{
	public static Texture2D CreateTextureFromColour(Color colour)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		Texture2D val = new Texture2D(1, 1, (TextureFormat)5, false);
		val.SetPixel(1, 1, colour);
		val.Apply();
		return val;
	}
}
