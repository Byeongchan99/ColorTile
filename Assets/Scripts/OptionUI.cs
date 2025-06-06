using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button BGMButton;
    public Button SFXButton;
    public Button vibrationButton; // ���� ��ư
    public Button closeButton;
    public Button colorblindButton; // ���� ��� ��ư

    [Header("Flags")]
    public bool _isBGMOn = true; // BGM ����
    public bool _isSFXOn = true; // SFX ����
    public bool _isVibrationOn = true; // ���� ����
    public bool _isColorblindModeOn = false; // ���� ��� ����

    private void Awake()
    {
        GameEvents.OnOpenOption += OpenOption; // �ɼ� ���� �̺�Ʈ ���
        
        // ��ư Ŭ�� �̺�Ʈ ���
        closeButton.onClick.AddListener(OnClickCloseOption);
        BGMButton.onClick.AddListener(ToggleBGM);
        SFXButton.onClick.AddListener(ToggleSFX);
        vibrationButton.onClick.AddListener(ToggleVibration);
        colorblindButton.onClick.AddListener(ToggleColorblindMode);

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false); // �ɼ� UI ��Ȱ��ȭ
        }
    }

    private void Start()
    {
        // ����� ���� �ҷ�����
        _isBGMOn = Settings.GetBool(Settings.KEY_BGM);
        _isSFXOn = Settings.GetBool(Settings.KEY_SFX);
        _isVibrationOn = Settings.GetBool(Settings.KEY_VIBRATION);
        _isColorblindModeOn = Settings.GetBool(Settings.KEY_COLORBLIND_MODE);

        RefreshButtons();

        // �� ���� �ÿ��� �� �� �����ؼ�, �����ڵ��� �ʱ� ���¸� �˰Բ�
        GameEvents.OnBGMChanged?.Invoke(_isBGMOn);
        GameEvents.OnSFXChanged?.Invoke(_isSFXOn);
        GameEvents.OnVibrationChanged?.Invoke(_isVibrationOn);
        GameEvents.OnColorblindModeChanged?.Invoke(_isColorblindModeOn);
    }

    public void OpenOption()
    {
        Debug.Log("Option UI Opened");
        this.gameObject.SetActive(true); // �ɼ� UI Ȱ��ȭ
    }

    public void OnClickCloseOption()
    {
        this.gameObject.SetActive(false); // �ɼ� UI ��Ȱ��ȭ
    }

    // BGM ���
    void ToggleBGM()
    {
        _isBGMOn = !_isBGMOn;
        Settings.SetBool(Settings.KEY_BGM, _isBGMOn);
        RefreshButtons();
        GameEvents.OnBGMChanged?.Invoke(_isBGMOn);
        //Debug.Log($"BGM: {(_isBGMOn ? "On" : "Off")}");
    }

    // SFX ���
    void ToggleSFX()
    {
        _isSFXOn = !_isSFXOn;
        Settings.SetBool(Settings.KEY_SFX, _isSFXOn);
        RefreshButtons();
        GameEvents.OnSFXChanged?.Invoke(_isSFXOn);
        //Debug.Log($"SFX: {(_isSFXOn ? "On" : "Off")}");
    }

    // ���� ���
    void ToggleVibration()
    {
        _isVibrationOn = !_isVibrationOn;
        Settings.SetBool(Settings.KEY_VIBRATION, _isVibrationOn);
        RefreshButtons();
        GameEvents.OnVibrationChanged?.Invoke(_isVibrationOn);
        //Debug.Log($"Vibration: {(_isVibrationOn ? "On" : "Off")}");
    }

    // ���� ��� ���
    void ToggleColorblindMode()
    {
        _isColorblindModeOn = !_isColorblindModeOn;
        Settings.SetBool(Settings.KEY_COLORBLIND_MODE, _isColorblindModeOn);
        RefreshButtons();
        GameEvents.OnColorblindModeChanged?.Invoke(_isColorblindModeOn);
        //Debug.Log($"Colorblind Mode: {(_isColorblindModeOn ? "On" : "Off")}");
    }

    /// ��ư ���� ����
    void RefreshButtons()
    {
        BGMButton.image.color = _isBGMOn ? Color.white : Color.gray;
        SFXButton.image.color = _isSFXOn ? Color.white : Color.gray;
        vibrationButton.image.color = _isVibrationOn ? Color.white : Color.gray;
        colorblindButton.image.color = _isColorblindModeOn ? Color.white : Color.gray;
    }
}
