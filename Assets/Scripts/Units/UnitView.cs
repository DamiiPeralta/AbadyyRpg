using System.Collections;
using UnityEngine;

// Representa la visualización de una unidad en la escena
public class UnitView : MonoBehaviour
{
    // Referencia a la unidad de datos
    public Unit unit;

    // Componentes visuales
    private SpriteRenderer spriteRenderer;
    private Color aliveColor = Color.green;
    private Color deadColor = Color.red;

    // Flash control
    private Coroutine flashCoroutine;
    public float flashDuration = 0.25f;
    
    // Shake / Attack motion
    private Vector3 originalLocalPos;
    private Coroutine shakeCoroutine;
    private Coroutine attackCoroutine;
    public float shakeDuration = 0.25f;
    public float shakeMagnitude = 0.1f;
    public float attackDistance = 0.2f;
    public float attackDuration = 0.25f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"UnitView en {gameObject.name} necesita un SpriteRenderer");
        }
        originalLocalPos = transform.localPosition;
    }

    // Sincroniza el estado visual con los datos de la unidad
    public void UpdateVisuals()
    {
        if (unit == null || spriteRenderer == null)
            return;

        // Cambiar color según si está vivo o muerto
        spriteRenderer.color = unit.isAlive ? aliveColor : deadColor;

        // Mostrar información en el nombre del GameObject
        gameObject.name = $"{unit.unitName} ({unit.currentHP}/{unit.maxHP})";
    }

    // Inicializa la vista con una unidad
    public void SetUnit(Unit newUnit)
    {
        unit = newUnit;
        UpdateVisuals();
    }

    // Inicia un destello amarillo para indicar que es su turno
    public void FlashTurn()
    {
        StartFlash(Color.yellow);
    }

    // Inicia un destello blanco para indicar que ha sido atacado
    public void FlashHit()
    {
        StartFlash(Color.white);
    }

    // Sacudida breve al recibir daño
    public void ShakeOnHit()
    {
        StartShake();
    }

    // Movimiento rápido izquierda-derecha al atacar
    public void AttackMotion()
    {
        StartAttackMotion();
    }

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
        Color original = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        // Restaurar el color según el estado actual (vivo/muerto)
        UpdateVisuals();
        flashCoroutine = null;
    }

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

    private void StartAttackMotion()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        // Move left
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

        // Move to right (pass through original to right)
        t = 0f;
        while (t < segment * 2f)
        {
            transform.localPosition = Vector3.Lerp(leftPos, rightPos, t / (segment * 2f));
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = rightPos;

        // Return to original
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
}
