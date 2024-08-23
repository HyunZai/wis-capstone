using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float destroyDelay = 0.5f;  // 물과 불이 사라지기 전 대기 시간
    private bool hasCollided = false;  // 충돌 여부를 확인하는 플래그

    private Rigidbody2D rb;  // Rigidbody2D 컴포넌트를 저장할 변수

    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D 컴포넌트 가져오기
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 충돌이 처리된 경우 더 이상 처리하지 않음
        if (hasCollided)
            return;

        // 물 오브젝트가 불 오브젝트와 충돌했는지 확인
        if (other.CompareTag("Fire"))
        {
            // 충돌이 발생했음을 표시
            hasCollided = true;

            // 물의 이동을 멈춤
            if (rb != null)
            {
                rb.velocity = Vector2.zero;  // 속도를 0으로 설정하여 물의 이동을 멈춤
                rb.isKinematic = true;  // 물 오브젝트가 물리적으로 더 이상 움직이지 않도록 설정
            }

            // 불 오브젝트에 접근하여 불을 끄는 함수 호출
            Fire fire = other.GetComponent<Fire>();
            if (fire != null)
            {
                fire.ExtinguishFireWithDelay(destroyDelay);  // 일정 시간 후 불을 끄도록 설정
            }

            // 물 오브젝트의 Collider를 비활성화하여 추가 충돌을 방지
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // 일정 시간 후 물 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}
