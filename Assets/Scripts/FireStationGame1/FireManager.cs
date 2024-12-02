using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public GameObject firePrefab;      // 불 프리팹
    public Transform[] windowPositions; // 창문 위치들
    public float minSpawnInterval = 3f; // 최소 불 발생 간격
    public float maxSpawnInterval = 8f; // 최대 불 발생 간격

    private void Start()
    {
        StartCoroutine(SpawnFireRoutine());
    }

    private IEnumerator SpawnFireRoutine()
    {
        while (true)
        {
            // 불의 발생 간격을 랜덤하게 설정
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            // 창문 중 하나를 랜덤하게 선택하여 불 생성
            int randomIndex = Random.Range(0, windowPositions.Length);
            Transform spawnPoint = windowPositions[randomIndex];

            // 이미 불이 발생한 창문인지 확인
            if (spawnPoint.childCount == 0)
            {
                Instantiate(firePrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            }
        }
    }
}
