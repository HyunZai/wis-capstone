using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShooter : MonoBehaviour
{
    public GameObject waterPrefab;  // 물 오브젝트 프리팹
    public float waterSpeed = 25f;  // 물 오브젝트의 속도
    public float waterStartY = 0f;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootWater();
        }
    }

    void ShootWater()
    {
        // waterPrefab이 null인지 확인
        if (waterPrefab == null)
        {
            Debug.LogError("Water prefab is not assigned!");  // 프리팹이 할당되지 않았을 경우 오류 메시지
            return;
        }

        // 마우스 클릭 위치를 월드 좌표로 변환
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        // 물 오브젝트의 발사 y좌표를 고정된 값으로 설정
        Vector3 spawnPosition = new Vector3(mousePosition.x, waterStartY, mousePosition.z);

        // 물 오브젝트 생성
        GameObject water = Instantiate(waterPrefab, spawnPosition, Quaternion.identity);

        // Rigidbody2D 컴포넌트 가져오기 및 물 오브젝트 이동
        Rigidbody2D rb = water.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.up * waterSpeed;
        }
        else
        {
            Debug.LogError("Rigidbody2D component not found on waterPrefab!");  // Rigidbody2D가 없을 경우 오류 메시지
        }
        Destroy(water, 1.5f);
    }
}

public class Water2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 물 오브젝트가 불 오브젝트와 충돌했는지 확인
        if (other.CompareTag("Fire"))
        {
            // 불 오브젝트에 접근하여 불을 끄는 함수 호출
            Fire fire = other.GetComponent<Fire>();
            if (fire != null)
            {
                // fire.ExtinguishFire() 대신 일정 시간 후 불을 끄도록 수정
                fire.ExtinguishFireWithDelay(0f);  // 바로 불을 끄고 싶다면 0초 지연 사용
            }

            // 물 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}

public class Fire2 : MonoBehaviour
{
    public GameObject waterEffectPrefab;  // 물 이펙트 프리팹
    public AudioClip extinguishSound;     // 불 끄는 소리
    public float destroyDelay = 0.5f;     // 불 제거 전 대기 시간

    private void OnMouseDown()
    {
        ExtinguishFire();
    }

    public void ExtinguishFire()
    {
        

        // 불 끄는 소리 재생
        if (extinguishSound != null)
        {
            AudioSource.PlayClipAtPoint(extinguishSound, transform.position);
        }

        // 일정 시간 후 불 오브젝트 파괴
        Destroy(gameObject, destroyDelay);
    }
}
