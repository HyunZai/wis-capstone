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
    public GameObject guiPanel;   // GUI 패널 참조

    private bool isPaused = false; // 게임 일시 정지 여부를 추적

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
            stopPlayingButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogError("StopPlaying Button is not assigned in the inspector.");
        }
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
}
