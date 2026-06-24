using UnityEngine;
using System.Collections;

public class GameSfxPlayer : MonoBehaviour
{
    public static GameSfxPlayer Instance { get; private set; }

    [Header("Volume")]
    [Range(0f, 1f)] public float masterVolume = 1f;

    [Header("Combat")]
    [Range(0f, 1f)] public float hitDelayAfterAttack = 0.12f;
    [Range(0f, 1f)] public float deathDelayAfterHit = 0.16f;
    public AudioClip basicAttack;
    public AudioClip hit;
    public AudioClip heal;
    public AudioClip death;
    public AudioClip victory;
    public AudioClip defeat;

    [Header("UI")]
    public AudioClip buttonClick;

    [Header("World Map")]
    public AudioClip nodeTravel;

    [Header("Crafting")]
    public AudioClip craftSuccess;
    public AudioClip craftFail;

    [Header("Rest")]
    public AudioClip sleep;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        if (GetComponent<GlobalButtonSfxListener>() == null)
            gameObject.AddComponent<GlobalButtonSfxListener>();
    }

    public static GameSfxPlayer GetOrCreate()
    {
        if (Instance != null)
            return Instance;

        GameSfxPlayer existing = FindObjectOfType<GameSfxPlayer>();
        if (existing != null)
            return existing;

        GameObject go = new GameObject("GameSfxPlayer");
        return go.AddComponent<GameSfxPlayer>();
    }

    public void PlayBasicAttack()
    {
        Play(basicAttack);
    }

    public void PlayHit()
    {
        Play(hit);
    }

    public void PlayHitDelayed()
    {
        PlayDelayed(hit, hitDelayAfterAttack);
    }

    public void PlayHeal()
    {
        Play(heal);
    }

    public void PlayDeath()
    {
        Play(death);
    }

    public void PlayDeathDelayed()
    {
        PlayDelayed(death, hitDelayAfterAttack + deathDelayAfterHit);
    }

    public void PlayVictory()
    {
        Play(victory);
    }

    public void PlayDefeat()
    {
        Play(defeat);
    }

    public void PlayButtonClick()
    {
        Play(buttonClick);
    }

    public void PlayNodeTravel()
    {
        Play(nodeTravel);
    }

    public void PlayCraftSuccess()
    {
        Play(craftSuccess);
    }

    public void PlayCraftFail()
    {
        Play(craftFail);
    }

    public void PlaySleep()
    {
        Play(sleep);
    }

    private void Play(AudioClip clip)
    {
        if (clip == null)
            return;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            return;

        audioSource.PlayOneShot(clip, Mathf.Clamp01(masterVolume));
    }

    private void PlayDelayed(AudioClip clip, float delay)
    {
        if (clip == null)
            return;

        if (delay <= 0f)
        {
            Play(clip);
            return;
        }

        StartCoroutine(PlayAfterDelay(clip, delay));
    }

    private IEnumerator PlayAfterDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        Play(clip);
    }
}
