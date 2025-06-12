using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
  public static SoundManager Instance;

  [Header("BGM")]
  [SerializeField] private AudioSource bgmSource;
  private float bgmVolume = 1f;
  private float bgmFadeDuration = 0.5f;

  [Header("SFX")]
  [SerializeField] private AudioSource sfxSourcePrefab;
  [SerializeField] private int sfxPoolSize = 5;
  [SerializeField] private List<AudioClip> soundEffects;
  private float sfxVolume = 1f;

  private Dictionary<string, AudioClip> sfxMap;
  private Queue<AudioSource> sfxSources;
  private Coroutine bgmFadeCoroutine;

  private void Awake()
  {
    Instance = this;
  }

  void Start()
  {
    sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);

    sfxMap = new Dictionary<string, AudioClip>();
    foreach (AudioClip clip in soundEffects)
    {
      if (clip != null && !sfxMap.ContainsKey(clip.name))
      {
        sfxMap.Add(clip.name, clip);
      }
    }

    sfxSources = new Queue<AudioSource>();
    for (int i = 0; i < sfxPoolSize; i++)
    {
      AudioSource sfx = Instantiate(sfxSourcePrefab, transform);
      sfx.playOnAwake = false;
      sfx.loop = false;
      sfx.volume = sfxVolume;
      sfxSources.Enqueue(sfx);
    }

    bgmSource.volume = bgmVolume;
  }

  // ---------------- BGM ----------------

  public void PlayBGM(AudioClip clip)
  {
    // If null or same clip, continue playing
    if (clip == null || bgmSource.clip == clip) return;

    if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
    bgmFadeCoroutine = StartCoroutine(FadeToBGM(clip));
  }

  public void StopBGM()
  {
    if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
    if (bgmSource.isPlaying && bgmSource.clip != null)
    {
      bgmFadeCoroutine = StartCoroutine(FadeOutAndStopBGM());
    }
  }

  private IEnumerator FadeOutAndStopBGM()
  {
    float elapsed = 0f;
    float startVol = bgmSource.volume;

    while (elapsed < bgmFadeDuration)
    {
      bgmSource.volume = Mathf.Lerp(startVol, 0f, elapsed / bgmFadeDuration);
      elapsed += Time.deltaTime;
      yield return null;
    }

    bgmSource.volume = 0f;
    bgmSource.Stop();
    bgmSource.clip = null;
  }

  public void SetBGMVolume(float volume)
  {
    bgmVolume = Mathf.Clamp01(volume);
    bgmSource.volume = bgmVolume;
  }

  private IEnumerator FadeToBGM(AudioClip newClip)
  {
    // Only fade out if there is currently a clip playing
    if (bgmSource.isPlaying && bgmSource.clip != null)
    {
      float elapsed = 0f;
      float startVol = bgmSource.volume;

      while (elapsed < bgmFadeDuration)
      {
        bgmSource.volume = Mathf.Lerp(startVol, 0f, elapsed / bgmFadeDuration);
        elapsed += Time.deltaTime;
        yield return null;
      }

      bgmSource.volume = 0f;
      bgmSource.Stop();
    }

    // Now handle fade-in if new clip is provided
    if (newClip != null)
    {
      bgmSource.clip = newClip;
      bgmSource.loop = true;
      bgmSource.volume = 0f;
      bgmSource.Play();

      float elapsed = 0f;
      while (elapsed < bgmFadeDuration)
      {
        bgmSource.volume = Mathf.Lerp(0f, bgmVolume, elapsed / bgmFadeDuration);
        elapsed += Time.deltaTime;
        yield return null;
      }

      bgmSource.volume = bgmVolume;
    }
    else
    {
      bgmSource.clip = null;
    }
  }

  // ---------------- SFX ----------------

  public void PlaySFX(string clipName)
  {
    if (sfxMap.TryGetValue(clipName, out var clip))
    {
      PlaySFX(clip);
    }
    else
    {
      Debug.LogWarning($"[SoundManager] SFX '{clipName}' not found.");
    }
  }

  public void PlaySFX(AudioClip clip)
  {
    if (clip == null) return;

    AudioSource sfx = sfxSources.Dequeue();
    sfx.Stop();
    sfx.clip = clip;
    sfx.volume = sfxVolume;
    sfx.Play();
    sfxSources.Enqueue(sfx);
  }

  public void SetSFXVolume(float volume)
  {
    if (sfxSources == null) return;

    sfxVolume = Mathf.Clamp01(volume);
    foreach (AudioSource sfx in sfxSources)
    {
      sfx.volume = sfxVolume;
    }
  }

  public void StopAllSFX()
  {
    foreach (AudioSource sfx in sfxSources)
    {
      if (sfx.isPlaying)
      {
        sfx.Stop();
      }
    }
  }
}
