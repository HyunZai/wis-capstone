using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Text requestText; // 빌려달라고 요청하는 텍스트
    public List<GameObject> itemPrefabs; // 책상 위에 놓일 물건들의 프리팹
    public GameObject clearPanel; // 게임 클리어 패널
    public Button quitButton; // 클리어 패널의 게임 종료 버튼
    public Transform itemParent; // 아이템들이 배치될 부모 오브젝트

    public Button hintButton; // 힌트 버튼
    public Text hintText; // 힌트 텍스트를 보여줄 Text UI
    public Image hintImage; // 힌트 이미지를 보여줄 Image UI
    public Image hintBackground; // 힌트의 배경 이미지
    public Sprite[] blurredSprites; // 각 아이템에 대한 흐릿한 이미지 스프라이트 배열 (순서는 itemPrefabs와 동일)
    public string[] hintDescriptions; // 각 아이템에 대한 설명 배열 (순서는 itemPrefabs와 동일)

    private List<GameObject> spawnedItems = new List<GameObject>(); // 생성된 물건들
    private List<string> availableItems = new List<string>(); // 아직 요청되지 않은 아이템 목록
    private string currentItem; // 현재 요청하는 물건
    private int itemsClicked = 0; // 클릭한 아이템 수
    private bool hintVisible = false; // 힌트 표시 여부

    void Start()
    {
        // 게임 시작 시 클리어 패널 및 힌트 비활성화
        clearPanel.SetActive(false);
        hintText.gameObject.SetActive(false);
        hintImage.gameObject.SetActive(false);
        hintBackground.gameObject.SetActive(false); // 힌트 배경 비활성화

        // `availableItems`에 초기 아이템 이름 추가
        foreach (var item in itemPrefabs)
        {
            availableItems.Add(item.name);
        }

        // 아이템 배치
        SpawnItems();

        // 처음 요청하는 물건 텍스트 설정
        UpdateRequestText();

        // 게임 종료 버튼에 이벤트 추가
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() => SceneManager.LoadScene("MapScene"));
        }

        // 힌트 버튼에 이벤트 추가
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(ToggleHint);
        }
    }

    void SpawnItems()
    {
        // 물건들이 배치될 위치들 (예: 책상 위의 특정 위치들)
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-4.5f, 1.91f, 9.89933f),
            new Vector3(-0.17f, 1.19f, 9.89933f),
            new Vector3(4.78f, 1.61f, 9.89933f),
            new Vector3(-4.04f, -1.6f, 9.89933f),
            new Vector3(-0.79f, -3.19f, 9.89933f),
            new Vector3(3.3f, -2.32f, 9.89933f)
        };

        // 두 배열의 크기 중 작은 값만큼 반복
        int itemCount = Mathf.Min(itemPrefabs.Count, positions.Length);

        // 물건 배치
        for (int i = 0; i < itemCount; i++)
        {
            GameObject newItem = Instantiate(itemPrefabs[i], positions[i], Quaternion.identity, itemParent);
            newItem.name = itemPrefabs[i].name; // 프리팹 이름 유지
            spawnedItems.Add(newItem);
        }
    }

    void UpdateRequestText()
    {
        // 랜덤으로 요청할 수 있는 물건 선택
        if (availableItems.Count > 0)
        {
            currentItem = availableItems[Random.Range(0, availableItems.Count)];
            requestText.text = $"~~아 나 {currentItem} 좀 빌려줄래?";
        }
    }

    public void ItemClicked(GameObject item)
    {
        // 클릭한 아이템의 이름이 현재 요청된 아이템과 같으면
        if (item.name == currentItem)
        {
            itemsClicked++;
            StartCoroutine(AnimateItem(item));

            // 이미 요청된 아이템은 `availableItems`에서 제거
            availableItems.Remove(currentItem);

            // 3개의 물건을 클릭하면 클리어 패널 표시
            if (itemsClicked == 3)
            {
                clearPanel.SetActive(true);
            }
            else
            {
                // 다음 요청 텍스트 업데이트
                UpdateRequestText();
            }

            // 힌트 숨기기
            hintText.gameObject.SetActive(false);
            hintImage.gameObject.SetActive(false);
            hintBackground.gameObject.SetActive(false);
        }
    }

    // 아이템 애니메이션 효과 및 사라짐
    IEnumerator AnimateItem(GameObject item)
    {
        // 애니메이션 효과: 커졌다 작아지기
        Vector3 originalScale = item.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        float duration = 0.2f;
        float elapsedTime = 0f;

        // 커지는 애니메이션
        while (elapsedTime < duration)
        {
            item.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 작아지며 사라지는 애니메이션
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            item.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 아이템 삭제
        item.SetActive(false);
    }

    // 힌트 표시 토글 메서드
    void ToggleHint()
    {
        hintVisible = !hintVisible;

        // currentItem의 인덱스를 찾음
        int index = itemPrefabs.FindIndex(item => item.name == currentItem);

        // 힌트 토글
        hintText.gameObject.SetActive(hintVisible);
        hintImage.gameObject.SetActive(hintVisible);
        hintBackground.gameObject.SetActive(hintVisible);

        if (hintVisible)
        {
            // 힌트 설명 표시
            if (index >= 0 && index < hintDescriptions.Length)
            {
                hintText.text = $"Hint: {hintDescriptions[index]}";
            }

            // 힌트 이미지 표시
            if (index >= 0 && index < blurredSprites.Length)
            {
                hintImage.sprite = blurredSprites[index];
            }
        }
    }
}
