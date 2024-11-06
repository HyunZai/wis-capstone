
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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class CC : MonoBehaviour
{   
    public GameObject player;    //메인 캐릭터 지정
    float moveSpeed = 4.0f;
    GameObject[] characterList, crossPoints ,destinationPoints ,buildingPoints;    //캐릭터 설정 리스트 ++++ 캐릭터 목록 추가점요
    Button[] setDistinationBtns;// 빌딩목록 넣기
    public int destinationNum ,beforeDestination; 
    Vector2 [] cp, dp, bp;
    static Vector2 pp;
    Animator anim;
    Vector2 beforePP;
    public Vector2 isMoved;
    private int homeRoute1, homeRoute2;
    public GameObject popup;
    public float popupDelay = 2.0f;
    TextMeshProUGUI askText;
    private bool goHomeMode, isMove = false;
    
    public GameObject buildingBtns;
    
    public int setHome= 4;
    public Button yBtn, nBtn, goHomeBtn;
    public VideoClip[] videoClips;
    private VideoPlayer videoPlayer;
    void Awake(){
        SetBeforeStart();
    }
    void Start(){
       
    }
    void Update(){
        pp = player.transform.position;  
        MoveAnimation();
    }
    void SetBeforeStart(){ 
        popup.SetActive(true);
        characterList = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name).ToArray();
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DestinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("BuildingPoint").OrderBy(building =>  building.name).ToArray();
        setDistinationBtns = buildingBtns.GetComponentsInChildren<Button>().OrderBy(btn => btn.name).ToArray();
        askText = popup.GetComponentInChildren<TextMeshProUGUI>();


        if (anim == null) anim = GetComponent<Animator>();  

        if (videoPlayer == null) videoPlayer = FindObjectOfType<VideoPlayer>();
        else if (videoPlayer != null) videoPlayer.loopPointReached += EndReached; // 중간에 검은 화면 추가 필요함

        nBtn.onClick.AddListener(() => NoButtonClick());
        yBtn.onClick.AddListener(()=> YesButtonClick());
        goHomeBtn.onClick.AddListener(()=> GoHomeButtonClick());

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

        int loadPosNum = PlayerPrefs.GetInt("DestinationPoinNum", setHome); // 씬 복원될떄 위치 번호 불러오기 없으면 setHome 위치 
        destinationNum = loadPosNum;
        beforeDestination = loadPosNum;
        player.transform.position = bp[loadPosNum];


        popup.SetActive(false);
        
    }
    void NoButtonClick(){
        popup.SetActive(false);
        ActivateBuildingBtns();
    }
    void YesButtonClick(){
        AskGOHomeMode();
        PlayerPrefs.SetInt("DestinationPoinNum", destinationNum);
        PlayerPrefs.Save();
        videoPlayer.Play();
        }
    void GoHomeButtonClick(){
        ActivateGoHomeMode();
        ActivateBuildingBtns(); 
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
        beforeDestination = destinationNum;
        ShowPopup();
        isMove = false;
    }
    void SetDestination(int gotoHere){
        if(destinationNum != gotoHere && isMove == false)
        {
            if(!goHomeMode)
            {
                if(gotoHere == setHome){
                    SetPopup($"아직은 집으로 갈때가 아니에요!\n좀 더 동네를 탐험해볼까요?");
                }else{
                    destinationNum = gotoHere;
                    StartCoroutine(GoDestination());
                }
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
                    SetPopup($"{BuildingName(homeRoute1)}으로 이동해주세요!");
                }else if(destinationNum == homeRoute1 && gotoHere != homeRoute2){
                    SetPopup($"{BuildingName(homeRoute2)}으로 이동해주세요!");
                }else if(destinationNum == homeRoute2 && gotoHere != setHome){
                    SetPopup($"{BuildingName(setHome)}으로 이동해주세요!");
                }else if(destinationNum == setHome){
                    SetPopup("집에 도착했어요! 저장완료!");
                }                
            }
            
        }
        else if(destinationNum == gotoHere)
        {
            SetPopup("다른 건물을 선택해주세요!");
        }
        else if(isMove == true)
        {
            SetPopup("캐릭터가 이동 중 입니다!");
        }
    }
    void MoveAnimation(){
        if(isMove){
            isMoved = pp - beforePP;
            anim.SetFloat("inputx", isMoved.x);
            anim.SetFloat("inputy", isMoved.y);
            // 애니메이션 추가만 하면 끝난다잇
        }
    }
    void AskGOHomeMode(){
        nBtn.gameObject.SetActive(true);
        yBtn.gameObject.SetActive(false);
        goHomeBtn.gameObject.SetActive(true);
        askText.text = "집으로 돌아갈까요?";
        popup.SetActive(true);
    }
    void ActivateGoHomeMode(){
        goHomeMode = true;
        do {
            homeRoute1 = Random.Range(0, buildingPoints.Length);
        } while (homeRoute1 == beforeDestination && setHome == homeRoute1); 
        
        do {
            homeRoute2 = Random.Range(0, buildingPoints.Length);
        } while (homeRoute2 == beforeDestination && homeRoute2 == homeRoute1 && setHome == homeRoute2);
        ShowPopup();
       
    }
    string BuildingName(int PointNum){
        string stringName;
        switch (PointNum){
                case 0: stringName = "학교"; break;
                case 1: stringName = "카페"; break;
                case 2: stringName = "소방서"; break;
                case 3: stringName = "도서관"; break;
                case 4: stringName = "집"; break;
                case 5: stringName = "마켓"; break;
                case 6: stringName = "경찰서"; break;
                case 7: stringName = "은행"; break;
                case 8: stringName = "병원"; break;
                default: stringName = "알 수 없음"; break;
            }
        return stringName;
    }
    void ShowPopup(){
        DeActivateBuildingBtns();
        popup.SetActive(true);
        string playerLocation = BuildingName(destinationNum);
        if(!goHomeMode){
            if(Vector2.Distance(pp, bp[destinationNum]) < 0.1){
                nBtn.gameObject.SetActive(true);
                goHomeBtn.gameObject.SetActive(false);
                yBtn.gameObject.SetActive(true);
                askText.text = $"{playerLocation}에 들어갈까요?";
                //go in buildinig
                //out buildin
            }
        }
        else if(goHomeMode){
             if(destinationNum !=homeRoute2 && destinationNum != homeRoute1 && destinationNum != setHome){
                SetPopup($"먼저 집으로 가기 위해서 {BuildingName(homeRoute1)}로 가볼까요?");
            }else if(destinationNum == homeRoute1){
                SetPopup($"잘 도착했어요!\n이제 {BuildingName(homeRoute2)}로 이동한 뒤에 집으로 가볼까요?");
            }else if(destinationNum == homeRoute2){
                SetPopup("집으로 가볼까요?");
            }else if(destinationNum == setHome){
                SetPopup("집에 도착했어요! 저장완료!");
        }
        } 

        
    }
    void SetPopup(string a){
        askText.text = a;
        yBtn.gameObject.SetActive(false);
        nBtn.gameObject.SetActive(false);
        goHomeBtn.gameObject.SetActive(false);
        DeActivateBuildingBtns(); 

        popup.SetActive(true);
        StartCoroutine(AutoClosePopup(popupDelay));  
    }
    IEnumerator AutoClosePopup(float closeTime){
        yield return new WaitForSeconds(closeTime);
        popup.SetActive(false);
        ActivateBuildingBtns();
    }
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "BuildingPoint" && videoPlayer != null) 
        {
            string num = other.gameObject.name.Split(".")[0];
            switch (other.gameObject.name)
            {
                case "0.School":
                    videoPlayer.clip = videoClips[0];
                    break;
                case "1.Cafe":
                    videoPlayer.clip = videoClips[1];
                    break;
                case "2.FireStation":
                    videoPlayer.clip = videoClips[2];
                    break;
                case "3.Library":
                    videoPlayer.clip = videoClips[3];
                    break;
                case "4.Home":
                    videoPlayer.clip = videoClips[4];
                    break;
                case "5.Market":
                    videoPlayer.clip = videoClips[5];
                    break;
                case "6.PoliceOffice":
                    videoPlayer.clip = videoClips[6];
                    break;
                case "7.Bank":
                    videoPlayer.clip = videoClips[7];
                    break;
                case "8.Hospital":
                    videoPlayer.clip = videoClips[8];
                    break;
                
            }
        }
    }
    void EndReached(VideoPlayer vp)
    {
        PlayerPrefs.SetString("BuildingName", vp.clip.name.Split("_")[0]);
        SceneManager.LoadScene("InformationScene");
    }
    void OnApplicationQuit() {
        PlayerPrefs.DeleteAll();
    }
    void DeActivateBuildingBtns(){
        for(int i = 0; i< setDistinationBtns.Length; i++){
            setDistinationBtns[i].gameObject.SetActive(false);   
        }
       
    }
    void ActivateBuildingBtns(){
        for(int i = 0; i< setDistinationBtns.Length; i++){
            setDistinationBtns[i].gameObject.SetActive(true);   
        }
    }
}
