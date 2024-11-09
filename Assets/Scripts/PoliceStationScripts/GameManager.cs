using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;  // 카드 프리팹 오브젝트
    public Sprite[] cardFaces;     // 카드 앞면 이미지 배열
    public Sprite cardBack;        // 카드 뒷면 이미지

    // UI 관련 변수
    public TextMeshProUGUI scoreText;  // 점수 표시 텍스트
    public GameObject endPanel;        // 게임 종료 시 표시할 패널

    // 게임 상태 관련 변수
    private List<GameObject> cards = new List<GameObject>();  // 생성된 카드 오브젝트 리스트
    private bool firstCardFlipped = false;  // 첫 번째 카드가 뒤집혔는지 여부
    private GameObject firstCard;           // 첫 번째로 선택된 카드
    private GameObject secondCard;          // 두 번째로 선택된 카드
    private int score = 0;                  // 현재 점수
    private int remainingPairs;             // 남은 카드 쌍의 수

    // 게임 시작 시 호출되는 메서드
    void Start()
    {
        InitializeGame();
        if (endPanel != null)
        {
            endPanel.SetActive(false);  // 게임 시작 시 종료 패널 비활성화
        }
    }

    // 게임 초기화 메서드
    void InitializeGame()
    {
        List<int> cardValues = new List<int> { 0, 0, 1, 1, 2, 2 };  // 3쌍의 카드 값 생성
        ShuffleList(cardValues);  // 카드 값 섞기

        remainingPairs = cardValues.Count / 2;  // 남은 쌍 수 초기화

        // 카드 배치를 위한 변수
        float xOffset = 3.5f;
        float yOffset = 4f;
        float startX = -3.5f;
        float startY = 2f;

        // 카드 생성 및 배치
        for (int i = 0; i < cardValues.Count; i++)
        {
            float x = startX + (i % 3) * xOffset;  // 3열로 배치
            float y = startY - (i / 3) * yOffset;

            GameObject card = Instantiate(cardPrefab, new Vector3(x, y, 0), Quaternion.identity);
            card.transform.localScale = new Vector3(0.25f, 0.25f, 1f);  // 카드 크기 조정 건들지마시옹..ㅠㅠ
            CardController cardController = card.GetComponent<CardController>();
            cardController.SetupCard(cardValues[i], cardFaces[cardValues[i]], cardBack);
            cards.Add(card);
        }

        //UpdateScoreText();  // 초기 점수 표시 업데이트
    }

    // 리스트의 요소를 무작위로 섞는 메서드
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // 카드 클릭 시 호출되는 메서드
    public void CardClicked(GameObject card)
    {
        if (firstCard != null && secondCard != null)
        {
            return;  // 이미 두 카드가 선택된 상태면 추가 선택 무시
        }

        CardController cardController = card.GetComponent<CardController>();

        if (!firstCardFlipped)
        {
            firstCard = card;
            firstCardFlipped = true;
            cardController.FlipCard();
        }
        else if (firstCard != card)
        {
            secondCard = card;
            cardController.FlipCard();
            StartCoroutine(CheckMatch());  // 매치 확인 루틴 시작
        }
    }

    // 선택된 두 카드의 매치를 확인하는 루틴
    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);  // 1초 대기

        CardController firstCardController = firstCard.GetComponent<CardController>();
        CardController secondCardController = secondCard.GetComponent<CardController>();

        if (firstCardController.cardValue == secondCardController.cardValue)
        {
            // 카드가 일치하는 경우
            score++;
            remainingPairs--;
            firstCard.SetActive(false);
            secondCard.SetActive(false);

            if (CheckAllCardsMatched())
            {
                ClearPoliceGame();  // 모든 카드가 매치되면 게임 클리어
            }
        }
        else
        {
            // 카드가 일치하지 않는 경우
            firstCardController.FlipCard();
            secondCardController.FlipCard();
        }

        // 선택 상태 초기화
        firstCardFlipped = false;
        firstCard = null;
        secondCard = null;
        //UpdateScoreText();  // 점수 표시 업데이트
    }

    //// 점수 텍스트 업데이트 메서드
    //void UpdateScoreText()
    //{
    //    if (scoreText != null)
    //    {
    //        scoreText.text = "Score: " + score + " | Remaining Pairs: " + remainingPairs;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("ScoreText is not assigned!");
    //    }
    //}

    // 모든 카드가 매치되었는지 확인하는 메서드

    bool CheckAllCardsMatched()
    {
        return remainingPairs == 0;
    }

    // 게임 클리어 시 호출되는 메서드
    private void ClearPoliceGame()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);  // 종료 패널 활성화
            Time.timeScale = 0;  // 게임 일시 정지
            Debug.Log("Game cleared");
        }
        else
        {
            Debug.LogError("endPanel GameObject is not assigned.");
        }
    }
}