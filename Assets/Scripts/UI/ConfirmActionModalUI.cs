using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmActionModalUI : MonoBehaviour
{
    public GameObject root;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public Button confirmButton;
    public Button cancelButton;
    public TMP_Text confirmButtonText;
    public TMP_Text cancelButtonText;

    private System.Action onConfirm;

    private void Awake()
    {
        AutoBind();
        Hide();
    }

    public void Show(string title, string body, System.Action onConfirm, string confirmLabel = "Confirmar", string cancelLabel = "Cancelar")
    {
        this.onConfirm = onConfirm;

        SetText(titleText, title);
        SetText(bodyText, body);
        SetText(confirmButtonText, confirmLabel);
        SetText(cancelButtonText, cancelLabel);

        HookButtons();

        if (root != null)
            root.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Hide()
    {
        onConfirm = null;

        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void Confirm()
    {
        System.Action action = onConfirm;
        Hide();
        action?.Invoke();
    }

    private void HookButtons()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(Confirm);
            confirmButton.onClick.AddListener(Confirm);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(Hide);
            cancelButton.onClick.AddListener(Hide);
        }
    }

    private void AutoBind()
    {
        if (root == null)
            root = gameObject;

        if (titleText == null)
            titleText = FindChild<TMP_Text>("Text_Title");

        if (bodyText == null)
            bodyText = FindChild<TMP_Text>("Text_Body");

        if (confirmButton == null)
            confirmButton = FindChild<Button>("Button_Confirm");

        if (cancelButton == null)
            cancelButton = FindChild<Button>("Button_Cancel");

        if (confirmButton != null && confirmButtonText == null)
            confirmButtonText = confirmButton.GetComponentInChildren<TMP_Text>(true);

        if (cancelButton != null && cancelButtonText == null)
            cancelButtonText = cancelButton.GetComponentInChildren<TMP_Text>(true);
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }

    private T FindChild<T>(string childName) where T : Component
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Trim() == childName)
                return child.GetComponent<T>();
        }

        return null;
    }
}
