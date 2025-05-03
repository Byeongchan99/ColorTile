using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVibrationManager : MonoBehaviour
{
    [Header("Audio Sources & Mixer")]
    public AudioSource bgmSource; // BGM�� AudioSource
    public AudioSource sfxSource; // SFX�� AudioSource
    public AudioMixer mixer; // "BGMVolume", "SFXVolume" Exposed Parameters

    [Header("Indexed SFX Clips")]
    [Tooltip("0���� ������� ȿ������ ����ϼ���.")]
    public List<AudioClip> sfxClips; // �ε����� �����ϴ� ȿ���� ����Ʈ 0: UI ��ư Ŭ��, 1: Ÿ�� ����, 2: �г�Ƽ

    // ���� ����
    private Dictionary<int, AudioClip> _sfxDictonary;
    private bool _isBGMOn, _isSFXOn, _isVibrationOn;

    void Awake()
    {
        // �ɼ� ���� �ҷ�����
        _isBGMOn = Settings.GetBool(Settings.KEY_BGM);
        _isSFXOn = Settings.GetBool(Settings.KEY_SFX);
        _isVibrationOn = Settings.GetBool(Settings.KEY_VIBRATION);

        // ���� �ʱ� ����
        ApplyBGM(_isBGMOn);
        ApplySFX(_isSFXOn);

        // ����Ʈ �� ��ųʸ��� ����
        _sfxDictonary = new Dictionary<int, AudioClip>(sfxClips.Count);
        for (int i = 0; i < sfxClips.Count; i++)
            if (sfxClips[i] != null)
                _sfxDictonary[i] = sfxClips[i];
    }

    void OnEnable()
    {
        GameEvents.OnBGMChanged += OnBGMChanged;
        GameEvents.OnSFXChanged += OnSFXChanged;
        GameEvents.OnVibrationChanged += OnVibrationChanged;
        GameEvents.OnPlaySFX += PlaySFX;
    }

    void OnDisable()
    {
        GameEvents.OnBGMChanged -= OnBGMChanged;
        GameEvents.OnSFXChanged -= OnSFXChanged;
        GameEvents.OnVibrationChanged -= OnVibrationChanged;
        GameEvents.OnPlaySFX -= PlaySFX;
    }

    void OnBGMChanged(bool on) => ApplyBGM(_isBGMOn = on);
    void OnSFXChanged(bool on) => ApplySFX(_isSFXOn = on);
    void OnVibrationChanged(bool on) => _isVibrationOn = on;

    /// <summary>
    /// �ε����� �����ϴ� ȿ������ PlayOneShot���� ����մϴ�.
    /// </summary>
    public void PlaySFX(int index)
    {
        if (!_isSFXOn) return;

        if (_sfxDictonary != null && _sfxDictonary.TryGetValue(index, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioVibrationManager] SFX index not found: {index}");
        }
    }

    /// <summary>
    /// ���� ȣ��
    /// </summary>
    public void Vibrate()
    {
        if (_isVibrationOn)
            Handheld.Vibrate();
    }

    void ApplyBGM(bool on)
    {
        mixer.SetFloat("BGMVolume", on ? 0f : -80f);
        if (on && !bgmSource.isPlaying) bgmSource.Play();
        if (!on && bgmSource.isPlaying) bgmSource.Pause();
    }

    void ApplySFX(bool on)
    {
        mixer.SetFloat("SFXVolume", on ? 0f : -80f);
    }
}
