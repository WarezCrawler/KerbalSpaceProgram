namespace KerbalEngineer.Unity.Flight;

public interface ISectionModule
{
	bool IsDeleted { get; }

	bool IsEditorVisible { get; set; }

	bool IsVisible { get; set; }

	bool IsHud { get; set; }

	string Name { get; }
}
