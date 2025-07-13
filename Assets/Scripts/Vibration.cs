using System.Collections;
using UnityEngine;

public static class Vibration
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
    public static void Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate");
#else
        Handheld.Vibrate();
#endif
    }

    public static void Vibrate(long milliseconds, int amplitude)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    if (AndroidVibrator == null) return;

    int sdk = new AndroidJavaClass("android.os.Build$VERSION")
              .GetStatic<int>("SDK_INT");

    // API 26 이상에서만 amplitude 조절 가능
    if (sdk >= 26)
    {
        // 진동 세기를 지원하는지 장치에 직접 물어봄
        bool hasAmp = AndroidVibrator.Call<bool>("hasAmplitudeControl");

        if (hasAmp)
        {
            var vibEffectCls = new AndroidJavaClass("android.os.VibrationEffect");
            var effect = vibEffectCls.CallStatic<AndroidJavaObject>(
                            "createOneShot", milliseconds, Mathf.Clamp(amplitude, 1, 255));
            AndroidVibrator.Call("vibrate", effect);
            return;
        }
    }

    // 지원하지 않는 기기는 기본 세기로 짧게만
    AndroidVibrator.Call("vibrate", milliseconds);
#else
        Handheld.Vibrate();         // 에디터·iOS용 폴백
#endif
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", pattern, repeat);
#else
        Handheld.Vibrate();
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("cancel");
#endif
    }

}