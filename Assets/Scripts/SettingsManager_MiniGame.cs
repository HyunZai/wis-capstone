using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//스크립트를 해당 게임 씬에서 원하시는 오브젝트에 넣은 후에 inspector 창에 세팅,킵 플레잉, 스탑 플레잉 칸에 
//이름이 똑같은 버튼을 GUI캔버스 안 판넬에서 찾아서 드래그 앤 드롭하면 된다. 판넬 칸엔 GUI캔버스 안 판넬을 넣으면 된다.

public class SettingsManager_MiniGame : MonoBehaviour
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

    // 패널 활성화/비활성화 토글 함수
    void TogglePanel()
    {
        if (guiPanel != null)
        {
            bool isPanelActive = guiPanel.activeSelf;
            guiPanel.SetActive(!isPanelActive);
        }
    }

    // KeepPlaying 버튼을 누르면 패널을 닫고 게임을 다시 재개
    void ClosePanelAndResume()
    {
        if (guiPanel != null)
        {
            guiPanel.SetActive(false);
        }
    }

    // StopPlaying 버튼을 누르면 MapScene으로 이동
    void GoToMapScene()
    {
        SceneManager.LoadScene("MapScene");
    }
}
