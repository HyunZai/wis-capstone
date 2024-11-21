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
    public GameObject guiPanel;   // GUI 패널 참조

    private bool isPaused = false; // 게임 일시 정지 상태를 추적

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
}
