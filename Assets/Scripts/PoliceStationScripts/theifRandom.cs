using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theifRandom : MonoBehaviour
{
    public GameObject thiefPrefab; // 도둑 프리팹
    public Vector3[] windowPositions; // 창문 위치 배열
    private GameObject currentThief; // 현재 생성된 도둑을 참조
    void Start()
    {
        StartCoroutine(ShowThief());
    }

    IEnumerator ShowThief()
    {
        while (true)
        {
            // 현재 도둑이 없으면 새로운 도둑을 생성
            if (currentThief == null)
            {
                int windowIndex = Random.Range(0, windowPositions.Length); // 창문 중 하나를 랜덤으로 선택
                Vector3 spawnPosition = windowPositions[windowIndex];

                // 선택된 위치에 도둑을 생성하고 참조를 저장
                currentThief = Instantiate(thiefPrefab, spawnPosition, Quaternion.identity);

                // 도둑이 클릭될 때까지 대기
                while (currentThief != null)
                {
                    yield return null; // 매 프레임 대기
                }

                // 도둑이 사라진 후 다음 생성까지 대기
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                yield return null; // 도둑이 있을 때는 대기
            }
        }
    }
}

