using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameObjectControler : MonoBehaviour
{
    private Button[] answerExampleBtns = new Button[3];     
    private GameObject truck , AnswerSilhouetteImage;
    private GameObject[] imageList; // 오브젝트 목록
    private GameObject[] answerExamples = new GameObject[3]; //정답 선택지
    private GameObject[] scoreImages = new GameObject[3];    // 점수 이미지 전구모양
    private GameObject o, x;
    private Vector2 playTruckPos = new Vector2(0, 0.5f);
    private Vector2 endTruckPos = new Vector2(-20.0f, 0.5f);
    private bool isPlaying = false; // 게임 시작 상태 체크
    private float moveSpeed = 3.0f;
    private GameObject[] selectedImageList = new GameObject[3]; //결정된 오브젝트 3개     
    private int gameScore = 0;
    private GameObject endPanel;
    private Sprite beforeAnswer = null;


    void Start()
    {
        SetBeforeStart();
        StartGame();
    }
    void SetBeforeStart(){
        endPanel = GameObject.Find("UICanvas").transform.Find("endPanel").gameObject;

        imageList = GameObject.FindGameObjectsWithTag("Image").OrderBy(p => p.name).ToArray();
        if (imageList.Length == 0) Debug.Log("### NOT FOUND: ImageList ###");

        answerExamples = GameObject.FindGameObjectsWithTag("AnswerExampleImage").OrderBy(p => p.name).ToArray();
        if (answerExamples.Length == 0) Debug.Log("### NOT FOUND: AnswerExamples ###");

        scoreImages = GameObject.FindGameObjectsWithTag("ScoreImage").OrderBy(p => p.name).ToArray();
        if (scoreImages.Length == 0) Debug.Log("### NOT FOUND: ScoreImage ###");

        answerExampleBtns = GameObject.FindGameObjectsWithTag("Button")
            .Select(obj => obj.GetComponent<Button>())
            .Where(btn => btn != null)
            .ToArray();
        if (answerExampleBtns.Length == 0) Debug.Log("### NOT FOUND: Button ###");

        truck = GameObject.Find("FireTruck");
        if (truck == null) Debug.Log("### NOT FOUND: FireTruck ###");

        AnswerSilhouetteImage = GameObject.Find("AnswerSilhouetteImage");
        if (AnswerSilhouetteImage == null) Debug.Log("### NOT FOUND: AnswerImage ###");

        o = GameObject.Find("O");
        if (o == null) Debug.Log("### NOT FOUND: O ###");

        x = GameObject.Find("X");
        if (x == null) Debug.Log("### NOT FOUND: X ###");
        


        Time.timeScale = 1f;

        isPlaying = true; 
        AnswerSilhouetteImage.SetActive(true);
        try{endPanel.SetActive(false);}catch{}

        for(int i=0; i< answerExamples.Length; i++) answerExamples[i].SetActive(true);

        statButtonImages(false);
    }

    void StartGame(){
        ClickBtnsListener();
        StartCoroutine(TruckControlor());
        StartNextStage();
    }  
    IEnumerator TruckControlor(){
        
        while(isPlaying){ 
            if(Vector2.Distance(GetPos(truck), playTruckPos)<1f) statButtonImages(true);

            truck.transform.position = Vector2.Lerp( GetPos(truck), playTruckPos, Time.deltaTime * moveSpeed);
            yield return null;
        }

        for(int i=0; i<3; i++) answerExamples[i].SetActive(false);

        while(!isPlaying){ 
            truck.transform.position = Vector2.Lerp( GetPos(truck), endTruckPos, Time.deltaTime * moveSpeed);
            yield return null;

            if(Vector2.Distance(GetPos(truck), playTruckPos)<0.5f) endPanel.SetActive(true);
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


    //main Logic
    IEnumerator SelectRandomImageNum(){
        int[] randomImageNum = new int[3];

        while(randomImageNum.Distinct().Count() != randomImageNum.Length){
            for(int i =0; i< randomImageNum.Length; i++) randomImageNum[i] = Random.Range(0, imageList.Length);
            yield return null;
        }
        SetAnswerExampleImages(randomImageNum);
        StatBtns(true);
    }
    void SetAnswerExampleImages(int [] randomImageNum){
        for (int i = 0; i < answerExamples.Length; i++){
            answerExamples[i].gameObject.GetComponent<SpriteRenderer>().sprite = GetSprite(imageList[randomImageNum[i]].gameObject);
        }
        SetAnswerImage();
    }
    void SetAnswerImage(){
        int selectRandAnswerNum;
        do{
            selectRandAnswerNum = Random.Range(0, selectedImageList.Length);
        }while(GetSprite(answerExamples[selectRandAnswerNum]) == beforeAnswer);
        AnswerSilhouetteImage.GetComponent<SpriteRenderer>().sprite = GetSprite(answerExamples[selectRandAnswerNum]);
    }
    void CheckMatchImage(int i){
        if (GetSprite(answerExamples[i]).name == GetSprite(AnswerSilhouetteImage).name){
            StartCoroutine(MoveToAnswerPos(i));
            updateScore();
        }else if(GetSprite(answerExamples[i]).name != GetSprite(AnswerSilhouetteImage).name){
            answerExamples[i].GetComponent<SpriteRenderer>().sprite = GetSprite(x); 
        }
    }
    void updateScore(){
        scoreImages[gameScore].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        gameScore++;
    }
    IEnumerator MoveToAnswerPos(int i){
        StatBtns(false);

        Vector2 defaultPos = GetPos(answerExamples[i]);
        Vector2 defaultOPos = GetPos(o);
        Vector2 defultScale = answerExamples[i].transform.lossyScale;

        answerExamples[i].transform.localScale = AnswerSilhouetteImage.transform.lossyScale;
        
        o.transform.position = defaultPos;
        o.transform.position += new Vector3(0,0,-1f);

        while(Vector2.Distance(GetPos(answerExamples[i]), GetPos(AnswerSilhouetteImage)) > 0.5){
            answerExamples[i].transform.position = Vector2.Lerp( GetPos(answerExamples[i]) , GetPos(AnswerSilhouetteImage), moveSpeed * Time.deltaTime);
            yield return null;
        }
        answerExamples[i].transform.position = GetPos(AnswerSilhouetteImage);

        yield  return new WaitForSeconds(1.5f);

        answerExamples[i].transform.position = defaultPos;
        answerExamples[i].transform.localScale = defultScale;
        
        o.transform.position = defaultOPos;
        
        beforeAnswer = GetSprite(AnswerSilhouetteImage);
        if(gameScore<scoreImages.Length){
            StartNextStage();
        }
        else {
            statButtonImages(false);
            isPlaying = false;
        }
    }



    // Stat Control
    void statButtonImages(bool stat){
        AnswerSilhouetteImage.GetComponent<SpriteRenderer>().enabled = stat;
        for(int i=0; i< answerExamples.Length; i++){
            answerExamples[i].GetComponent<SpriteRenderer>().enabled = stat;
        }
    }
    void StatBtns(bool stat){
        for (int i = 0; i < answerExampleBtns.Length; i++) {
            answerExampleBtns[i].gameObject.SetActive(stat);  
        }
    }


    //for Utill
    Sprite GetSprite(GameObject gObj){
        return gObj.GetComponent<SpriteRenderer>().sprite;
    }
    Vector2 GetPos(GameObject gObj){
        return  gObj.transform.position;
    }
}
