using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;               // 현재 점수
    public int scoreToWin = 10;         // 게임 클리어에 필요한 점수
    public Text scoreText;              // 점수를 표시할 UI 텍스트
    public GameObject winPanel;         // 게임 클리어 시 표시할 패널

    private void Start()
    {
        UpdateScoreText();
        winPanel.SetActive(false);  // 게임 시작 시 승리 패널 비활성화
    }

    // 점수 추가 메서드
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();

        // 게임 클리어 조건 체크
        if (score >= scoreToWin)
        {
            WinGame();
        }
    }

    // 점수를 UI에 업데이트
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    // 게임 클리어 처리
    private void WinGame()
    {
        Debug.Log("Game Won!");
        winPanel.SetActive(true);  // 승리 패널 활성화
        Time.timeScale = 0;        // 게임 일시정지
    }
}
