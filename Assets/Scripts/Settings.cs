using UnityEngine;

static class Settings
{
    public const string KEY_BGM = "BGM";
    public const string KEY_SFX = "SFX";
    public const string KEY_VIBRATION = "Vibration";

    // �⺻��: ���� ����(1)
    public static bool GetBool(string key) => PlayerPrefs.GetInt(key, 1) == 1;
    public static void SetBool(string key, bool on)
    {
        PlayerPrefs.SetInt(key, on ? 1 : 0);
        PlayerPrefs.Save();
    }
}
