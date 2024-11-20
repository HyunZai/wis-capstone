using System.Collections;
using UnityEngine;
using TMPro;

//스마트폰 객체에서 입력된 전화번호가 부모 전화번호와 일치하지 않을 때 흔들리는 효과를 주기 위한 클래스
public class TextShake : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float shakeDuration = 0.5f; // 흔들림 지속 시간
    public float shakeMagnitude = 5f; // 흔들림 강도

    private Vector3 originalPosition;

    private void Start()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TextMeshProUGUI>();

        originalPosition = textMeshPro.rectTransform.localPosition;
    }

    public void ShakeText()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude),
                0
            );

            textMeshPro.rectTransform.localPosition = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        textMeshPro.rectTransform.localPosition = originalPosition; // 원래 위치로 복구
    }
}