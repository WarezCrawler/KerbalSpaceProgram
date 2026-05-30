using UnityEngine;
using UnityEngine.UI;
using ns2;

namespace ns10;

public interface ApplauncherLayout
{
	GameObject GetGameObject();

	RectTransform GetRectTransform();

	LayoutElement GetTopRightSpacer();

	LayoutElement GetBottomLeftSpacer();

	UIList GetStockList();

	UIList GetModList();

	UIList GetKnowledgeBaseList();

	ScrollRect GetModScrollRect();

	Image GetModListDivider();

	PointerClickAndHoldHandler GetModListBtnUp();

	PointerClickAndHoldHandler GetModListBtnDown();
}
