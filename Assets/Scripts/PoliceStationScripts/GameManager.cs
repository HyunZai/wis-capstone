using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;  // ī�� ������ ������Ʈ
    public Sprite[] cardFaces;     // ī�� �ո� �̹��� �迭
    public Sprite cardBack;        // ī�� �޸� �̹���

    // UI ���� ����
    public TextMeshProUGUI scoreText;  // ���� ǥ�� �ؽ�Ʈ
    public GameObject endPanel;        // ���� ���� �� ǥ���� �г�

    // ���� ���� ���� ����
    private List<GameObject> cards = new List<GameObject>();  // ������ ī�� ������Ʈ ����Ʈ
    private bool firstCardFlipped = false;  // ù ��° ī�尡 ���������� ����
    private GameObject firstCard;           // ù ��°�� ���õ� ī��
    private GameObject secondCard;          // �� ��°�� ���õ� ī��
    private int score = 0;                  // ���� ����
    private int remainingPairs;             // ���� ī�� ���� ��

    // ���� ���� �� ȣ��Ǵ� �޼���
    void Start()
    {
        InitializeGame();
        if (endPanel != null)
        {
            endPanel.SetActive(false);  // ���� ���� �� ���� �г� ��Ȱ��ȭ
        }
    }

    // ���� �ʱ�ȭ �޼���
    void InitializeGame()
    {
        List<int> cardValues = new List<int> { 0, 0, 1, 1, 2, 2 };  // 3���� ī�� �� ����
        ShuffleList(cardValues);  // ī�� �� ����

        remainingPairs = cardValues.Count / 2;  // ���� �� �� �ʱ�ȭ

        // ī�� ��ġ�� ���� ����
        float xOffset = 3.5f;
        float yOffset = 4f;
        float startX = -3.5f;
        float startY = 2f;

        // ī�� ���� �� ��ġ
        for (int i = 0; i < cardValues.Count; i++)
        {
            float x = startX + (i % 3) * xOffset;  // 3���� ��ġ
            float y = startY - (i / 3) * yOffset;

            GameObject card = Instantiate(cardPrefab, new Vector3(x, y, 0), Quaternion.identity);
            card.transform.localScale = new Vector3(0.25f, 0.25f, 1f);  // ī�� ũ�� ���� �ǵ������ÿ�..�Ф�
            CardController cardController = card.GetComponent<CardController>();
            cardController.SetupCard(cardValues[i], cardFaces[cardValues[i]], cardBack);
            cards.Add(card);
        }

        //UpdateScoreText();  // �ʱ� ���� ǥ�� ������Ʈ
    }

    // ����Ʈ�� ��Ҹ� �������� ���� �޼���
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

    // ī�� Ŭ�� �� ȣ��Ǵ� �޼���
    public void CardClicked(GameObject card)
    {
        if (firstCard != null && secondCard != null)
        {
            return;  // �̹� �� ī�尡 ���õ� ���¸� �߰� ���� ����
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
            StartCoroutine(CheckMatch());  // ��ġ Ȯ�� ��ƾ ����
        }
    }

    // ���õ� �� ī���� ��ġ�� Ȯ���ϴ� ��ƾ
    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);  // 1�� ���

        CardController firstCardController = firstCard.GetComponent<CardController>();
        CardController secondCardController = secondCard.GetComponent<CardController>();

        if (firstCardController.cardValue == secondCardController.cardValue)
        {
            // ī�尡 ��ġ�ϴ� ���
            score++;
            remainingPairs--;
            firstCard.SetActive(false);
            secondCard.SetActive(false);

            if (CheckAllCardsMatched())
            {
                ClearPoliceGame();  // ��� ī�尡 ��ġ�Ǹ� ���� Ŭ����
            }
        }
        else
        {
            // ī�尡 ��ġ���� �ʴ� ���
            firstCardController.FlipCard();
            secondCardController.FlipCard();
        }

        // ���� ���� �ʱ�ȭ
        firstCardFlipped = false;
        firstCard = null;
        secondCard = null;
        //UpdateScoreText();  // ���� ǥ�� ������Ʈ
    }

    //// ���� �ؽ�Ʈ ������Ʈ �޼���
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

    // ��� ī�尡 ��ġ�Ǿ����� Ȯ���ϴ� �޼���

    bool CheckAllCardsMatched()
    {
        return remainingPairs == 0;
    }

    // ���� Ŭ���� �� ȣ��Ǵ� �޼���
    private void ClearPoliceGame()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);  // ���� �г� Ȱ��ȭ
            Time.timeScale = 0;  // ���� �Ͻ� ����
            Debug.Log("Game cleared");
        }
        else
        {
            Debug.LogError("endPanel GameObject is not assigned.");
        }
    }
}