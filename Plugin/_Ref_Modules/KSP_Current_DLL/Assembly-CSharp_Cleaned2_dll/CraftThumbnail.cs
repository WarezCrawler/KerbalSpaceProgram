using System;
using System.IO;
using UnityEngine;

public static class CraftThumbnail
{
	private static float camFov = 30f;

	private static float camDist = 0f;

	private static Camera snapshotCamera;

	private static RenderTexture renderBuffer;

	public static EventData<ShipConstruct, string, byte[]> OnSnapshotCapture = new EventData<ShipConstruct, string, byte[]>("OnSnapshotCapture");

	internal static void TakeStockSnaphot(ShipConstruct ship, int resolution, string facilityName, bool expansion, string craftPath, float elevation = 45f, float azimuth = 45f, float pitch = 45f, float hdg = 45f, float fovFactor = 1f)
	{
		string text = "";
		string relativePath = KSPUtil.GetRelativePath(craftPath, KSPUtil.ApplicationRootPath);
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(craftPath);
		string[] separator = new string[1] { "Ships/" };
		string[] array = relativePath.Split(separator, StringSplitOptions.None);
		text = ((!expansion || array.Length == 0) ? ("Ships/@thumbs/" + facilityName) : (array[0] + "Ships/@thumbs/" + facilityName));
		TakeSnaphot(ship, resolution, text, fileNameWithoutExtension, elevation, azimuth, pitch, hdg, fovFactor);
	}

	public static void TakeSnaphot(ShipConstruct ship, int resolution, string folderPath, string craftName, float elevation = 45f, float azimuth = 45f, float pitch = 45f, float hdg = 45f, float fovFactor = 1f)
	{
		GameObject gameObject = new GameObject("SnapshotCamera");
		snapshotCamera = gameObject.AddComponent<Camera>();
		snapshotCamera.clearFlags = CameraClearFlags.Color;
		snapshotCamera.backgroundColor = Color.clear;
		snapshotCamera.fieldOfView = camFov;
		snapshotCamera.cullingMask = 1;
		snapshotCamera.enabled = false;
		renderBuffer = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.Default);
		camDist = KSPCameraUtil.GetDistanceToFit(ShipConstruction.CalculateCraftSize(ship), camFov * fovFactor);
		snapshotCamera.transform.position = ShipConstruction.FindCraftCenter(ship, excludeClamps: true) + Quaternion.AngleAxis(azimuth, Vector3.up) * Quaternion.AngleAxis(elevation, Vector3.right) * (Vector3.back * camDist);
		snapshotCamera.transform.rotation = Quaternion.AngleAxis(hdg, Vector3.up) * Quaternion.AngleAxis(pitch, Vector3.right);
		snapshotCamera.targetTexture = renderBuffer;
		snapshotCamera.Render();
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderBuffer;
		Texture2D texture2D = new Texture2D(resolution, resolution, TextureFormat.ARGB32, mipChain: false);
		texture2D.ReadPixels(new Rect(0f, 0f, resolution, resolution), 0, 0);
		byte[] array = ImageConversion.EncodeToPNG(texture2D);
		string text = KSPUtil.ApplicationRootPath + folderPath;
		if (craftName.Contains("/"))
		{
			int num = craftName.LastIndexOf("/");
			text = text + "/" + craftName.Substring(0, num);
			craftName = craftName.Substring(num + 1);
		}
		craftName = KSPUtil.SanitizeString(craftName, '_', replaceEmpty: true);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (Directory.Exists(text))
		{
			string text2 = text + "/" + craftName + ".png";
			try
			{
				File.WriteAllBytes(text2, array);
				OnSnapshotCapture.Fire(ship, text2, array);
			}
			catch (Exception ex)
			{
				Debug.LogError("[Thumbnail]: Error writing thumbnail with path " + text2 + ". Message: " + ex);
			}
		}
		else
		{
			Debug.LogError("[Thumbnail]: Error creating directory " + text);
		}
		RenderTexture.active = active;
		renderBuffer.Release();
		UnityEngine.Object.Destroy(gameObject);
		UnityEngine.Object.Destroy(renderBuffer);
		UnityEngine.Object.Destroy(texture2D);
	}
}
