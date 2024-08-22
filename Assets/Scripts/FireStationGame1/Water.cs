using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float destroyTime = 2f;  // 물 오브젝트가 파괴되기까지의 시간

    private void Start()
    {
        // 일정 시간이 지나면 물 오브젝트 파괴
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 물 오브젝트가 불 오브젝트와 충돌했는지 확인
        if (other.CompareTag("Fire"))
        {
            // 불 오브젝트에 접근하여 불을 끄는 함수 호출
            Fire fire = other.GetComponent<Fire>();
            if (fire != null)
            {
                fire.ExtinguishFire();
            }

            // 물 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}
