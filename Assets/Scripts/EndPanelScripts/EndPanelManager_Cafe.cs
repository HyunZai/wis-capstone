using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPanelManager : MonoBehaviour
{
    public GameObject endPanel; // 끝내기 패널
    public Button endPanelButton; // 끝내기 버튼

    void Start()
    {
        // 씬이 로드되면 즉시 endPanel을 활성화
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        // 버튼 클릭 시 메인 맵으로 돌아가는 기능 추가
        if (endPanelButton != null)
        {
            endPanelButton.onClick.AddListener(GoToMapScene);
        }
    }

    void GoToMapScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
    }
}