using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class policeGameStart : MonoBehaviour
{
    public GameObject startPanel; // 게임 시작 패널
    public policeScore scoreManager; // ScoreManager 참조

    private void Start()
    {
        startPanel.SetActive(true); // 게임 시작 시 패널 활성화
        scoreManager.enabled = false; // 게임 시작 전에는 ScoreManager 비활성화

        StartCoroutine(StartGameAfterDelay(3.0f)); // 3초 후에 게임 시작
    }

    // 일정 시간 후 게임을 시작하는 코루틴
    IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        startPanel.SetActive(false); // 시작 패널 비활성화
        scoreManager.enabled = true; // 게임 점수 관리 활성화
    }
}
