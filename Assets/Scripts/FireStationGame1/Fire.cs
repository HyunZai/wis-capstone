using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject waterEffectPrefab;  // 물 이펙트 프리팹
    public AudioClip extinguishSound;     // 불 끄는 소리
    public float destroyDelay = 0.5f;     // 불 제거 전 대기 시간


    
    // 즉시 불을 끄는 메서드 추가
    public void ExtinguishFire()
    {
        // 물 이펙트 생성
        if (waterEffectPrefab != null)
        {
            GameObject waterEffect = Instantiate(waterEffectPrefab, transform.position, Quaternion.identity);
            // 물 이펙트를 불의 자식으로 설정
            waterEffect.transform.SetParent(transform);
        }

        // 불 끄는 소리 재생
        if (extinguishSound != null)
        {
            AudioSource.PlayClipAtPoint(extinguishSound, transform.position);
        }

        // 즉시 불 오브젝트 파괴
        Destroy(gameObject);
    }

    // 일정 시간 후 불을 끄는 메서드
    public void ExtinguishFireWithDelay(float delay)
    {
        // 물 이펙트 생성
        if (waterEffectPrefab != null)
        {
            GameObject waterEffect = Instantiate(waterEffectPrefab, transform.position, Quaternion.identity);
            // 물 이펙트를 불의 자식으로 설정
            waterEffect.transform.SetParent(transform);
        }

        // 불 끄는 소리 재생
        if (extinguishSound != null)
        {
            AudioSource.PlayClipAtPoint(extinguishSound, transform.position);
        }

        // 일정 시간 후 불 오브젝트 파괴
        Destroy(gameObject, delay);
    }
}
