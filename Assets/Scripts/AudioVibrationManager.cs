using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVibrationManager : MonoBehaviour
{
    [Header("Audio Sources & Mixer")]
    public AudioSource bgmSource; // BGM용 AudioSource
    public AudioSource sfxSource; // SFX용 AudioSource
    public AudioMixer mixer; // "BGMVolume", "SFXVolume" Exposed Parameters

    [Header("Indexed SFX Clips")]
    [Tooltip("0부터 순서대로 효과음을 등록하세요.")]
    public List<AudioClip> sfxClips; // 인덱스에 대응하는 효과음 리스트 0: UI 버튼 클릭, 1: 타일 제거, 2: 패널티

    // 내부 상태
    private Dictionary<int, AudioClip> _sfxDictonary;
    private bool _isBGMOn, _isSFXOn, _isVibrationOn;

    void Awake()
    {
        // 옵션 상태 불러오기
        _isBGMOn = Settings.GetBool(Settings.KEY_BGM);
        _isSFXOn = Settings.GetBool(Settings.KEY_SFX);
        _isVibrationOn = Settings.GetBool(Settings.KEY_VIBRATION);

        // 볼륨 초기 적용
        ApplyBGM(_isBGMOn);
        ApplySFX(_isSFXOn);

        // 리스트 → 딕셔너리로 매핑
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
    /// 인덱스에 대응하는 효과음을 PlayOneShot으로 재생합니다.
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
    /// 진동 호출
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
