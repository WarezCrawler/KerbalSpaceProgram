using System.IO;
using System.Reflection;

namespace KerbalEngineer;

public static class EngineerGlobals
{
	public const string ASSEMBLY_VERSION = "1.1.7.1";

	private static string assemblyFile;

	private static string assemblyName;

	private static string assemblyPath;

	private static string settingsPath;

	public static string AssemblyFile => assemblyFile ?? (assemblyFile = Assembly.GetExecutingAssembly().Location);

	public static string AssemblyName => assemblyName ?? (assemblyName = new FileInfo(AssemblyFile).Name);

	public static string AssemblyPath => assemblyPath ?? (assemblyPath = AssemblyFile.Replace(new FileInfo(AssemblyFile).Name, ""));

	public static string SettingsPath
	{
		get
		{
			if (string.IsNullOrEmpty(settingsPath))
			{
				settingsPath = Path.Combine(AssemblyPath, "Settings");
			}
			return settingsPath;
		}
	}
}
