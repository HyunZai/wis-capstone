
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CC : MonoBehaviour
{   
    public GameObject player;    //메인 캐릭터 지정
    float moveSpeed = 4.0f;
    GameObject[] characterList, crossPoints ,destinationPoints ,buildingPoints;    //캐릭터 설정 리스트 ++++ 캐릭터 목록 추가점요
    Button[] setDistinationBtns;// 빌딩목록 넣기
    public int destinationNum;  //목표빌딩 번호
    public int beforeDestination =4; // 집
    Vector2 [] cp, dp, bp;
    Vector2 pp;
    bool isMove = false;
    public Animator animator;
    Vector2 beforePP;
    public Vector2 isMoved;
    int homeRoute1, homeRoute2;
    public GameObject popup;
    private bool goHomeMode = false;
    Button[] btns;
    TextMeshProUGUI askText;
    public GameObject buildingBtns;
    float popupDelay = 2.0f;
    int setHome= 4;
    
    
    void Start(){
        
        SetBeforeStart();
    }
    void Update(){
        pp = player.transform.position;  
        MoveAnimation();
    }
    void SetBeforeStart(){ //이름 순서대로 할당해주고  캐릭터 포지션을 집으로 이동 및 클릭버튼 활성화>> 클릭버튼은  추후 실물판에서 값 받아와서 바꿔주는걸로
        popup.SetActive(true);
        destinationNum = beforeDestination;
        characterList = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name).ToArray();
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DistinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("Building").OrderBy(building =>  building.name).ToArray();
        setDistinationBtns = buildingBtns.GetComponentsInChildren<Button>().OrderBy(btn => btn.name).ToArray();
        btns = popup.GetComponentsInChildren<Button>().OrderBy(btnyns => btnyns.name).ToArray();
        askText = popup.GetComponentInChildren<TextMeshProUGUI>();

        
        btns[0].onClick.AddListener(()=> AskGOHomeMode());
        btns[1].onClick.AddListener(() => popup.SetActive(false));
        btns[2].onClick.AddListener(()=> ActivateGoHomeMode());

        player.transform.position = destinationPoints[setHome].transform.position;

        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[destinationPoints.Length];
        bp = new Vector2[buildingPoints.Length];
        beforePP = pp;

        for(int i =0; i<destinationPoints.Length; i++){        //건물 클릭 시 반응
            if(i<crossPoints.Length)cp[i] = crossPoints[i].transform.position;
            dp[i] = destinationPoints[i].transform.position;
            bp[i] = buildingPoints[i].transform.position;
            int index = i;
            setDistinationBtns[index].onClick.AddListener(()=>SetDestination(index));
            
        }
        Debug.Log(
          setDistinationBtns.Length  
        );
        popup.SetActive(false);
    }
    (int numB, int numP) SetCrossPoint(){
        int numB = int.MaxValue;
        float minB= float.MaxValue;
        for(int i =0; i < crossPoints.Length; i++){
            if(Mathf.Abs(dp[destinationNum].x - cp[i].x) < 0.1f 
            || Mathf.Abs(dp[destinationNum].y-cp[i].y) < 0.1f)
            {
                if(minB > Vector2.Distance(pp ,cp[i])){
                    minB = Vector2.Distance(pp ,cp[i]);
                    numB = i;
                }
            }
        }
        int numP = numB;
        for(int i =0; i< crossPoints.Length; i++){
          
            if ((Mathf.Abs(cp[i].x - cp[numB].x) < 0.1f 
                || Mathf.Abs(cp[i].y-cp[numB].y) < 0.1f) 
                && (Mathf.Abs(pp.x - cp[i].x) < 0.1f 
                || Mathf.Abs(pp.y - cp[i].y) < 0.1f)) {
                if (numB ==i) {
                    numP =i;
                    break;
                }
                numP = i;
            }
        }
        return (numB,  numP);
    }
    IEnumerator GoDestination(){ 
        isMove = true;
        beforePP = pp;
        
        // StartCoroutine(XYDebug());
        while(Vector2.Distance(pp,dp[beforeDestination])>0.1f){
            player.transform.position = Vector2.MoveTowards(pp,dp[beforeDestination],moveSpeed* Time.deltaTime);
            yield return null;
        }
        var (numB, numP) =SetCrossPoint();
        isMove = true;
       
        if(!(Mathf.Abs(pp.x - dp[destinationNum].x) < 0.1f || Mathf.Abs(pp.y-dp[destinationNum].y)<0.1f)){
            beforePP = pp;
            while(Vector2.Distance(pp,cp[numP])>0.1f){
                player.transform.position = Vector2.MoveTowards(pp,cp[numP],moveSpeed* Time.deltaTime);
                yield return null;
            }
            beforePP = pp;
            while(Vector2.Distance(pp,cp[numB])>0.1f){
                player.transform.position = Vector2.MoveTowards(pp,cp[numB],moveSpeed* Time.deltaTime);
                if(Vector2.Distance(pp,dp[destinationNum])<0.1f)yield break;
                yield return null;
            }
        }
        beforePP = pp;
        while(Vector2.Distance(pp,dp[destinationNum])>0.1f){
            player.transform.position = Vector2.MoveTowards(pp,dp[destinationNum],moveSpeed* Time.deltaTime);
            yield return null;
        }
        beforePP = pp;
        while(Vector2.Distance(pp,bp[destinationNum])>0.1f){
            player.transform.position = Vector2.MoveTowards(pp,bp[destinationNum],moveSpeed* Time.deltaTime);
            yield return null;
        }
        ShowPopup();
        beforeDestination = destinationNum;
        isMove = false;
    }
    void SetDestination(int gotoHere){
        if(destinationNum != gotoHere && isMove == false)
        {
            if(!goHomeMode){
                destinationNum = gotoHere;
                StartCoroutine(GoDestination());
            }
            else if(goHomeMode){
                if (homeRoute1 == gotoHere && destinationNum != homeRoute2 && destinationNum != setHome){
                    destinationNum = gotoHere;
                    StartCoroutine(GoDestination());
                }else if(destinationNum == homeRoute1 && gotoHere == homeRoute2){
                    destinationNum = gotoHere;
                    StartCoroutine(GoDestination()); 
                }else if(destinationNum == homeRoute2 && gotoHere == setHome){
                    destinationNum = gotoHere;
                    StartCoroutine(GoDestination()); 
                }else if(destinationNum != homeRoute1 && gotoHere != homeRoute1){
                    SetPopup($"{BuildingNameChanger(homeRoute1)}으로 이동해주세요!");
                }else if(destinationNum == homeRoute1 && gotoHere != homeRoute2){
                    SetPopup($"{BuildingNameChanger(homeRoute2)}으로 이동해주세요!");
                }else if(destinationNum == homeRoute2 && gotoHere != setHome){
                    SetPopup($"{BuildingNameChanger(homeRoute2)}으로 이동해주세요!");
                }

                
            }
            
        }
        else if(destinationNum == gotoHere)
        {
            SetPopup("다른 건물을 선택해주세요 :()");
        }
        else if(isMove == true)
        {
            SetPopup("캐릭터가 이동 중 입니다!");
        }
    }
    void MoveAnimation(){
        if(isMove){
            isMoved = pp - beforePP;
            // animator.SetFloat("MoveX", isMoved.x);
            // animator.SetFloat("MoveY", isMoved.y);
            // 애니메이션 추가만 하면 끝난다잇
        }
        
    }
    void AskGOHomeMode(){
        btns[0].gameObject.SetActive(false);
        btns[2].gameObject.SetActive(true);
        askText.text = "집으로 돌아갈까요?";
        popup.SetActive(true);
    }
    void ActivateGoHomeMode(){
        goHomeMode = true;
        do {
            homeRoute1 = Random.Range(0, buildingPoints.Length);
        } while (homeRoute1 == destinationNum && setHome == homeRoute1); 
        
        do {
            homeRoute2 = Random.Range(0, buildingPoints.Length);
        } while (homeRoute2 == destinationNum || homeRoute2 == homeRoute1 && setHome == homeRoute2);
        ShowPopup();
       
    }

    string BuildingNameChanger(int a){
        string playerLocation = "";
        switch (a)
            {
                case 0: playerLocation = "학교"; break;
                case 1: playerLocation = "카페"; break;
                case 2: playerLocation = "소방서"; break;
                case 3: playerLocation = "도서관"; break;
                case 4: playerLocation = "집"; break;
                case 5: playerLocation = "마켓"; break;
                case 6: playerLocation = "경찰서"; break;
                case 7: playerLocation = "은행"; break;
                case 8: playerLocation = "병원"; break;
                default: playerLocation = "알 수 없음"; break;
            }
        return playerLocation;
    }
    void ShowPopup(){
        popup.SetActive(true);
        string playerLocation = BuildingNameChanger(destinationNum);
        if(!goHomeMode){
            if(Vector2.Distance(pp, bp[destinationNum]) < 0.1){
                btns[2].gameObject.SetActive(false);
                btns[0].gameObject.SetActive(true);
                askText.text = $"{playerLocation}에 들어갈까요?";
                //go in buildinig
                //out buildin
            }
        }
        else if(goHomeMode){
             if(destinationNum !=homeRoute2 && destinationNum != homeRoute1){
                SetPopup($"먼저 집으로 가기 위해서 {BuildingNameChanger(homeRoute1)}로 가볼까요?");
            }else if(destinationNum ==homeRoute1){
                SetPopup($"잘 도착했어요!\n이제 {BuildingNameChanger(homeRoute2)}로 이동한 뒤에 집으로 가볼까요?");
            }else if(destinationNum == homeRoute2){
                SetPopup("집으로 가볼까요?");
            }else if(destinationNum == setHome){
                SetPopup("집에 도착했어요! 저장완료!");
        }
        }
        
    }
    void SetPopup(string a){
        askText.text = a;
        for(int i = 0; i< btns.Length; i++){

            btns[i].gameObject.SetActive(false);   
        }
        popup.SetActive(true);
        StartCoroutine(AutoClosePopup(popupDelay));
        
    }

    IEnumerator AutoClosePopup(float closeTime){
        yield return new WaitForSeconds(closeTime);
        popup.SetActive(false);
    }
    void OnOffGameObjects(GameObject[] a ,bool b){
        for(int i =0; i < a.Length; i++){
            setDistinationBtns[i].gameObject.SetActive(b);
        }
    }

}

