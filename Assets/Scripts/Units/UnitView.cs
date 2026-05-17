using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DamageFeedbackType
{
    PhysicalArmor,
    MagicalArmor,
    HP,
    Heal,
    Status,
    Miss,
    Immune,
    PhysicalArmorRestore,
    MagicalArmorRestore,
    Item
}

public enum AbilityVisualType
{
    Attack,
    Heal,
    Buff,
    Debuff,
    Status,
    None
}

public class UnitView : MonoBehaviour
{
    [Header("Runtime")]
    public Unit unit;

    [Header("Visual")]
    private SpriteRenderer spriteRenderer;
    private Color aliveColor = Color.white;
    private Color deadColor = Color.gray;
    private Color currentBaseColor;

    [Header("Status Pulse")]
    public bool enableStatusPulse = true;
    public float statusPulseInTime = 0.18f;
    public float statusPulseHoldTime = 0.08f;
    public float statusPulseOutTime = 0.18f;
    public float statusPulseRestTime = 0.12f;

    private Coroutine statusPulseCoroutine;
    private readonly List<Color> activeStatusColors = new List<Color>();
    private bool hasActiveStatusVisual;

    [Header("Status Icons UI")]
    public StatusIconUI statusIconUI;

    [Header("Floating Text")]
    public GameObject floatingTextPrefab;
    public float floatingTextDelay = 0.12f;

    [Header("Flash")]
    private Coroutine flashCoroutine;
    public float flashDuration = 0.25f;
    private bool isFlashing = false;

    [Header("Shake")]
    private Vector3 originalLocalPos;
    private Coroutine shakeCoroutine;
    public float shakeDuration = 0.25f;
    public float shakeMagnitude = 0.1f;

    [Header("Attack Motion")]
    private Coroutine attackCoroutine;
    public float attackDistance = 0.2f;
    public float attackDuration = 0.25f;

    [Header("Heal Motion")]
    private Coroutine healCoroutine;
    public float healFloatDistance = 0.15f;
    public float healDuration = 0.45f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogWarning($"UnitView en {gameObject.name} necesita un SpriteRenderer");

