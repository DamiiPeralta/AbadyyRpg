using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingCombatText : MonoBehaviour
{
    [Header("Text")]
    public TextMeshPro text;

    [Header("Timing")]
    public float lifetime = 1.2f;
    public float fadeStartTime = 0.5f;

    [Header("Motion")]
    public float verticalSpeed = 1.5f;

    [Header("Horizontal Random")]
    public float minAmplitude = 0.2f;
    public float maxAmplitude = 0.6f;

    public float minFrequency = 3f;
    public float maxFrequency = 7f;

    private float amplitude;
    private float frequency;
    private float phase;

    private Color originalColor;
    private Vector3 startPosition;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshPro>();
    }

    public void Initialize(string value, Color color)
    {
        if (text == null)
            return;

        text.text = value;
        text.color = color;
        originalColor = color;

        startPosition = transform.position;

        // 🔥 RANDOMIZACIÓN CLAVE
        amplitude = Random.Range(minAmplitude, maxAmplitude);
        frequency = Random.Range(minFrequency, maxFrequency);
        phase = Random.Range(0f, Mathf.PI * 2f);

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            float yOffset = verticalSpeed * elapsed;

            // movimiento lateral único por texto
            float xOffset = Mathf.Sin((elapsed * frequency) + phase) * amplitude;

            transform.position = startPosition + new Vector3(xOffset, yOffset, 0f);

            // Fade
            if (elapsed >= fadeStartTime && text != null)
            {
                float fadeProgress = (elapsed - fadeStartTime) / (lifetime - fadeStartTime);
                Color c = originalColor;
                c.a = Mathf.Lerp(1f, 0f, fadeProgress);
                text.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}