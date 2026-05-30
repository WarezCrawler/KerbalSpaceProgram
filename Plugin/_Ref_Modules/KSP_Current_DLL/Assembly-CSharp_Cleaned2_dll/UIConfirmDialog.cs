using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ns2;

public class UIConfirmDialog : MonoBehaviour
{
	[Header("UI Components")]
	[SerializeField]
	private TextMeshProUGUI textHeader;

	[SerializeField]
	private TextMeshProUGUI textDescription;

	[SerializeField]
	private TextMeshProUGUI textCancel;

	[SerializeField]
	private Button buttonCancel;

	[SerializeField]
	private TextMeshProUGUI textConfirmation;

	[SerializeField]
	private Button buttonConfirmation;

	[SerializeField]
	private TextMeshProUGUI textDontShowAgain;

	[SerializeField]
	private Toggle toggleDontShowAgain;

	public bool modal = true;

	private CanvasGroup canvasGroup;

	private Callback<bool> onOk;

	private Callback<bool> onCancel;

	public static UIConfirmDialog Spawn(string title, string message, Callback<bool> onOk, Callback<bool> onCancel, bool showCancelBtn = true)
	{
		UIConfirmDialog component = Object.Instantiate(AssetBase.GetPrefab("UIConfirmDialog")).GetComponent<UIConfirmDialog>();
		component.transform.SetParent(PopupDialogController.PopupDialogCanvas.transform, worldPositionStays: true);
		component.transform.localScale = Vector3.one;
		component.transform.localPosition = Vector3.zero;
		component.textHeader.text = title;
		component.textDescription.text = message;
		component.onOk = onOk;
		component.onCancel = onCancel;
		if (!showCancelBtn)
		{
			component.buttonCancel.gameObject.SetActive(value: false);
		}
		return component;
	}

	public static UIConfirmDialog Spawn(string title, string message, Callback<bool> onOk, bool showCancelBtn = true)
	{
		return Spawn(title, message, onOk, null, showCancelBtn);
	}

	public static UIConfirmDialog Spawn(string title, string message, string textCancel, string textOK, string textDontShowAgain, Callback<bool> onOk, Callback<bool> onCancel, bool showCancelBtn = true)
	{
		UIConfirmDialog uIConfirmDialog = Spawn(title, message, onOk, onCancel, showCancelBtn);
		uIConfirmDialog.textCancel.text = textCancel;
		uIConfirmDialog.textConfirmation.text = textOK;
		uIConfirmDialog.textDontShowAgain.text = textDontShowAgain;
		return uIConfirmDialog;
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			if (onCancel != null)
			{
				OnCancel();
			}
			else if (onOk != null)
			{
				OnConfirm();
			}
		}
	}

	private void Start()
	{
		buttonConfirmation.onClick.AddListener(delegate
		{
			OnConfirm();
		});
		buttonCancel.onClick.AddListener(delegate
		{
			OnCancel();
		});
		canvasGroup = GetComponent<CanvasGroup>();
		if (modal)
		{
			UIMasterController.Instance.RegisterModalDialog(canvasGroup);
		}
		else
		{
			UIMasterController.Instance.RegisterNonModalDialog(canvasGroup);
		}
	}

	public void OnConfirm()
	{
		if (onOk != null)
		{
			onOk(toggleDontShowAgain.isOn);
		}
		CloseDialog();
	}

	public void OnCancel()
	{
		if (onCancel != null)
		{
			onCancel(toggleDontShowAgain.isOn);
		}
		CloseDialog();
	}

	public void CloseDialog()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (modal)
		{
			UIMasterController.Instance.UnregisterModalDialog(canvasGroup);
		}
		else
		{
			UIMasterController.Instance.UnregisterNonModalDialog(canvasGroup);
		}
		buttonConfirmation.onClick.RemoveListener(delegate
		{
			OnConfirm();
		});
		buttonCancel.onClick.RemoveListener(delegate
		{
			OnCancel();
		});
	}
}
