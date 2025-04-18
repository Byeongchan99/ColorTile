using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVibrationManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;    // Inspector 에 할당
    public AudioMixer mixer;        // "BGMVol", "SFXVol" 노출

    // 내부 플래그
    bool _isBGMOn;
    bool _isSFXOn;
    bool _isVibrationOn;

    void Awake()
    {
        // 1) 저장값 로드
        _isBGMOn = Settings.GetBool(Settings.KEY_BGM);
        _isSFXOn = Settings.GetBool(Settings.KEY_SFX);
        _isVibrationOn = Settings.GetBool(Settings.KEY_VIBRATION);

        // 2) 초기 적용
        ApplyBGM(_isBGMOn);
        ApplySFX(_isSFXOn);
    }

    void OnEnable()
    {
        // 옵션 UI 이벤트 구독
        GameEvents.OnBGMChanged += OnBGMChanged;
        GameEvents.OnSFXChanged += OnSFXChanged;
        GameEvents.OnVibrationChanged += OnVibrationChanged;
    }

    void OnDisable()
    {
        // 구독 해제
        GameEvents.OnBGMChanged -= OnBGMChanged;
        GameEvents.OnSFXChanged -= OnSFXChanged;
        GameEvents.OnVibrationChanged -= OnVibrationChanged;
    }

    // 옵션 변경 콜백
    void OnBGMChanged(bool on)
    {
        _isBGMOn = on;
        ApplyBGM(on);
    }

    void OnSFXChanged(bool on)
    {
        _isSFXOn = on;
        ApplySFX(on);
    }

    void OnVibrationChanged(bool on)
    {
        _isVibrationOn = on;
    }

    // 외부에서 SFX 재생할 때
    public void PlaySFX(AudioClip clip, Vector3 pos)
    {
        if (_isSFXOn && clip != null)
            AudioSource.PlayClipAtPoint(clip, pos);
    }

    // 외부에서 진동 호출할 때
    public void Vibrate()
    {
        if (_isVibrationOn)
            Handheld.Vibrate();
    }

    // 실제 볼륨/재생 제어
    void ApplyBGM(bool on)
    {
        Debug.Log($"BGM: {on}");
        mixer.SetFloat("BGMVolume", on ? 0f : -80f);
        if (on && !bgmSource.isPlaying) bgmSource.Play();
        if (!on && bgmSource.isPlaying) bgmSource.Pause();
    }

    void ApplySFX(bool on)
    {
        mixer.SetFloat("SFXVolume", on ? 0f : -80f);
    }
}
