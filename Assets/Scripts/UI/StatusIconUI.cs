using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIconUI : MonoBehaviour
{
    [Header("Setup")]
    public GameObject iconPrefab;
    public Transform container;

    [Header("Sprites")]
    public Sprite poisonIcon;
    public Sprite regenIcon;
    public Sprite stunIcon;
    public Sprite invisIcon;
    public Sprite attackDownIcon;

    private List<GameObject> activeIcons = new List<GameObject>();

    public void Refresh(List<StatusEffect> effects)
    {
        ClearIcons();

        if (effects == null || effects.Count == 0)
            return;

        HashSet<StatusEffectType> added = new HashSet<StatusEffectType>();

        foreach (var effect in effects)
        {
            if (added.Contains(effect.type))
                continue;

            Sprite icon = GetIcon(effect.type);
            if (icon == null)
                continue;

            GameObject go = Instantiate(iconPrefab, container);
            Image img = go.GetComponent<Image>();

            if (img != null)
                img.sprite = icon;

            activeIcons.Add(go);
            added.Add(effect.type);
        }
    }

    private Sprite GetIcon(StatusEffectType type)
    {
        switch (type)
        {
            case StatusEffectType.Poison: return poisonIcon;
            case StatusEffectType.Regeneration: return regenIcon;
            case StatusEffectType.Stun: return stunIcon;
            case StatusEffectType.Invisibility: return invisIcon;
            case StatusEffectType.AttackDown: return attackDownIcon != null ? attackDownIcon : stunIcon;
        }

        return null;
    }

    private void ClearIcons()
    {
        foreach (var icon in activeIcons)
            Destroy(icon);

        activeIcons.Clear();
    }
}
