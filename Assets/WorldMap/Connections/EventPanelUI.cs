using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelUI : MonoBehaviour
{
    public GameObject panel;

    public TextMeshProUGUI eventText;
    public Button option1Button;
    public Button option2Button;

    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;

    private WorldMapEvent currentEvent;

    public void ShowEvent(WorldMapEvent mapEvent)
    {
        currentEvent = mapEvent;

        panel.SetActive(true);

        eventText.text = mapEvent.eventText;

        option1Text.text = mapEvent.option1.optionText;
        option2Text.text = mapEvent.option2.optionText;

        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();

        option1Button.onClick.AddListener(() =>
        {
            WorldMapManager.Instance.ResolveOption(mapEvent.option1);
            Hide();
        });

        option2Button.onClick.AddListener(() =>
        {
            WorldMapManager.Instance.ResolveOption(mapEvent.option2);
            Hide();
        });
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}