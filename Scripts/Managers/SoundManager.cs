using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    [SerializeField]
    [Range(0f, 1f)]
    private float _defaultBGMVolume = 0.75f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _defaultSEVolume = 0.25f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _defaultPitch = 0.05f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _defaultFadeSpeed = 0.5f;

    private Dictionary<SoundType, AudioClip> _clipCache;

    private AudioSource _bgmSpeaker;
    private List<AudioSource> _soundEffectSpeakers;

    private float _globalVolume = 1.0f;
    private float _bgmVolume = 1.0f;
    private float _seVolume = 1.0f;

    private float _finalyBGMVolume = 0.75f;
    private float _finalySEVolume = 0.25f;

    private bool _isOn;
    private SoundType _currentBGM;

    public Coroutine coroutine;

    public float GlobalVolume
    {
        get => _globalVolume;
    }

    public float BGMVolume
    {
        get => _bgmVolume;
    }

    public float SEVolume
    {
        get => _seVolume;
    }

    public bool IsOn
    {
        get => _isOn;
        set => _isOn = value;
    }

    public void Init()
    {
        _clipCache = new();
        _bgmSpeaker = GenerateSpeaker(transform, true, 0);
        _soundEffectSpeakers = new(10);
        for (int i = 0; i < 10; i++)
        {
            var speaker = GenerateSpeaker(transform, false, _defaultSEVolume);
            speaker.loop = false;
            _soundEffectSpeakers.Add(speaker);
        }
    }

    public void CacheClip(SoundType type)
    {
        if (type == SoundType.None)
            return;

        var clip = GetAudioClip(type);
        _clipCache.Add(type, clip);
    }

    public void PlayBGM(SoundType type)
    {
        if (type == SoundType.None || _isOn == false || _currentBGM == type)
            return;

        if (coroutine != null)
            StopCoroutine(coroutine);

        _currentBGM = type;
        var clip = GetAudioClip(type);
        coroutine = StartCoroutine(PlayFadeInOutBGM(clip));
    }

    public void PlaySE(SoundType type)
    {
        if (type == SoundType.None || _isOn == false)
            return;

        foreach (var speaker in _soundEffectSpeakers)
        {
            if (speaker.isPlaying == false)
            {
                var clip = GetAudioClip(type);
                PlaySoundEffect(speaker, clip);
                break;
            }
        }
    }

    public void ChangeGlobalVolume(float volume)
    {
        _globalVolume = volume;
        ChangeBGMVolume(volume);
        ChangeSEVolume(volume);
    }

    public void ChangeBGMVolume(float volume)
    {
        _bgmVolume = volume;
        _finalyBGMVolume = _bgmVolume * _globalVolume * _defaultBGMVolume;
        _bgmSpeaker.volume = _finalyBGMVolume;
    }

    public void ChangeSEVolume(float volume)
    {
        _seVolume = volume;
        _finalySEVolume = _seVolume * _globalVolume * _defaultSEVolume;

        foreach (var item in _soundEffectSpeakers)
            item.volume = _finalySEVolume;
    }

    private IEnumerator PlayFadeInOutBGM(AudioClip clip)
    {
        while (_bgmSpeaker.volume > 0.01f)
        {
            _bgmSpeaker.volume -= _defaultFadeSpeed * Time.deltaTime;
            yield return null;
        }

        _bgmSpeaker.volume = 0;
        _bgmSpeaker.clip = clip;
        _bgmSpeaker.Play();

        while (_bgmSpeaker.volume < _finalyBGMVolume)
        {
            _bgmSpeaker.volume += _defaultFadeSpeed * Time.deltaTime;
            yield return null;
        }

        _bgmSpeaker.volume = _finalyBGMVolume;
    }

    private void PlaySoundEffect(AudioSource speaker, AudioClip clip)
    {
        speaker.clip = clip;
        speaker.volume = _finalySEVolume;
        speaker.pitch = 1f + UnityEngine.Random.Range(-_defaultPitch, _defaultPitch);
        speaker.Play();
    }

    private AudioSource GenerateSpeaker(Transform parent, bool isLoop, float volume)
    {
        var obj = new GameObject("Speaker");
        obj.transform.parent = parent;
        var audio = obj.AddComponent<AudioSource>();
        audio.loop = isLoop;
        audio.volume = volume;
        return audio;
    }

    private AudioClip GetAudioClip(SoundType type)
    {
        if (_clipCache.TryGetValue(type, out AudioClip clip) == false)
            clip = AssetManager.Instance.LoadAssetSync<AudioClip>($"Sound{(int)type}");

        return clip;
    }
}
