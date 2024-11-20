using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameObjectControler : MonoBehaviour
{
    Vector2 playTruckPos = new Vector2(0, 0.5f);
    Vector2 endTruckPos = new Vector2(-20.0f, 0.5f);
    public Button[] answerExampleBtns = new Button[3];     
    public GameObject truck , showNeedObjImage;
    float moveSpeed = 3.0f;
    private bool isPlaying = false; // 게임 시작 상태 체크
    public GameObject[] imageList; // 오브젝트 목록
    public GameObject[] answerExamples = new GameObject[3]; //정답 선택지
    public GameObject[] scoreImages = new GameObject[3];    // 점수 이미지 전구모양
    GameObject[] selectedImageList = new GameObject[3]; //결정된 오브젝트 3개     
    public int gameScore = 0;


    void Start()
    {
        SetBeforeStart();
        StartGame();
    }
    void Update()
    {
        
    }
    void SetBeforeStart(){
        try{GameObject.Find("endPanel").SetActive(false);}catch{}
        
        isPlaying = true; 
        Time.timeScale = 1f;
        showNeedObjImage.SetActive(true);
        for(int i=0; i< answerExamples.Length; i++)answerExamples[i].SetActive(true);
        statNeedObjImages(false);
        StatBtns(false);
    }
    void StartGame(){
        ClickBtnsListener();
        StartCoroutine(TruckControlor());
        StartCoroutine(SelectRandomImageNum());
    }  
    void StatBtns(bool stat){
        for (int i = 0; i < answerExampleBtns.Length; i++) {
            answerExampleBtns[i].gameObject.SetActive(stat);  
        }
    }
    IEnumerator TruckControlor(){
        
        while(isPlaying){ 
            if(Vector2.Distance(truck.transform.position, playTruckPos)<1f) {
                StatBtns(true);
                statNeedObjImages(true);
            }
            truck.transform.position = Vector2.Lerp(truck.transform.position, playTruckPos, Time.deltaTime * moveSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        while(!isPlaying){ 
            truck.transform.position = Vector2.Lerp(truck.transform.position, endTruckPos, Time.deltaTime * moveSpeed);
            yield return null;
            if(Vector2.Distance(truck.transform.position, playTruckPos)<0.1f) GameObject.Find("UICanvas")?.transform.Find("endPanel")?.gameObject.SetActive(true);
        }

    }
    void ClickBtnsListener(){
        for (int i = 0; i < selectedImageList.Length; i++){
            int index = i;  
            answerExampleBtns[index].onClick.AddListener(() => CheckMatchImage(index));
        }
    }
    void StartNextStage(){
        StartCoroutine(SelectRandomImageNum());
    }
   IEnumerator SelectRandomImageNum(){
    int[] randomImageNum = new int[3];

    while(randomImageNum.Distinct().Count() != randomImageNum.Length){
        for(int i =0; i< randomImageNum.Length; i++) randomImageNum[i] = Random.Range(0, imageList.Length);
        yield return null;
    }
    SetAnswerExampleImages(randomImageNum);
    }
    void statNeedObjImages(bool stat){
        GameObject.Find("ShowNeedOBjImage").GetComponent<SpriteRenderer>().enabled = stat;
        GameObject.Find("Answer").GetComponent<SpriteRenderer>().enabled = stat;
    }

    void SetAnswerExampleImages(int [] randomImageNum){
        for (int i = 0; i < answerExamples.Length; i++){
            answerExamples[i].gameObject.GetComponent<SpriteRenderer>().sprite = imageList[randomImageNum[i]].gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        SetAnswerImage();
    }
    void SetAnswerImage(){
        int randAnswer = Random.Range(0, selectedImageList.Length);
        showNeedObjImage.GetComponent<SpriteRenderer>().sprite = answerExamples[randAnswer].GetComponent<SpriteRenderer>().sprite;
    }
    void CheckMatchImage(int i){
        if (answerExamples[i].GetComponent<SpriteRenderer>().sprite.name == showNeedObjImage.GetComponent<SpriteRenderer>().sprite.name){
            updateScoreImage();
            gameScore++;
            if(gameScore<scoreImages.Length)StartNextStage();
            else {
                statNeedObjImages(false);
                isPlaying = false;
                
                
            }
        }
        
    }
    void updateScoreImage(){
        scoreImages[gameScore].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

}
