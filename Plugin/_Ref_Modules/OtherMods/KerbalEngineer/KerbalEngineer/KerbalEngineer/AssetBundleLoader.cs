using UnityEngine;

namespace KerbalEngineer;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class AssetBundleLoader : MonoBehaviour
{
	private static AssetBundle images;

	private static AssetBundle prefabs;

	public static AssetBundle Images => images;

	public static AssetBundle Prefabs => prefabs;

	protected virtual void Start()
	{
		string text = EngineerGlobals.AssemblyPath + "/AssetBundles/";
		images = AssetBundle.LoadFromFile(text + "/images");
		prefabs = AssetBundle.LoadFromFile(text + "/prefabs");
		MyLogger.Log(images);
	}
}
