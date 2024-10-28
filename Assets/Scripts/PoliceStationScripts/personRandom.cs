using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class personRandom : MonoBehaviour
{
     public Vector3[] windowPositions; // 창문 위치 좌표 배열
    public GameObject thiefPrefab;
    public GameObject bookPrefab;
    public GameObject coffeePrefab;

    private List<int> availableWindows;
    private GameObject currentThief;
    private List<GameObject> spawnedPrefabs; // 생성된 프리팹 리스트

    void Start()
    {
        StartCoroutine(SpawnPrefabsRoutine());
    }

    IEnumerator SpawnPrefabsRoutine()
    {
        while (true)
        {
            // 3개의 프리팹 생성
            SpawnPrefabs();

            // 도둑이 클릭될 때까지 대기
            while (currentThief != null)
            {
                yield return null;
            }

            // 도둑이 사라진 후 모든 프리팹 제거
            ClearAllPrefabs();

            // 다음 생성까지 대기 시간
            yield return new WaitForSeconds(1.5f);
        }
    }

    void SpawnPrefabs()
    {
        InitializeAvailableWindows();

        spawnedPrefabs = new List<GameObject>();

        // 겹치지 않도록 창문 위치를 랜덤하게 선택
        int thiefIndex = GetRandomWindowIndex();
        int bookIndex = GetRandomWindowIndex();
        int coffeeIndex = GetRandomWindowIndex();

        // 선택된 위치에 프리팹을 생성하고 리스트에 추가
        currentThief = Instantiate(thiefPrefab, windowPositions[thiefIndex], Quaternion.identity);
        spawnedPrefabs.Add(currentThief);

        GameObject book = Instantiate(bookPrefab, windowPositions[bookIndex], Quaternion.identity);
        spawnedPrefabs.Add(book);

        GameObject coffee = Instantiate(coffeePrefab, windowPositions[coffeeIndex], Quaternion.identity);
        spawnedPrefabs.Add(coffee);
    }

    void ClearAllPrefabs()
    {
        // 모든 생성된 프리팹 제거
        foreach (GameObject prefab in spawnedPrefabs)
        {
            if (prefab != null)
            {
                Destroy(prefab);
            }
        }
        spawnedPrefabs.Clear();
    }

    void InitializeAvailableWindows()
    {
        availableWindows = new List<int>();
        for (int i = 0; i < windowPositions.Length; i++)
        {
            availableWindows.Add(i);
        }
    }

    int GetRandomWindowIndex()
    {
        if (availableWindows.Count == 0)
        {
            InitializeAvailableWindows();
        }

        int randomIndex = Random.Range(0, availableWindows.Count);
        int windowIndex = availableWindows[randomIndex];
        availableWindows.RemoveAt(randomIndex);
        return windowIndex;
    }
}




