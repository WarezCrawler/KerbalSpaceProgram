using UnityEngine;
using ns22;

public interface ISiteNode
{
	string GetName();

	void UpdateNodeCaption(MapNode mn, MapNode.CaptionData data);

	Transform GetWorldPos();
}
