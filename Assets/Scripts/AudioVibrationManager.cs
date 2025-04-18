using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVibrationManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bgmSource;    // Inspector �� �Ҵ�
    public AudioMixer mixer;        // "BGMVol", "SFXVol" ����

    // ���� �÷���
    bool _isBGMOn;
    bool _isSFXOn;
    bool _isVibrationOn;

    void Awake()
    {
        // 1) ���尪 �ε�
        _isBGMOn = Settings.GetBool(Settings.KEY_BGM);
        _isSFXOn = Settings.GetBool(Settings.KEY_SFX);
        _isVibrationOn = Settings.GetBool(Settings.KEY_VIBRATION);

        // 2) �ʱ� ����
        ApplyBGM(_isBGMOn);
        ApplySFX(_isSFXOn);
    }

    void OnEnable()
    {
        // �ɼ� UI �̺�Ʈ ����
        GameEvents.OnBGMChanged += OnBGMChanged;
        GameEvents.OnSFXChanged += OnSFXChanged;
        GameEvents.OnVibrationChanged += OnVibrationChanged;
    }

    void OnDisable()
    {
        // ���� ����
        GameEvents.OnBGMChanged -= OnBGMChanged;
        GameEvents.OnSFXChanged -= OnSFXChanged;
        GameEvents.OnVibrationChanged -= OnVibrationChanged;
    }

    // �ɼ� ���� �ݹ�
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

    // �ܺο��� SFX ����� ��
    public void PlaySFX(AudioClip clip, Vector3 pos)
    {
        if (_isSFXOn && clip != null)
            AudioSource.PlayClipAtPoint(clip, pos);
    }

    // �ܺο��� ���� ȣ���� ��
    public void Vibrate()
    {
        if (_isVibrationOn)
            Handheld.Vibrate();
    }

    // ���� ����/��� ����
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
