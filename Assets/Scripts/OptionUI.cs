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

    [Header("Flags")]
    public bool _isBGMOn = true; // BGM ����
    public bool _isSFXOn = true; // SFX ����
    public bool _isVibrationOn = true; // ���� ����

    private void Awake()
    {
        GameEvents.OnOpenOption += OpenOption; // �ɼ� ���� �̺�Ʈ ���
        
        // ��ư Ŭ�� �̺�Ʈ ���
        closeButton.onClick.AddListener(OnClickCloseOption);
        BGMButton.onClick.AddListener(BGMOnOff);
        SFXButton.onClick.AddListener(SFXOnOff);
        vibrationButton.onClick.AddListener(VibrationOnOff);

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false); // �ɼ� UI ��Ȱ��ȭ
        }
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

    void BGMOnOff()
    {
        _isBGMOn = !_isBGMOn;
        PlayerPrefs.SetInt("BGM", _isBGMOn ? 1 : 0);
        Debug.Log("BGM toggled: " + (_isBGMOn ? "On" : "Off"));
    }

    void SFXOnOff()
    {
        _isSFXOn = !_isSFXOn; 
        PlayerPrefs.SetInt("SFX", _isSFXOn ? 1 : 0);
        Debug.Log("SFX toggled: " + (_isSFXOn ? "On" : "Off"));
    }

    public void VibrationOnOff()
    {
        _isVibrationOn = !_isVibrationOn;
        PlayerPrefs.SetInt("Vibration", _isVibrationOn ? 1 : 0);
        Debug.Log("Vibration toggled: " + (_isVibrationOn ? "On" : "Off"));
    }
}
