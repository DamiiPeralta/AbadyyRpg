using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalButtonSfxListener : MonoBehaviour
{
    private readonly HashSet<Button> subscribedButtons = new HashSet<Button>();
    private float nextScanTime;

    private void OnEnable()
    {
        ScanButtons();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextScanTime)
            return;

        nextScanTime = Time.unscaledTime + 0.5f;
        ScanButtons();
    }

    private void ScanButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button button in buttons)
        {
            if (button == null || subscribedButtons.Contains(button))
                continue;

            button.onClick.AddListener(PlayButtonClick);
            subscribedButtons.Add(button);
        }

        subscribedButtons.RemoveWhere(button => button == null);
    }

    private void PlayButtonClick()
    {
        if (GameSfxPlayer.Instance != null)
            GameSfxPlayer.Instance.PlayButtonClick();
    }
}
