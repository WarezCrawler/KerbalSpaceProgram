using System.Linq;
using System.Text.RegularExpressions;

namespace KerbalEngineer.Flight.Presets;

public class Preset
{
	public string Abbreviation { get; set; }

	public string FileName => Regex.Replace(Name, "[^\\d\\w]", string.Empty) + ".xml";

	public bool IsHud { get; set; }

	public bool IsHudBackground { get; set; }

	public string Name { get; set; }

	public string[] ReadoutNames { get; set; }

	public override string ToString()
	{
		return Name + ReadoutNames.Select((string r) => "\n\t" + r);
	}
}
