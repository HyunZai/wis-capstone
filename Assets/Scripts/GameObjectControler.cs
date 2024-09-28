using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameObjectControler : MonoBehaviour
{
    Vector2 playtPenalPos = new Vector2(0, -3.5f);

    Vector2 playTruckPos = new Vector2(0, 0.5f);
    Vector2 endTruckPos = new Vector2(-20.0f, 0.5f);

    //Vector2 playImagePanelPos = new Vector2(-6.5f, 3.5f);
    Vector2 giniusPos = new Vector2(-1.0f, 4f);


    public Button[] clickBtns = new Button[3];     
    public GameObject truck;
    public GameObject itemPanel;
    public GameObject answerImage;
    public GameObject imagin;
    

    float moveSpeed = 3.0f;
    public Button startBtn;
    public Button endBtn;
    private bool isGameStarted = false; // 게임 시작 상태 체크
    private float moveTime = 0.0f; // 이동 시간 체크
    public GameObject[] imageList; // 오브젝트 목록
    public GameObject[] targets = new GameObject[3]; // 결정오브젝트 이동위치
    public GameObject[] popupPanel = new GameObject[2];// 1 success 2fail
    public GameObject[] ginius = new GameObject[4];

    public int isAnswerCount = 0;
    bool isAnswer;
    bool isGameEnd;

    GameObject[] selectedImageList = new GameObject[3]; //결정된 오브젝트 3개
    public GameObject answer; //정답 오브젝트
    void Start()
    {
        SetBeforeStart();
    }
    void Update()
    {
       
        SetGameObjectsPos();
        if(isAnswer == true){
            ++isAnswerCount;
            isAnswer = false;
            ResetImage();
        }
        
      
        
    }
    void SetBeforeStart()
    {
        startBtn.gameObject.SetActive(true);
        imagin.SetActive(false);
        answerImage.SetActive(false);
        for (int i = 0; i < clickBtns.Length; i++) {
            clickBtns[i].gameObject.SetActive(false);  
        }

        startBtn.onClick.AddListener(GameStart);
        endBtn.onClick.AddListener(endBtnClick);
        SetPopup();
    }
    void GameStart()
    {
        ResetImage();
        startBtn.gameObject.SetActive(false);
        ActiveClickBtns();
        isGameStarted = true; // 게임 시작 상태 설정
        moveTime = 0.0f; // 이동 시간 초기화
    }  
    void SetGameObjectsPos(){
        moveTime += Time.deltaTime; // 경과 시간 증가
        float t = moveSpeed* Time.deltaTime;
        if(isGameStarted)
        {
            for(int i = 0; i< ginius.Length-1; i++){
            ginius[i].transform.position = Vector2.Lerp(ginius[i].transform.position, giniusPos + new Vector2(i,0), t);
            }
            //answerImage.transform.position = Vector2.Lerp(answerImage.transform.position, playImagePanelPos, t);
            truck.transform.position = Vector2.Lerp(truck.transform.position, playTruckPos, t);
            itemPanel.transform.position = Vector2.Lerp(itemPanel.transform.position, playtPenalPos, t);

            if (//Vector2.Distance(answerImage.transform.position, playImagePanelPos) < 0.1f &&
                Vector2.Distance(truck.transform.position, playTruckPos) < 0.1f &&
                Vector2.Distance(itemPanel.transform.position, playtPenalPos) < 0.1f)
            {
                imagin.SetActive(true);
                answerImage.SetActive(true);
                isGameStarted = false; // 게임 시작 상태 종료
            }
        }
        else if(isGameEnd){
            truck.transform.position = Vector2.Lerp(truck.transform.position, endTruckPos, Time.deltaTime * moveSpeed);
            if(Vector2.Distance(truck.transform.position, endTruckPos) < 0.1f){
            endBtn.gameObject.SetActive(true);
            
           // EndBtn.onClick.AddListener(()=> ); add next Scene
            }
        }
    }
    void ActiveClickBtns()
    {
        for (int i = 0; i < selectedImageList.Length; i++)
        {
            clickBtns[i].gameObject.SetActive(true);
            int index = i;  // 지역 변수로 저장
            clickBtns[index].onClick.AddListener(() => CheckMatchImage(index));
        }
    }
    void ResetImage()
    {
        int[] randN = new int[3];
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
            ginius[isAnswerCount].GetComponent<SpriteRenderer>().sprite = ginius[3].GetComponent<SpriteRenderer>().sprite;
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
        StartCoroutine(AfterDelay(2.0f, panel));
    }
    IEnumerator AfterDelay(float delay, GameObject panel)
    {
        yield return new WaitForSeconds(delay);  // delay만큼 기다림
        panel.SetActive(false);  // 창 닫기
        if(isAnswerCount >= 3){
            imagin.SetActive(false);
            for(int i = 0; i < clickBtns.Length;i++){
                clickBtns[i].gameObject.SetActive(false);
                targets[i].gameObject.SetActive(false);
            }
            answerImage.gameObject.SetActive(false);
            itemPanel.gameObject.SetActive(false);

            Invoke("EndGame",2.0f);
        }   
       
    }
    void EndGame(){
        isGameEnd =true;
    }
    
    void endBtnClick() {
        SceneManager.LoadScene("MapScene");
    }

}
