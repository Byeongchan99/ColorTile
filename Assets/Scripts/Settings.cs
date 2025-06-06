using UnityEngine;

[Tooltip("게임 설정을 저장하는 클래스")]
static class Settings
{
    public const string KEY_BGM = "BGM";
    public const string KEY_SFX = "SFX";
    public const string KEY_VIBRATION = "Vibration";
    public const string KEY_COLORBLIND_MODE = "ColorblindMode";

    // 기본값: 켜진 상태
    public static bool GetBool(string key) => PlayerPrefs.GetInt(key, 1) == 1;
    public static void SetBool(string key, bool on)
    {
        PlayerPrefs.SetInt(key, on ? 1 : 0);
        PlayerPrefs.Save();
    }
}
