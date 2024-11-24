using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager_MiniGame2 : MonoBehaviour
{
    public Button settingsButton; // Settings 버튼 참조
    public Button keepPlayingButton; // KeepPlaying 버튼 참조
    public Button stopPlayingButton; // 그만 놀기 버튼 참조
    public Button bgmToggleButton; // 배경음 버튼 참조
    public Button sfxToggleButton; // 효과음 버튼 참조

    public GameObject guiPanel;   // GUI 패널 참조

    public AudioSource bgmAudioSource; // 배경음 AudioSource

    public Image bgmButtonImage; // 배경음 버튼의 이미지
    public Sprite bgmOnSprite; // 배경음 켜짐 상태 이미지
    public Sprite bgmOffSprite; // 배경음 꺼짐 상태 이미지

    public Image sfxButtonImage; // 효과음 버튼의 이미지
    public Sprite sfxOnSprite; // 효과음 켜짐 상태 이미지
    public Sprite sfxOffSprite; // 효과음 꺼짐 상태 이미지

    private bool isPaused = false; // 게임 일시 정지 상태를 추적
    private bool isBgmMuted = false; // 배경음 음소거 상태
    private bool isSfxMuted = false; // 효과음 음소거 상태

    void Start()
    {
        // GUI 패널을 처음에 비활성화
        if (guiPanel != null)
        {
            guiPanel.SetActive(false);
        }

        // Settings 버튼에 OnClick 이벤트 추가
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(TogglePanel);
        }
        else
        {
            Debug.LogError("Settings Button is not assigned in the inspector.");
        }

        // KeepPlaying 버튼에 OnClick 이벤트 추가
        if (keepPlayingButton != null)
        {
            keepPlayingButton.onClick.AddListener(ClosePanelAndResume);
        }
        else
        {
            Debug.LogError("KeepPlaying Button is not assigned in the inspector.");
        }

        // StopPlaying 버튼에 OnClick 이벤트 추가
        if (stopPlayingButton != null)
        {
            stopPlayingButton.onClick.AddListener(GoToMapScene);
        }
        else
        {
            Debug.LogError("StopPlaying Button is not assigned in the inspector.");
        }

        // BGM 버튼에 OnClick 이벤트 추가
        if (bgmToggleButton != null)
        {
            bgmToggleButton.onClick.AddListener(ToggleBGM);
        }
        else
        {
            Debug.LogError("BGM Toggle Button is not assigned in the inspector.");
        }

        // SFX 버튼에 OnClick 이벤트 추가
        if (sfxToggleButton != null)
        {
            sfxToggleButton.onClick.AddListener(ToggleSFX);
        }
        else
        {
            Debug.LogError("SFX Toggle Button is not assigned in the inspector.");
        }

        UpdateBgmIcon(); // BGM 아이콘 초기화
        UpdateSfxIcon(); // SFX 아이콘 초기화
    }

    // 패널 활성화/비활성화 및 타임 슬립 제어 함수
    void TogglePanel()
    {
        if (guiPanel != null)
        {
            bool isPanelActive = guiPanel.activeSelf;
            guiPanel.SetActive(!isPanelActive);

            if (isPanelActive)
            {
                ResumeGame(); // 패널이 닫힐 때 게임 재개
            }
            else
            {
                PauseGame(); // 패널이 열릴 때 게임 일시 정지
            }
        }
    }

    // 게임 일시 정지
    void PauseGame()
    {
        Time.timeScale = 0; // 게임 정지
        isPaused = true;
    }

    // 게임 재개
    void ResumeGame()
    {
        Time.timeScale = 1; // 게임 재개
        isPaused = false;
    }

    // KeepPlaying 버튼을 누르면 패널을 닫고 게임 재개
    void ClosePanelAndResume()
    {
        if (guiPanel != null)
        {
            guiPanel.SetActive(false);
        }
        ResumeGame();
    }

    // StopPlaying 버튼을 누르면 MapScene으로 이동
    void GoToMapScene()
    {
        SceneManager.LoadScene("MapScene");
    }

    // 배경음 토글
    void ToggleBGM()
    {
        isBgmMuted = !isBgmMuted;
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = isBgmMuted; // 배경음 음소거 설정
        }
        UpdateBgmIcon();
    }

    // 배경음 아이콘 업데이트
    void UpdateBgmIcon()
    {
        if (bgmButtonImage != null)
        {
            bgmButtonImage.sprite = isBgmMuted ? bgmOffSprite : bgmOnSprite;
        }
        else
        {
            Debug.LogError("BGM Button Image is not assigned in the Inspector.");
        }
    }

    // 효과음 토글
    void ToggleSFX()
    {
        isSfxMuted = !isSfxMuted;
        UpdateSfxIcon();
    }

    // 효과음 아이콘 업데이트
    void UpdateSfxIcon()
    {
        if (sfxButtonImage != null)
        {
            sfxButtonImage.sprite = isSfxMuted ? sfxOffSprite : sfxOnSprite;
        }
        else
        {
            Debug.LogError("SFX Button Image is not assigned in the Inspector.");
        }
    }
}
