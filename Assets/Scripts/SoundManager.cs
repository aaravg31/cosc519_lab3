using UnityEngine;

[DisallowMultipleComponent]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip appleSpawnClip;
    public AudioClip throwClip;
    public AudioClip hitClip;

    [Header("Volumes")]
    [Range(0f, 1f)] public float appleSpawnVolume = 0.8f;
    [Range(0f, 1f)] public float throwVolume = 0.9f;
    [Range(0f, 1f)] public float hitVolume = 1.0f;

    private AudioSource _audio;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _audio = GetComponent<AudioSource>();
        if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.spatialBlend = 0f;
        // Optional: persist across scene loads
        // DontDestroyOnLoad(gameObject);
    }

    public void PlaySpawn() { if (appleSpawnClip != null) _audio.PlayOneShot(appleSpawnClip, appleSpawnVolume); }
    public void PlayThrow() { if (throwClip != null) _audio.PlayOneShot(throwClip, throwVolume); }
    public void PlayHit()   { if (hitClip   != null) _audio.PlayOneShot(hitClip,   hitVolume); }
}