using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectControler : MonoBehaviour
{
    // panel
    Vector2 basePanelPos = new Vector2(0, -7.5f);
    Vector2 playtPanelPos = new Vector2(0, -3.5f);
    
    // truck Pos
    Vector2 baseTruckPos = new Vector2(15, 0);
    Vector2 playTruckPos = new Vector2(0, 0.5f);
    Vector2 endTruckPos = new Vector2(-15, 0);

    Vector2 playImagePanelPos = new Vector2(-6f, 3f);

    public Button[] clickBtns = new Button[3];     
    public GameObject truck;
    public GameObject itemPanel;
    public GameObject answerImagePanel;

    float moveSpeed = 3.0f;
    public Button startBtn;
    private bool isGameStarted = false; // 게임 시작 상태 체크
    private float moveTime = 0.0f; // 이동 시간 체크
    public GameObject[] imageList; // 오브젝트 목록
    public GameObject[] targets = new GameObject[3]; // 결정오브젝트 이동위치
    public GameObject[] popupPanel = new GameObject[2];// 1 success 2fail
    bool isAnswer;
    private int[] randN = new int[3];

    GameObject[] selectedImageList = new GameObject[3]; //결정된 오브젝트 3개
    public GameObject answer; //정답 오브젝트
    void Start()
    {
        SetBeforeStart();
    }
    void Update()
    {
       
        SetGameObjectsPos();
        
    }
    void SetBeforeStart()
    {
        startBtn.gameObject.SetActive(true);

        for (int i = 0; i < clickBtns.Length; i++) {
            clickBtns[i].gameObject.SetActive(false);  
        }

        startBtn.onClick.AddListener(GameStart);
        SetPopup();
    }
    void GameStart()
    {
        ChangeImage();
        startBtn.gameObject.SetActive(false);
        ActiveclickBtns();
        isGameStarted = true; // 게임 시작 상태 설정
        moveTime = 0.0f; // 이동 시간 초기화
    }  
    void SetGameObjectsPos(){
         if (isGameStarted)
        {
         moveTime += Time.deltaTime; // 경과 시간 증가
            float t = moveTime * moveSpeed* Time.deltaTime;

            answerImagePanel.transform.position = Vector2.Lerp(answerImagePanel.transform.position, playImagePanelPos, t);
            truck.transform.position = Vector2.Lerp(truck.transform.position, playTruckPos, t);
            itemPanel.transform.position = Vector2.Lerp(itemPanel.transform.position, playtPanelPos, t);

            if (Vector2.Distance(answerImagePanel.transform.position, playImagePanelPos) < 0.1f &&
                Vector2.Distance(truck.transform.position, playTruckPos) < 0.1f &&
                Vector2.Distance(itemPanel.transform.position, playtPanelPos) < 0.1f)
            {
                isGameStarted = false; // 게임 시작 상태 종료
            }
        }
    }
    void ActiveclickBtns()
    {
        for (int i = 0; i < selectedImageList.Length; i++)
        {
            clickBtns[i].gameObject.SetActive(true);
            int index = i;  // 지역 변수로 저장
            clickBtns[index].onClick.AddListener(() => CheckMatchImage(index));
        }
    }
    void ChangeImage()
    {
        for (int i = 0; i < randN.Length; i++)
        {
            randN[i] = Random.Range(0, imageList.Length);
            for (int j = 0; j < i; j++) // j는 i보다 작아야 중복 검사 가능
            {
                if (randN[i] == randN[j])
                {
                    i--; // 중복 발생 시 다시 뽑기
                    break;
                }
            }
        }
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].gameObject.GetComponent<SpriteRenderer>().sprite = imageList[randN[i]].gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        SetAnswer();
    }
    void SetAnswer()
    {
        int randAnswer = Random.Range(0, selectedImageList.Length);
        answer.GetComponent<SpriteRenderer>().sprite = targets[randAnswer].GetComponent<SpriteRenderer>().sprite;
    }
    void CheckMatchImage(int i)
    {
        if (targets[i].GetComponent<SpriteRenderer>().sprite.name == answer.GetComponent<SpriteRenderer>().sprite.name)
        {
            isAnswer = true;
            PopupEvent(popupPanel[0]);
        }
        else 
        {
            isAnswer = false; 
            PopupEvent(popupPanel[1]);
        }
        
    }
    void SetPopup(){
        //disable popup
        for(int i= 0; i< popupPanel.Length; i++){
            popupPanel[i].gameObject.SetActive(false);
        }
    }
    void PopupEvent(GameObject panel){
        panel.gameObject.SetActive(true);
        StartCoroutine(ClosePopupAfterDelay(2.0f, panel));
    }
    IEnumerator ClosePopupAfterDelay(float delay, GameObject panel)
    {
        yield return new WaitForSeconds(delay);  // delay만큼 기다림
        panel.SetActive(false);  // 창 닫기
    }

}
