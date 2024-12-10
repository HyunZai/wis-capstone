using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayerController : MonoBehaviour
{   
    public GameObject eventSystem; //영상 재생되는 동안 다른 오브젝트 상호작용 차단

    public RuntimeAnimatorController[] animList;
    public int setHome =4;
    private float moveSpeed = 4.0f;
    private GameObject[] crossPoints ,destinationPoints ,buildingPoints;
    
    public int destinationNum ,beforeDestination, routeCheckCount; 
    private Vector2 [] cp, dp, bp;
    private Animator anim;
    public GameObject popup;   //change private
    private float popupDelay = 2.0f;
    private TextMeshProUGUI askText;
    public bool goHomeMode, isMoveNow = false;
    
    
    public Button nBtn, goHomeBtn; // chagne private
    public VideoClip[] videoClips;  // change private
    public VideoPlayer videoPlayer;
    private DatabaseManager dbManager;
    public List<int> homeRouteList;
    
    public AudioSource audioSource; //오디오 파일 컨트롤
    public AudioClip[] audioClips; //오디오 클립 배열(리스트)
    public GameObject smartPhone;
    private SmartPhone smartPhoneScript;

    private User user;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private bool stopPlayer = false;
    public GameObject painter;

    ///////////Codes
    void Awake(){
        SetBeforeStart();
        

       
    }
    void Start(){
        Time.timeScale = 1f;
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
            eventSystem.SetActive(false);
            videoPlayer.Play();
        }
       
        smartPhoneScript = smartPhone.GetComponent<SmartPhone>();
        smartPhoneScript.OnConditionMet +=  GetHomeRoute;
    }

    void Update(){
        MovePlayer(); 
    }
    ///////////For BeforeStart    
    void SetBeforeStart(){ 
        DBManagerStart();
        FindGameObjects();
        SetValues();
        AddButtonListener();
    }
    void AddButtonListener(){
        nBtn.onClick.AddListener(() => NoButtonClick());
        goHomeBtn.onClick.AddListener(()=> GoHomeButtonClick());
    }
    void DBManagerStart(){
        dbManager = new DatabaseManager();
        dbManager.Connect();
        user = dbManager.login();
    }
    void FindGameObjects(){
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DestinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("BuildingPoint").OrderBy(building =>  building.name).ToArray();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        askText = popup.GetComponentInChildren<TextMeshProUGUI>();
        anim = GetComponent<Animator>(); 
        //for utile
        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[destinationPoints.Length];
        bp = new Vector2[buildingPoints.Length]; 

         for(int i = 0; i < destinationPoints.Length; i++){        
            if(i<crossPoints.Length)cp[i] = crossPoints[i].transform.position;
            dp[i] = destinationPoints[i].transform.position;
            bp[i] = buildingPoints[i].transform.position;  
        }
    }
    void SetValues(){
        PlayerPrefs.SetInt("userId", user.user_id);
        destinationNum = PlayerPrefs.GetInt("DestinationPointNum", setHome);  
        Debug.Log("DestintaionNum : "+destinationNum);
        anim.runtimeAnimatorController = animList[user.gender]; 
        if(destinationNum != setHome) PopupAskGoHome();    
        transform.position = dp[destinationNum];
    }
    //Button
    void NoButtonClick(){
        popup.SetActive(false);
        SetButtonsStat(false);
        stopPlayer = false;
    }
    void GoHomeButtonClick(){
        popup.SetActive(false); //팝업 비활성화
        stopPlayer = true;
        smartPhone.SetActive(true); //스마트폰 활성화
        
        audioSource.clip = audioClips[1]; //재생시킬 오디오 파일 선택("부모님 전화번호를 입력해주세요.")
        audioSource.Play(); //오디오 파일 재생
    }
   

    private AudioClip GetGoNextBuildingClip(int nextBuildingId){
        string clipName = "Go";
        switch (nextBuildingId){
                case 0: clipName += "School"; break;
                case 1: clipName += "Cafe"; break;
                case 2: clipName += "FireStation"; break;
                case 3: clipName += "Library"; break;
                case 4: clipName += "Home"; break;
                case 5: clipName += "Mart"; break;
                case 6: clipName += "Police"; break;
                case 7: clipName += "Bank"; break;
                case 8: clipName += "Hospital"; break;
        }

        return audioClips.Where(clip => clip.name == clipName).ToArray().First();
    }
    
    ///////////// For Animation
    void MovePlayer(){
       if(!stopPlayer){ 
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            UpdateAnimation();
        }
    }
     void UpdateAnimation(){
        
            animator.SetFloat("inputx", movement.x);
            animator.SetFloat("inputy", movement.y);
    
    }
    void FixedUpdate(){
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
    //popup
    void PopupAskGoHome(){
        askText.text = "집으로 돌아갈까요?";
        stopPlayer= false;

        SetButtonsStat(true);
        popup.SetActive(true);
    }
    void PopupInstance(string showMessage){
        askText.text = showMessage;
        StartCoroutine(PopupCloseDelay(popupDelay));  
    }
    IEnumerator PopupCloseDelay(float closeTime){
        stopPlayer = true;
        SetButtonsStat(false);
        popup.SetActive(true);
        yield return new WaitForSeconds(closeTime);
        popup.SetActive(false);
        stopPlayer= false;
    }
    void SetButtonsStat(bool stat){
        nBtn.gameObject.SetActive(stat);
        goHomeBtn.gameObject.SetActive(stat);
    }
    //
    void GetHomeRoute(){
        popup.SetActive(false);
        stopPlayer = false;
        goHomeMode = true;

        dbManager.Connect();

        List<HomeRoute> homeRoute = dbManager.getGoHomeRoute(destinationNum + 1);

        if (homeRoute[0].building_id != destinationNum + 1 && homeRoute.Count == 2) { //소방서, 병원에서 집으로 돌아갈 때 DB에서 Select해오는 데이터의 순서가 바뀌어 마트를 거치지 않고 바로 집으로 이동하는 이슈 발생 -> 아래 코드를 임시방편으로 해결함(sql쿼리를 수정해서 근본적인 원인을 해결해야됨)
            HomeRoute temp = homeRoute[0];
            homeRoute[0] = homeRoute[1];
            homeRoute[1] = temp;
        }
        
        homeRouteList = homeRoute.Select(hr => hr.next_building-1).ToList();
        for(int i = 0; i < homeRoute.Count; i++){
            Debug.Log($"homeRoute Num{i} : " + homeRouteList[i]);
        }
        PopupInstance($"길을 따라서 {BuildingName(homeRouteList[routeCheckCount])} 출발해볼까요?");
        
    }
    string BuildingName(int PointNum){
        string stringName;
        switch (PointNum){
                case 0: stringName = "학교로"; break;
                case 1: stringName = "카페로"; break;
                case 2: stringName = "소방서로"; break;
                case 3: stringName = "도서관으로"; break;
                case 4: stringName = "집으로"; break;
                case 5: stringName = "마트로"; break;
                case 6: stringName = "경찰서로"; break;
                case 7: stringName = "은행으로"; break;
                case 8: stringName = "병원으로"; break;
                default: stringName = "알 수 없음"; break;
        }
        return stringName;
    }
    
    /////////For Move Scenes
    private void OnTriggerEnter2D(Collider2D other) { 
        if (other.name.Contains("."))
        {
            string buildingName = other.name.Split(".")[1];
            int buildingNum = int.Parse(other.name.Split(".")[0]);
            destinationNum =  buildingNum;

            if (other.gameObject.tag == "BuildingPoint"  && videoPlayer != null ) {
                if(goHomeMode){
                    if(destinationNum !=  homeRouteList[routeCheckCount]){
                        WrongRouteWarniing();
                    }
                    else if(homeRouteList[routeCheckCount] == destinationNum){
                        if(homeRouteList[routeCheckCount] != setHome){
                            routeCheckCount+=1;
                            PopupInstance($"잘 도착했어요!\n다음 목적지인 {BuildingName(homeRouteList[routeCheckCount])} 이동해주세요!!");
                        }
                        else if(homeRouteList[routeCheckCount] == setHome){
                            goHomeMode = false;
                            PopupInstance("집에 잘 도착했네요!!\n축하해요!");
                        }
                    }

                }else if(!goHomeMode){
                    if(buildingNum == setHome){
                        WrongRouteWarniing();
                    }else{
                        string gender = user.gender == 0 ? "male" : "female";
                        string videoFileName = $"{buildingName}_in_{gender}";
                    
                        foreach (VideoClip clip in videoClips) {
                            if (clip.name.Contains(videoFileName)){
                                videoPlayer.clip = clip; 
                                break;
                            }
                        }

                        PlayerPrefs.SetInt(buildingName + "VisitCount", PlayerPrefs.GetInt(buildingName + "VisitCount") + 1);
                        PlayerPrefs.SetInt("DestinationPointNum",destinationNum);  
                        videoPlayer.loopPointReached += EndReached;

                        eventSystem.SetActive(false);
                        Time.timeScale = 0f;
                        PlayerPrefs.Save();
                        videoPlayer.Play();
                    }
                }
            }
        }
    }
    void WrongRouteWarniing(){
        stopPlayer= true;
        movement.x =0;
        movement.y =0;
        UpdateAnimation();
        transform.position = dp[destinationNum];

        if(goHomeMode)PopupInstance($"여기가 아니에요!\n{BuildingName(homeRouteList[routeCheckCount])} 이동해 볼까요?");
        else if(!goHomeMode) PopupInstance("여기가 아니에요!\n다른 곳으로 이동해주세요!!");
        else PopupInstance("알수없는 에러");
        
        audioSource.Play(); //오디오 파일 재생
        audioSource.clip = GetGoNextBuildingClip(homeRouteList[routeCheckCount]);
    }
    void EndReached(VideoPlayer vp){
        eventSystem.SetActive(true);
        PlayerPrefs.SetString("BuildingName", vp.clip.name.Split("_")[0]);
        Time.timeScale = 1f;
        SceneManager.LoadScene("InformationScene");
    }
    void askGoHomeAudioPlay(VideoPlayer vp){
        eventSystem.SetActive(true);
        //재생시킬 오디오 파일 선택
        audioSource.clip = audioClips[0];
        audioSource.Play(); //오디오 파일 재생
    }
    
    void OnApplicationQuit() {
        PlayerPrefs.DeleteAll();
    }

}
