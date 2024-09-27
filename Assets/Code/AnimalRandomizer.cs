using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalRandomizer : MonoBehaviour
{
    public TextMeshProUGUI animalText; // UI 텍스트 오브젝트
    public string[] animalNames; // 동물 이름 배열

    void Start()
    {
        // 동물 이름 배열 초기화
        animalNames = new string[] { "고 양 이", "강 아 지", "코 끼 리", "호 랑 이", "사 자", "원 숭 이", "얼 룩 말", "펭 귄", "황 소", "늑 대", "판 다", 
                                    "기 린", "코 알 라", "사 슴", "거 북 이", "너 구 리", "상 어", "돌 고 래", "염 소", "개 구 리", "표 범", "치 타", 
                                    "다 람 쥐", "수 달", "당 나 귀", "고 릴 라", "하 마", "오 리", "고 슴 도 치", "코 뿔 소", "낙 타", "하 이 에 나", "거 미", 
                                    "도 마 뱀", "여 우", "햄 스 터", "돼 지", "알 파 카", "토 끼", "박 쥐", "까 치", "닭", "타 조", "공 작", "거 위", "부 엉 이", 
                                    "올 빼 미", "앵 무 새", "악 어", "사 마 귀", "잠 자 리", "나 비", "메 뚜 기", "달 팽 이", "문 어", "오 징 어", "해 파 리", "불 가 사 리", "지 렁 이" };

        // 랜덤으로 이름 선택
        SetRandomAnimalName(false);
    }

    public void SetRandomAnimalName(bool isEnd)
    {
        RectTransform rectTransform = animalText.GetComponent<RectTransform>();
        Vector2 newPosition = rectTransform.anchoredPosition;
        
        if (isEnd)
        {
            animalText.text = "참 잘했어요!";
            newPosition.x = -102;
        }
        else 
        {
            // 배열에서 랜덤으로 하나의 이름 선택
            int randomIndex = Random.Range(0, animalNames.Length);

            if (animalText.text == animalNames[randomIndex]) //이전과 동일한 동물이름이 나오지 않도록 하기 위한 
            {
                randomIndex = Random.Range(0, animalNames.Length);

                if (animalText.text == animalNames[randomIndex])
                {
                    randomIndex = Random.Range(0, animalNames.Length);
                }
            }

            animalText.text = animalNames[randomIndex];
            
            float x = 0f;
            switch(animalNames[randomIndex].Length)
            {
                case 1: x = 413; break;
                case 3: x = 289; break;
                case 5: x = 141; break;
                case 7: x = -28; break;
            }
            newPosition.x = x;
        }

        rectTransform.anchoredPosition = newPosition;
    }
}
