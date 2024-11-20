using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameButtonHandler : MonoBehaviour
{
    public Button endGameButton;
    public GameObject endPanel; // 끝내기 패널
    public Button endPanelButton; // 끝내기 패널 안의 버튼

    public void Start()
    {
        // 시작 시 endPanel 비활성화
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        // endPanelButton에 클릭 이벤트 추가
        if (endPanelButton != null)
        {
            endPanelButton.onClick.AddListener(LoadMapScene);
        }
    }

    // endPanel을 활성화하는 메서드 (필요 시 다른 스크립트에서 호출 가능)
    public void ShowEndPanel()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }
    }

    void LoadMapScene()
    {
        SceneManager.LoadScene("MapScene");
    }
}
