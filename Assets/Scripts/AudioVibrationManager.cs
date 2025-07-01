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

    [Header("Vibration Settings")]
    [Tooltip("진동 지속 시간(밀리초)")]
    [SerializeField] private int vibrationDurationMs = 50;
    [Tooltip("진동 세기(1~255)")]
    [SerializeField, Range(1, 255)] private int vibrationAmplitude = 50;

    // 진동 API 설정
#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject _vibrator;
    private AndroidJavaClass _vibrationEffectClass;
    private bool _supportsVibrationEffect;
#endif

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

        // 진동 세팅
        SettingVibrate();
    }

    void OnEnable()
    {
        GameEvents.OnBGMChanged += OnBGMChanged;
        GameEvents.OnSFXChanged += OnSFXChanged;
        GameEvents.OnVibrationChanged += OnVibrationChanged;

        GameEvents.OnPlaySFX += PlaySFX;
        GameEvents.OnPlayVibration += PlayVibrate;
    }

    void OnDisable()
    {
        GameEvents.OnBGMChanged -= OnBGMChanged;
        GameEvents.OnSFXChanged -= OnSFXChanged;
        GameEvents.OnVibrationChanged -= OnVibrationChanged;

        GameEvents.OnPlaySFX -= PlaySFX;
        GameEvents.OnPlayVibration -= PlayVibrate;
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

    public void SettingVibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_vibrator == null)
        {
            try
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activity    = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var context     = activity.Call<AndroidJavaObject>("getApplicationContext");
                _vibrator       = context.Call<AndroidJavaObject>("getSystemService", "vibrator");

                var versionClass = new AndroidJavaClass("android.os.Build$VERSION");
                int sdkInt       = versionClass.GetStatic<int>("SDK_INT");
                if (sdkInt >= 26)
                {
                    _supportsVibrationEffect = true;
                    _vibrationEffectClass    = new AndroidJavaClass("android.os.VibrationEffect");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Vibrator init failed: {e}");
            }
        }
#endif
    }

    /// <summary>
    /// 진동 호출
    /// </summary>
    public void PlayVibrate()
    {
        if (!_isVibrationOn)
            return;

#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        if (_vibrator != null)
        {
            if (_supportsVibrationEffect && _vibrationEffectClass != null)
            {
                vibrationAmplitude = Mathf.Clamp(vibrationAmplitude, 1, 255);
                var effect = _vibrationEffectClass.CallStatic<AndroidJavaObject>(
                    "createOneShot", vibrationDurationMs, vibrationAmplitude);
                _vibrator.Call("vibrate", effect);
            }
            else
            {
                _vibrator.Call("vibrate", vibrationDurationMs);
            }
        }
        else
        {
            Debug.LogWarning("[AudioVibrationManager] Vibrator is null.");
            Handheld.Vibrate(); // fallback 기본 진동 호출
        }
    }
    catch (System.Exception e)
    {
        Debug.LogWarning($"[AudioVibrationManager] PlayVibrate error: {e.Message}");
        Handheld.Vibrate(); // fallback 기본 진동 호출
    }
#elif UNITY_IOS && !UNITY_EDITOR
    Handheld.Vibrate();
#else
        Handheld.Vibrate();
#endif
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
