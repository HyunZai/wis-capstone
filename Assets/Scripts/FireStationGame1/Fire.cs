using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject waterEffectPrefab;  // 물 이펙트 프리팹
    public AudioClip extinguishSound;     // 불 끄는 소리
    public float destroyDelay = 0.5f;     // 불 제거 전 대기 시간

    public void ExtinguishFire()
    {
        // 1. 물 이펙트 생성
        if (waterEffectPrefab != null)
        {
            Instantiate(waterEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. 불 끄는 소리 재생
        if (extinguishSound != null)
        {
            AudioSource.PlayClipAtPoint(extinguishSound, transform.position);
        }

        // 3. 일정 시간 후 불 오브젝트 파괴
        Destroy(gameObject, destroyDelay);
    }
}
