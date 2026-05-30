using UnityEngine;
using UnityEngine.EventSystems;

public class UINavMouseChecker : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
	public MenuNavigation menuNav;

	public int index;

	private bool mouseOver;

	public void SetMenuNavReference(MenuNavigation nav)
	{
		if (nav != null)
		{
			menuNav = nav;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!MenuNavigation.blockPointerEnterExit)
		{
			mouseOver = true;
			MouseIsHovering();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!MenuNavigation.blockPointerEnterExit)
		{
			mouseOver = false;
			MouseHasExited();
		}
	}

	private void MouseIsHovering()
	{
		if ((bool)menuNav)
		{
			menuNav.MouseIsHovering();
		}
	}

	private void MouseHasExited()
	{
		if ((bool)menuNav)
		{
			menuNav.SelectLastArrowSelected();
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if ((bool)menuNav && !mouseOver)
		{
			menuNav.SetLastItemSelected(index);
		}
	}
}