        originalLocalPos = transform.localPosition;
        currentBaseColor = aliveColor;
    }

    public void SetUnit(Unit newUnit)
    {
        unit = newUnit;
        UpdateVisuals();

        if (unit != null)
            RefreshStatusVisuals(unit.activeEffects);

        if (statusIconUI != null)
            statusIconUI.Refresh(unit.activeEffects);
    }

    public void UpdateVisuals()
    {
        if (unit == null || spriteRenderer == null)
            return;

        currentBaseColor = unit.isAlive ? aliveColor : deadColor;

        if (!hasActiveStatusVisual && !isFlashing)
            spriteRenderer.color = currentBaseColor;

        gameObject.name = $"{unit.unitName} ({unit.currentHP}/{unit.maxHP})";
    }

    // -------------------------
    // STATUS VISUALS
    // -------------------------

    public void RefreshStatusVisuals(List<StatusEffect> effects)
    {
        if (statusIconUI != null)
            statusIconUI.Refresh(effects);

        if (!enableStatusPulse || spriteRenderer == null)
            return;

        activeStatusColors.Clear();

        if (unit != null && !unit.isAlive)
        {
            StopStatusPulse();
            spriteRenderer.color = deadColor;
            return;
        }

        if (HasEffect(effects, StatusEffectType.Poison))
            activeStatusColors.Add(new Color(0.05f, 0.45f, 0.1f));

        if (HasEffect(effects, StatusEffectType.Stun))
            activeStatusColors.Add(Color.white);

        if (HasEffect(effects, StatusEffectType.Regeneration))
            activeStatusColors.Add(new Color(0.2f, 1f, 0.35f));

        if (HasEffect(effects, StatusEffectType.Invisibility))
            activeStatusColors.Add(new Color(0.45f, 0.45f, 0.65f));

        if (activeStatusColors.Count > 0)
            StartStatusPulse();
        else
            StopStatusPulse();
    }

    private bool HasEffect(List<StatusEffect> effects, StatusEffectType type)
    {
        if (effects == null)
            return false;

        foreach (StatusEffect effect in effects)
        {
            if (effect.type == type)
                return true;
        }

        return false;
    }

    private void StartStatusPulse()
    {
        hasActiveStatusVisual = true;

        if (statusPulseCoroutine == null)
            statusPulseCoroutine = StartCoroutine(StatusPulseCoroutine());
    }

    private void StopStatusPulse()
    {
        hasActiveStatusVisual = false;
        activeStatusColors.Clear();

        if (statusPulseCoroutine != null)
        {
            StopCoroutine(statusPulseCoroutine);
            statusPulseCoroutine = null;
        }

        if (spriteRenderer != null && !isFlashing)
            spriteRenderer.color = currentBaseColor;
    }

    private IEnumerator StatusPulseCoroutine()
    {
        int index = 0;

        while (hasActiveStatusVisual)
        {
            if (activeStatusColors.Count == 0)
            {
                StopStatusPulse();
                yield break;
            }

            Color statusColor = activeStatusColors[index % activeStatusColors.Count];

            yield return LerpSpriteColor(currentBaseColor, statusColor, statusPulseInTime);

            if (statusPulseHoldTime > 0f)
                yield return new WaitForSeconds(statusPulseHoldTime);

            yield return LerpSpriteColor(statusColor, currentBaseColor, statusPulseOutTime);

            if (statusPulseRestTime > 0f)
                yield return new WaitForSeconds(statusPulseRestTime);

            index++;
        }

        statusPulseCoroutine = null;
    }

    private IEnumerator LerpSpriteColor(Color from, Color to, float duration)
    {
        if (duration <= 0f)
        {
            if (!isFlashing && spriteRenderer != null)
                spriteRenderer.color = to;

            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!isFlashing && spriteRenderer != null)
            {
                float t = elapsed / duration;
                spriteRenderer.color = Color.Lerp(from, to, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!isFlashing && spriteRenderer != null)
            spriteRenderer.color = to;
    }

    // -------------------------
    // TURN / HIT FEEDBACK
    // -------------------------

    public void FlashTurn()
    {
        StartFlash(Color.yellow);
    }

    public void FlashHit(Color color)
    {
        StartFlash(color);
    }

    public void ShakeOnHit()
    {
        StartShake();
    }

    public void AttackMotion()
    {
        StartAttackMotion();
    }

    public void HealMotion()
    {
        StartHealMotion();
    }

    public void PlayAbilityVisual(AbilityVisualType visualType)
    {
        switch (visualType)
        {
            case AbilityVisualType.Attack:
                AttackMotion();
                break;

            case AbilityVisualType.Heal:
            case AbilityVisualType.Buff:
                HealMotion();
                break;

            case AbilityVisualType.Debuff:
            case AbilityVisualType.Status:
                FlashHit(new Color(0.6f, 0.2f, 1f));
                break;
        }
    }

    // -------------------------
    // FLOATING TEXT API
    // -------------------------

    public void ShowDamageText(int amount, DamageFeedbackType type)
    {
        if (amount <= 0)
            return;

        string prefix =
        type == DamageFeedbackType.Heal ||
        type == DamageFeedbackType.PhysicalArmorRestore ||
        type == DamageFeedbackType.MagicalArmorRestore
        ? "+"
        : "-";
        ShowFloatingText($"{prefix}{amount}", GetFeedbackColor(type));
    }

    public void ShowStatusText(string text)
    {
        ShowFloatingText(text, GetFeedbackColor(DamageFeedbackType.Status));
    }
    public void ShowItemText(string itemName)
    {
        ShowFloatingText("ITEM: " + itemName.ToUpper(), new Color(1f, 0.65f, 0f));
    }
    public void ShowMissText()
    {
        ShowFloatingText("MISS", GetFeedbackColor(DamageFeedbackType.Miss));
    }

    public void ShowImmuneText()
    {
        ShowFloatingText("IMMUNE", GetFeedbackColor(DamageFeedbackType.Immune));
    }

    public void ShowDamageBreakdown(int physArmor, int magArmor, int hpDamage)
    {
        StartCoroutine(ShowDamageBreakdownCoroutine(physArmor, magArmor, hpDamage));
    }

    private IEnumerator ShowDamageBreakdownCoroutine(int physArmor, int magArmor, int hpDamage)
    {
        if (physArmor > 0)
        {
            ShowDamageText(physArmor, DamageFeedbackType.PhysicalArmor);
            yield return new WaitForSeconds(floatingTextDelay);
        }

        if (magArmor > 0)
        {
            ShowDamageText(magArmor, DamageFeedbackType.MagicalArmor);
            yield return new WaitForSeconds(floatingTextDelay);
        }

        if (hpDamage > 0)
            ShowDamageText(hpDamage, DamageFeedbackType.HP);
    }

    private void ShowFloatingText(string text, Color color)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} no tiene floatingTextPrefab asignado");
            return;
        }

        Vector3 spawnPos = GetFloatingTextSpawnPosition();

        GameObject instance = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

        FloatingCombatText floatingText = instance.GetComponent<FloatingCombatText>();

        if (floatingText != null)
        {
            floatingText.Initialize(text, color);
            return;
        }

        TextMeshPro tmp = instance.GetComponent<TextMeshPro>();

        if (tmp != null)
        {
            tmp.text = text;
            tmp.color = color;
        }

        Destroy(instance, 1.2f);
    }

    private Vector3 GetFloatingTextSpawnPosition()
    {
        if (spriteRenderer != null)
        {
            float yOffset = spriteRenderer.bounds.size.y * 0.6f;
            return transform.position + Vector3.up * yOffset;
        }

        return transform.position + Vector3.up * 1f;
    }

    private Color GetFeedbackColor(DamageFeedbackType type)
    {
        return type switch
        {
            DamageFeedbackType.Item => new Color(1f, 0.65f, 0f),
            DamageFeedbackType.PhysicalArmor => Color.gray,
            DamageFeedbackType.MagicalArmor => Color.blue,
            DamageFeedbackType.HP => Color.red,
            DamageFeedbackType.Heal => Color.green,
            DamageFeedbackType.Status => new Color(0.6f, 0.2f, 1f),
            DamageFeedbackType.Miss => Color.white,
            DamageFeedbackType.Immune => Color.cyan,
            DamageFeedbackType.PhysicalArmorRestore => Color.gray,
            DamageFeedbackType.MagicalArmorRestore => Color.blue,
            _ => Color.white
        };
    }

    // -------------------------
    // FLASH
    // -------------------------

    private void StartFlash(Color flashColor)
    {
        if (spriteRenderer == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine(flashColor));
    }

    private IEnumerator FlashCoroutine(Color flashColor)
    {
        isFlashing = true;

        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);

        isFlashing = false;

        UpdateVisuals();

        if (unit != null)
            RefreshStatusVisuals(unit.activeEffects);

        flashCoroutine = null;
    }

    // -------------------------
    // SHAKE
    // -------------------------

    private void StartShake()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = (Vector3)Random.insideUnitCircle * shakeMagnitude;
            transform.localPosition = originalLocalPos + new Vector3(randomOffset.x, randomOffset.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        shakeCoroutine = null;
    }

    // -------------------------
    // ATTACK MOTION
    // -------------------------

    private void StartAttackMotion()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        Vector3 leftPos = originalLocalPos + Vector3.left * attackDistance;
        Vector3 rightPos = originalLocalPos + Vector3.right * attackDistance;

        float segment = attackDuration / 3f;

        float t = 0f;
        while (t < segment)
        {
            transform.localPosition = Vector3.Lerp(originalLocalPos, leftPos, t / segment);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = leftPos;

        t = 0f;
        while (t < segment * 2f)
        {
            transform.localPosition = Vector3.Lerp(leftPos, rightPos, t / (segment * 2f));
            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = rightPos;

        t = 0f;
        while (t < segment)
        {
            transform.localPosition = Vector3.Lerp(rightPos, originalLocalPos, t / segment);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        attackCoroutine = null;
    }

    // -------------------------
    // HEAL MOTION
    // -------------------------

    private void StartHealMotion()
    {
        if (healCoroutine != null)
            StopCoroutine(healCoroutine);

        healCoroutine = StartCoroutine(HealCoroutine());
    }

    private IEnumerator HealCoroutine()
    {
        Vector3 upPos = originalLocalPos + Vector3.up * healFloatDistance;

        float halfDuration = healDuration / 2f;

        float t = 0f;
        while (t < halfDuration)
        {
            transform.localPosition = Vector3.Lerp(originalLocalPos, upPos, t / halfDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = upPos;

        t = 0f;
        while (t < halfDuration)
        {
            transform.localPosition = Vector3.Lerp(upPos, originalLocalPos, t / halfDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        healCoroutine = null;
    }
}