using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ns31;

public class ShowInputLocks : MonoBehaviour
{
	public TextMeshProUGUI text;

	public Button clearButton;

	private void Awake()
	{
		clearButton.onClick.AddListener(OnClearClick);
	}

	private void Update()
	{
		string text = GetText();
		if (this.text.text != text)
		{
			this.text.text = text;
		}
	}

	private void OnClearClick()
	{
		InputLockManager.ClearControlLocks();
	}

	protected virtual string GetText()
	{
		return InputLockManager.PrintLockStack();
	}
}
