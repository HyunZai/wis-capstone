using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CharacterControler : MonoBehaviour
{   
    public RuntimeAnimatorController [] animList;
    public VideoClip[] videoClips;  // change private
    public AudioClip[] audioClips; //오디오 클립 배열(리스트)
    public AudioSource audioSource; //오디오 파일 컨트롤
    public GameObject buildingButtons, popup, smartPhone;

    private int setHome= 4;
    private Vector2 pp, beforePP, isMoved;
    private float moveSpeed = 4.0f;
    private GameObject[] crossPoints ,destinationPoints ,buildingPoints;
    private Button[] setDestinationBtns;// 빌딩목록 넣기
    private int destinationNum ,beforeDestinationNum, routeCheckCount; 
    private Vector2 [] cp, dp, bp;
    private Animator anim;

    private TextMeshProUGUI askText;
    private bool goHomeMode, isMoveNow = false;
    

    private Button noBtn, goHomeBtn, quitGameBtn; // chagne private
    private VideoPlayer videoPlayer;
    private DatabaseManager dbManager;
    private List<int> homeRouteList;
    
    private SmartPhone smartPhoneScript;
    private User user;

    ///////////Codes
    void Awake(){
        
        SetBeforeStart();
    }
    void Start(){
        
        //------------------------------건물에서 나왔을 때 재생되는 영상 실행------------------------------
        string buildingName = PlayerPrefs.GetString("BuildingName");
        if (!string.IsNullOrEmpty(buildingName))
        {
            string gender = user.gender == 0 ? "male" : "female";
            string videoFileName = $"{buildingName}_out_{gender}";

            foreach (VideoClip clip in videoClips) 
            {
                if (clip.name.Contains(videoFileName)) 
                {
                    videoPlayer.clip = clip; 
                    break;
                }
            }
            videoPlayer.loopPointReached -= EndReached;
            videoPlayer.loopPointReached += askGoHomeAudioPlay;
            videoPlayer.Play();
        }
        //------------------------------건물에서 나왔을 때 재생되는 영상 실행------------------------------
        Time.timeScale = 1f;
        
    }
    void Update(){
        Player();
    }
    private void SetBeforeStart(){ 
        StopAllCoroutines();

        LoadDBManager();
        FindGameObjectInScene();
        StatBuildingButtons(true);
        AddButtonListener();
        ForDeactivateObjects();
        LoadPosNum();
        
        PlayerPrefs.SetInt("userId", user.user_id);   
        
        }
    //beforeStart
    private void FindGameObjectInScene(){
        //points
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DestinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("BuildingPoint").OrderBy(building =>  building.name).ToArray();

        //set popup
        askText = popup.GetComponentInChildren<TextMeshProUGUI>(true);

        //buttons
        setDestinationBtns = buildingButtons.GetComponentsInChildren<Button>().OrderBy(btn => btn.name).ToArray();
        noBtn = popup.transform.Find("notGoHomeButton").GetComponent<Button>();
        goHomeBtn = popup.transform.Find("goHomeButton").GetComponent<Button>();
        quitGameBtn = popup.transform.Find("QuitGameButton").GetComponent<Button>();
        videoPlayer = FindObjectOfType<VideoPlayer>();
        
        anim = this.GetComponent<Animator>(); 
        anim.runtimeAnimatorController = animList[user.gender]; 
        //smartPhone
        smartPhoneScript = smartPhone.GetComponent<SmartPhone>();
        smartPhoneScript.OnConditionMet += GoHomeStart; //부모님 전화번호 입력 성공 후 귀가경로 안내가 시작되도록 처리하기 위한..


        //for utility
        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[destinationPoints.Length];
        bp = new Vector2[buildingPoints.Length]; 
    }
    private void Player(){
        pp = this.transform.position; 
        MoveAnimation();
    }
    private void LoadPosNum(){
        int loadPosNum = PlayerPrefs.GetInt("DestinationPointNum", setHome); // 씬 복원될떄 위치 번호 불러오기 없으면 setHome 위치
        
        destinationNum = loadPosNum;
        beforeDestinationNum = loadPosNum;

        if(!goHomeMode)this.transform.position = (dp[loadPosNum] + bp[loadPosNum])/2;
        if(destinationNum != setHome) PopupForAsk("집으로 돌아갈까요?");
    }
    private void LoadDBManager(){
        dbManager = new DatabaseManager();
        dbManager.Connect();
        user = dbManager.login();
    }
    private void AddButtonListener(){
        noBtn.onClick.AddListener(() => NoButtonClick());
        quitGameBtn.onClick.AddListener(() => QuitGameButtonClick());
        goHomeBtn.onClick.AddListener(()=> GoHomeButtonClick());

        for(int i = 0; i < destinationPoints.Length; i++){        
            
            if(i < crossPoints.Length) cp[i] = crossPoints[i].transform.position;
            
            dp[i] = destinationPoints[i].transform.position;
            bp[i] = buildingPoints[i].transform.position;
            int index = i;
            setDestinationBtns[index].onClick.AddListener(() => SetDestination(index));     
        }
    }
    private void ForDeactivateObjects(){
        popup.SetActive(false);
        noBtn.gameObject.SetActive(false);
        goHomeBtn.gameObject.SetActive(false);
        quitGameBtn.gameObject.SetActive(false);
    }


    ///////////For Popup Button
    private void QuitGameButtonClick(){
        Application.Quit();
    #if UNITY_EDITOR
            // 에디터 모드에서는 EditorApplication을 종료
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
    private void NoButtonClick(){
        popup.SetActive(false);
        StatBuildingButtons(true);
    }
    private void GoHomeButtonClick(){
        goHomeMode = true;
        
        smartPhone.SetActive(true); //스마트폰 활성화
        audioSource.clip = audioClips[1]; //재생시킬 오디오 파일 선택("부모님 전화번호를 입력해주세요.")
        audioSource.Play(); //오디오 파일 재생
    }
    private void GoHomeStart(){
        popup.SetActive(true);
        SetHomeRoute();
        SetDestination(-1);
        StatBuildingButtons(true);
    }
   
    //////////// For Move 
    private(int numB, int numP) SetCrossPoint(){
        int numB = int.MaxValue;
        float minB= float.MaxValue;

        for(int i =0; i < crossPoints.Length; i++){
            if(Mathf.Abs(dp[destinationNum].x - cp[i].x) < 0.01f 
            || Mathf.Abs(dp[destinationNum].y-cp[i].y) < 0.01f){

                if(minB > Vector2.Distance(pp ,cp[i])){
                    minB = Vector2.Distance(pp ,cp[i]);
                    numB = i;
                }
            }
        }
        int numP = numB;

        for(int i =0; i< crossPoints.Length; i++){   
            if ((Mathf.Abs(cp[i].x - cp[numB].x) < 0.01f 
                || Mathf.Abs(cp[i].y-cp[numB].y) < 0.01f) 
                && (Mathf.Abs(pp.x - cp[i].x) < 0.01f 
                || Mathf.Abs(pp.y - cp[i].y) < 0.01f)) {

                if (numB ==i) {
                    numP =i;
                    break;
                }
                numP = i;
            }
        }
        return (numB,  numP);
    }
    private void SetDestination(int gotoHere){
        if(isMoveNow == false){
            if(!goHomeMode){
                if(gotoHere == setHome){
                    PopupForInstance($"아직은 집으로 갈때가 아니에요!\n동네의 다른 장소들을 탐험해볼까요?");
                }else{
                    destinationNum = gotoHere;
                    StartCoroutine(GoSetDestination());
                }
            }
            else if(goHomeMode){
                if(routeCheckCount < homeRouteList.Count){
                    if(gotoHere==homeRouteList[routeCheckCount]){
                        destinationNum = gotoHere;
                        routeCheckCount++;
                        StartCoroutine(GoSetDestination());
                    }else{
                        PopupForInstance($"{BuildingName(homeRouteList[routeCheckCount])} 이동해주세요!!");
                    }
                }
            }  
        }else if(isMoveNow == true){
            PopupForInstance("가고있어요!");
        }
    }
    private IEnumerator GoSetDestination(){ 
        isMoveNow = true;
        beforePP = pp;
        
        PlayerPrefs.SetInt("DestinationPoinNum", destinationNum);
        PlayerPrefs.Save();

        if(beforeDestinationNum != destinationNum || beforeDestinationNum == setHome || goHomeMode){
            while(Vector2.Distance(pp,dp[beforeDestinationNum]) >= 0.01f){
                this.transform.position = Vector2.MoveTowards(pp,dp[beforeDestinationNum],moveSpeed* Time.deltaTime);
                yield return null;
            }
            beforePP = pp;

            var (numB, numP) = SetCrossPoint();

            if( !( (Mathf.Abs(pp.x - dp[destinationNum].x) < 0.01f) || (Mathf.Abs(pp.y-dp[destinationNum].y) < 0.01f) )){
                while(Vector2.Distance(pp,cp[numP]) > 0.01f){
                    this.transform.position = Vector2.MoveTowards(pp,cp[numP],moveSpeed* Time.deltaTime);
                    yield return null;
                }
                beforePP = pp;

                while(Vector2.Distance(pp,cp[numB]) > 0.01f){
                    this.transform.position = Vector2.MoveTowards(pp,cp[numB],moveSpeed* Time.deltaTime);
                    if(Vector2.Distance(pp,dp[destinationNum])<0.01f)yield break;
                    yield return null;
                }
                beforePP = pp;
            }

            while(Vector2.Distance(pp,dp[destinationNum]) > 0.01f){
                this.transform.position = Vector2.MoveTowards(pp,dp[destinationNum],moveSpeed* Time.deltaTime);
                yield return null;
            }
            beforePP = pp;
        }
        while(Vector2.Distance(pp,bp[destinationNum]) >= 0.01f){
            this.transform.position = Vector2.MoveTowards(pp,bp[destinationNum],moveSpeed* Time.deltaTime);
            yield return null;
        }
        beforePP = pp;

        isMoveNow = false;
        beforeDestinationNum = destinationNum;

        if(goHomeMode){ 
            if(destinationNum == setHome) {
                PopupForAsk("집에 도착했어요!\n게임을 종료할까요?");
                goHomeMode = false;
            }else {
                SetDestination(-1);
            }
        }    
        
    }
    
    ///////////// For Animation
    private void MoveAnimation(){
        if(isMoveNow){
            anim.SetBool("ismove", true);
            isMoved.x = pp.x - beforePP.x;
            isMoved.y = pp.y - beforePP.y;
            if(MathF.Abs(isMoved.x) > MathF.Abs(isMoved.y)){
                anim.SetFloat("inputx", isMoved.x > 0 ? 1.0f : -1.0f);
                anim.SetFloat("inputy", 0.0f);
            }else if(MathF.Abs(isMoved.x) < MathF.Abs(isMoved.y)){
                anim.SetFloat("inputx", 0.0f);
                anim.SetFloat("inputy", isMoved.y > 0 ? 1.0f : -1.0f);
            }
        }else if(!isMoveNow){
            anim.SetBool("ismove", false);
            anim.SetFloat("inputx", 0);
            anim.SetFloat("inputy", 0);
        }

    }
    private void SetHomeRoute(){
        dbManager = new DatabaseManager();
        dbManager.Connect();
       
        List<HomeRoute> homeRoute = dbManager.getGoHomeRoute(destinationNum + 1);
        homeRouteList = homeRoute.Select(hr => hr.next_building-1).ToList();
    }
    private string BuildingName(int PointNum){
        string stringName;
        if(!goHomeMode){ 
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

        }else if(goHomeMode){
            switch (PointNum){
                case 0: stringName = "학교로"; break;
                case 1: stringName = "카페로"; break;
                case 2: stringName = "소방서로"; break;
                case 3: stringName = "도서관으로"; break;
                case 4: stringName = "집으로"; break;
                case 5: stringName = "마켓으로"; break;
                case 6: stringName = "경찰서로"; break;
                case 7: stringName = "은행으로"; break;
                case 8: stringName = "병원으로"; break;
                default: stringName = "알 수 없음"; break;
            }
            return stringName;
        }else return null;
    }

    // popup 
    private void PopupForInstance(string showMessage){
        askText.text = showMessage;

        SetPopup();
        StatPopupButton(false);
        StartCoroutine(PopupCloseDelay());  
    }
    private void PopupForAsk(string showMessage){
        askText.text = showMessage;

        SetPopup();
        StatPopupButton(true);
    }
    private void SetPopup(){
        popup.SetActive(true);

        audioSource.clip = audioClips[0]; //재생시킬 오디오 파일 선택
        audioSource.Play(); //오디오 파일 재생
    }
    private IEnumerator PopupCloseDelay(){
        yield return new WaitForSeconds(2.0f);
        popup.SetActive(false);
        StatBuildingButtons(true);
    }
    private void StatPopupButton(bool stat){
        StatBuildingButtons(false); 
        goHomeBtn.gameObject.SetActive(false);
        quitGameBtn.gameObject.SetActive(false);


        noBtn.gameObject.SetActive(stat);
        if(!goHomeMode)goHomeBtn.gameObject.SetActive(stat);
        if(goHomeMode)quitGameBtn.gameObject.SetActive(stat);
    }
    /////////For Move Scenes
    private void OnTriggerEnter2D(Collider2D other) { 
        if (other.gameObject.tag == "BuildingPoint" && videoPlayer != null && !goHomeMode) 
        {
            string buildingName = other.gameObject.name.Split(".")[1];
            string gender = user.gender == 0 ? "male" : "female";
            string videoFileName = $"{buildingName}_in_{gender}";
            
            foreach (VideoClip clip in videoClips) 
            {
                if (clip.name.Contains(videoFileName)) 
                {
                    videoPlayer.clip = clip; 
                    break;
                }
            }

            PlayerPrefs.SetInt(buildingName + "VisitCount", PlayerPrefs.GetInt(buildingName + "VisitCount") + 1);

            videoPlayer.loopPointReached += EndReached;
            videoPlayer.Play();
        }
    }
    private void EndReached(VideoPlayer vp)
    {
        PlayerPrefs.SetString("BuildingName", vp.clip.name.Split("_")[0]);
        PlayerPrefs.Save();
        SceneManager.LoadScene("InformationScene");
    }
    private void OnApplicationQuit() {
        PlayerPrefs.DeleteAll();
    }

    ////////////For Me
    private void StatBuildingButtons(bool stat){
        for(int i = 0; i < setDestinationBtns.Length; i++){
            setDestinationBtns[i].gameObject.SetActive(stat);   
        }
    }
    void askGoHomeAudioPlay(VideoPlayer vp)
    {
        //재생시킬 오디오 파일 선택
        audioSource.clip = audioClips[0];
        audioSource.Play(); //오디오 파일 재생
    }
}
