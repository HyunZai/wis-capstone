using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressbarHandler : MonoBehaviour
{
    public Slider slider; // Slider 오브젝트를 참조
    private float loadTime = 2f; // 로딩 시간 (3초)
    private float elapsedTime = 0f;

    void Start()
    {
        slider.gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        slider.gameObject.SetActive(true);
        elapsedTime = 0f;
        slider.value = 0f;
    }
    void Update()
    {
        if (slider.gameObject.activeSelf)
        {
            // 경과 시간을 누적
            elapsedTime += Time.deltaTime;

            // Slider 값을 0에서 1까지 3초에 걸쳐 변경
            slider.value = elapsedTime / loadTime;

            // 3초가 지나면 로딩을 완료하고, 추가 작업이 필요할 경우 여기서 처리
            if (elapsedTime >= loadTime)
            {
                // 로딩 완료 후 처리
                OnLoadingComplete();
            }
        }
    }

    // 로딩 완료 후 처리
    void OnLoadingComplete()
    {
        slider.gameObject.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }
}
