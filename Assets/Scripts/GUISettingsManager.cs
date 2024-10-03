using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GUISettingsManager : MonoBehaviour
{
    public Button settingsButton; // Settings 버튼 참조
    public Button keepPlayingButton; // KeepPlaying 버튼 참조
    public Button stopPlayingButton; // 그만 놀기 버튼 참조
    public GameObject guiPanel;   // GUI 패널 참조

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
            keepPlayingButton.onClick.AddListener(ClosePanel);
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
            guiPanel.SetActive(!guiPanel.activeSelf);
        }
    }

    // KeepPlaying 버튼을 누르면 패널을 닫음
    void ClosePanel()
    {
        if (guiPanel != null)
        {
            guiPanel.SetActive(false);
        }
    }

    // StopPlaying 버튼을 누르면 게임 종료
    void QuitGame()
    {
        // 빌드된 게임에서는 작동, 에디터에서는 메시지만 출력
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
