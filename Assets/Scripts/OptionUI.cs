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
        BGMButton.onClick.AddListener(ToggleBGM);
        SFXButton.onClick.AddListener(ToggleSFX);
        vibrationButton.onClick.AddListener(ToggleVibration);

        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false); // 옵션 UI 비활성화
        }
    }

    private void Start()
    {
        // 저장된 설정 불러오기
        _isBGMOn = Settings.GetBool(Settings.KEY_BGM);
        _isSFXOn = Settings.GetBool(Settings.KEY_SFX);
        _isVibrationOn = Settings.GetBool(Settings.KEY_VIBRATION);

        RefreshButtons();

        // 앱 시작 시에도 한 번 발행해서, 구독자들이 초기 상태를 알게끔
        GameEvents.OnBGMChanged?.Invoke(_isBGMOn);
        GameEvents.OnSFXChanged?.Invoke(_isSFXOn);
        GameEvents.OnVibrationChanged?.Invoke(_isVibrationOn);
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

    // BGM 토글
    void ToggleBGM()
    {
        _isBGMOn = !_isBGMOn;
        Settings.SetBool(Settings.KEY_BGM, _isBGMOn);
        RefreshButtons();
        GameEvents.OnBGMChanged?.Invoke(_isBGMOn);
        //Debug.Log($"BGM: {(_isBGMOn ? "On" : "Off")}");
    }

    // SFX 토글
    void ToggleSFX()
    {
        _isSFXOn = !_isSFXOn;
        Settings.SetBool(Settings.KEY_SFX, _isSFXOn);
        RefreshButtons();
        GameEvents.OnSFXChanged?.Invoke(_isSFXOn);
        //Debug.Log($"SFX: {(_isSFXOn ? "On" : "Off")}");
    }

    // 진동 토글
    void ToggleVibration()
    {
        _isVibrationOn = !_isVibrationOn;
        Settings.SetBool(Settings.KEY_VIBRATION, _isVibrationOn);
        RefreshButtons();
        GameEvents.OnVibrationChanged?.Invoke(_isVibrationOn);
        //Debug.Log($"Vibration: {(_isVibrationOn ? "On" : "Off")}");
    }

    /// 버튼 색상 변경
    void RefreshButtons()
    {
        BGMButton.image.color = _isBGMOn ? Color.white : Color.gray;
        SFXButton.image.color = _isSFXOn ? Color.white : Color.gray;
        vibrationButton.image.color = _isVibrationOn ? Color.white : Color.gray;
    }
}
