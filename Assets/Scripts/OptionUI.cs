using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button BGMButton;
    public Button SFXButton;
    public Button vibrationButton; // 진동 버튼
    public Button closeButton;

    [Header("Flags")]
    public bool _isBGMOn = true; // BGM 여부
    public bool _isSFXOn = true; // SFX 여부
    public bool _isVibrationOn = true; // 진동 여부

    private void Awake()
    {
        GameEvents.OnOpenOption += OpenOption; // 옵션 열기 이벤트 등록
        
        // 버튼 클릭 이벤트 등록
        closeButton.onClick.AddListener(OnClickCloseOption);
        BGMButton.onClick.AddListener(BGMOnOff);
        SFXButton.onClick.AddListener(SFXOnOff);
        vibrationButton.onClick.AddListener(VibrationOnOff);

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false); // 옵션 UI 비활성화
        }
    }

    public void OpenOption()
    {
        Debug.Log("Option UI Opened");
        this.gameObject.SetActive(true); // 옵션 UI 활성화
    }

    public void OnClickCloseOption()
    {
        this.gameObject.SetActive(false); // 옵션 UI 비활성화
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
