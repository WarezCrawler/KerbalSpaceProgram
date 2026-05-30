using System;
using System.Collections;
using UnityEngine;

namespace ns2;

public class UIPanelTransitionManager : MonoBehaviour
{
	public UIPanelTransition[] panels;

	public void BringIn(int panel, Action onFinished = null)
	{
		int num = panels.Length;
		do
		{
			if (num-- <= 0)
			{
				return;
			}
		}
		while (num != panel);
		PanelTransitionIn(panels[num], onFinished);
	}

	public void BringIn(UIPanelTransition panel, Action onFinished = null)
	{
		StartCoroutine(PanelTransitionIn(panel, onFinished));
	}

	public void Dismiss(UIPanelTransition panel)
	{
		panel.Transition("Out");
	}

	private IEnumerator PanelTransitionIn(UIPanelTransition panel, Action onFinished = null)
	{
		if (panel.State == "In")
		{
			yield break;
		}
		int num = panels.Length;
		while (num-- > 0)
		{
			if (panels[num] != panel)
			{
				panels[num].Transition("Out");
			}
		}
		while (AnyPanelsTransitioning())
		{
			yield return null;
		}
		panel.Transition("In", onFinished);
	}

	private bool AnyPanelsTransitioning()
	{
		int num = panels.Length;
		do
		{
			if (num-- <= 0)
			{
				return false;
			}
		}
		while (!panels[num].Transitioning);
		return true;
	}
}
