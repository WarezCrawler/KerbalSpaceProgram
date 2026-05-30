namespace ns24;

public class ScreenDatabaseModels : ScreenDatabaseList
{
	protected override void LoadDatabaseListItems()
	{
		int i = 0;
		for (int count = GameDatabase.Instance.databaseModel.Count; i < count; i++)
		{
			textMeshQueue.AddLine(GameDatabase.Instance.databaseModel[i].name);
		}
	}
}
