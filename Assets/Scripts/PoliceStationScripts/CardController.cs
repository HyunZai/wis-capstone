using UnityEngine;

public class CardController : MonoBehaviour
{
    // 카드의 값, 앞면 이미지, 뒷면 이미지를 저장하는 프로퍼티
    public int cardValue { get; private set; }
    public Sprite cardFace { get; private set; }
    public Sprite cardBack { get; private set; }

    // 카드가 뒤집혀 있는지 여부
    private bool isFlipped = false;

    // 카드의 이미지를 렌더링하는 SpriteRenderer 컴포넌트
    private SpriteRenderer spriteRenderer;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();  // SpriteRenderer 컴포넌트 가져오기
    }

    // SetupCard 카드를 초기화
    // 처음에는 뒷면을 표시
    public void SetupCard(int value, Sprite face, Sprite back)
    {
        cardValue = value;  // 카드의 값을 설정
        cardFace = face;  // 카드의 앞면 이미지를 설정
        cardBack = back;  // 카드의 뒷면 이미지를 설정
        spriteRenderer.sprite = cardBack;  // 처음에는 뒷면 이미지를 표시
    }

    // FlipCard는 카드를 뒤집는 메서드
    // 현재 상태에 따라 앞면 또는 뒷면을 표시
    public void FlipCard()
    {
        isFlipped = !isFlipped;  // 카드를 뒤집음 (true <-> false 전환)
        spriteRenderer.sprite = isFlipped ? cardFace : cardBack;  // 상태에 따라 앞면 또는 뒷면을 표시
    }

    // IsFlipped는 카드가 현재 뒤집혀 있는지 여부를 반환
    public bool IsFlipped()
    {
        return isFlipped;
    }

    // OnMouseDown은 Unity에서 마우스 클릭 이벤트를 감지하는 메서드
    // 오류 검사용
    void OnMouseDown()
    {
        Debug.Log("Card clicked: " + cardValue);  // 클릭된 카드의 값을 콘솔에 출력

        // 게임 매니저를 찾아서 CardClicked 메서드를 호출하여 게임 로직을 처리
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.CardClicked(gameObject);
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }
}