using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager_MapScene : MonoBehaviour
{
    public Button settingsButton; // Settings 버튼 참조
    public Button keepPlayingButton; // 계속하기 버튼 참조
    public Button stopPlayingButton; // 그만하기 버튼 참조
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

    private bool isPaused = false; // 게임 일시 정지 여부를 추적
    private bool isBgmMuted = false; // 배경음 음소거 상태
    private bool isSfxMuted = false; // 효과음 음소거 상태

    void Start()
    {
        InitializeBgmState(); // BGM 상태 초기화

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
            stopPlayingButton.onClick.AddListener(QuitGame);
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

    // BGM 상태 초기화
    // void InitializeBgmState()
    // {
    //     // PlayerPrefs에서 저장된 BGM 상태를 읽어옵니다 (기본값은 0)
    //     int savedBgmState = PlayerPrefs.GetInt("BGM_STATE", 0);
    //     isBgmMuted = (savedBgmState == 1);

    //     // BGM 음소거 설정 및 아이콘 업데이트
    //     if (bgmAudioSource != null)
    //     {
    //         bgmAudioSource.mute = isBgmMuted;
    //     }
    //     UpdateBgmIcon();
    // }
    void InitializeBgmState()
    {
        // AudioManager에서 BGM 상태를 가져와 초기화
        isBgmMuted = AudioManager.Instance.IsBgmMuted;

        // BGM 음소거 설정
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = isBgmMuted;
        }

        // BGM 아이콘 업데이트
        UpdateBgmIcon();
    }

    // 패널 활성화/비활성화 토글 함수
    void TogglePanel()
    {
        if (guiPanel != null)
        {
            bool isPanelActive = guiPanel.activeSelf;
            guiPanel.SetActive(!isPanelActive);

            if (isPanelActive)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // 게임을 일시 정지
    void PauseGame()
    {
        Time.timeScale = 0; // 게임 일시 정지
        isPaused = true;
    }

    // 게임을 재개
    void ResumeGame()
    {
        Time.timeScale = 1; // 게임 재개
        isPaused = false;
    }

    // KeepPlaying 버튼을 누르면 패널을 닫고 게임을 다시 재개
    void ClosePanelAndResume()
    {
        if (guiPanel != null)
        {
            guiPanel.SetActive(false);
        }
        ResumeGame();
    }

    // StopPlaying 버튼을 누르면 게임 종료
    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중지
        #else
            Application.Quit(); // 빌드된 게임 종료
        #endif
    }

    // 배경음 토글
    // void ToggleBGM()
    // {
    //     isBgmMuted = !isBgmMuted;
    //     if (bgmAudioSource != null)
    //     {
    //         bgmAudioSource.mute = isBgmMuted; // 배경음 음소거 설정
    //     }
    //     UpdateBgmIcon();
    // }

    void ToggleBGM()
    {
        isBgmMuted = !isBgmMuted;

        // BGM 음소거 설정
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = isBgmMuted;
        }

        // 싱글턴(AudioManager)에 상태 저장
        AudioManager.Instance.IsBgmMuted = isBgmMuted;

        // BGM 아이콘 업데이트
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
