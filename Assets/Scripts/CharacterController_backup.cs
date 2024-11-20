using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CharacterSensorController : MonoBehaviour
{   
    public AnimatorController [] animList;
    public int setHome= 4;
    public Image a;


    private static Vector2 pp;
    private float moveSpeed = 4.0f;
    private GameObject[] crossPoints ,destinationPoints ,buildingPoints;    //캐릭터 설정 리스트 ++++ 캐릭터 목록 추가점요
    private Button[] setDistinationBtns;// 빌딩목록 넣기
    private int destinationNum ,beforeDestination, routeCheckCount; 
    private Vector2 [] cp, dp, bp;
    private Animator anim;
    private Vector2 beforePP, isMoved;
    public GameObject popup;   //change private
    private float popupDelay = 2.0f;
    private TextMeshProUGUI askText;
    private bool goHomeMode, isMoveNow = false;
    
    public GameObject buildingBtns; // after delete
    public Button nBtn, goHomeBtn; // chagne private
    public VideoClip[] videoClips;  // change private
    private VideoPlayer videoPlayer;
    private DatabaseManager dbManager;
    private List<int> homeRouteList;
    

    public AudioSource audioSource; //오디오 파일 컨트롤
    public AudioClip[] audioClips; //오디오 클립 배열(리스트)
    public GameObject smartPhone;

    ///////////Codes
    void Awake(){
        SetBeforeStart();
    }
    void Start(){
    }
    void Update(){
         
        MoveAnimation();
        pp = this.transform.position; 
    }
    ///////////For BeforeStart    
    void SetBeforeStart(){ 
        Time.timeScale = 1f;

        dbManager = new DatabaseManager();
        dbManager.Connect();
        User user = dbManager.login();

        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DestinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("BuildingPoint").OrderBy(building =>  building.name).ToArray();
        setDistinationBtns = buildingBtns.GetComponentsInChildren<Button>().OrderBy(btn => btn.name).ToArray();
        askText = popup.GetComponentInChildren<TextMeshProUGUI>();
        videoPlayer = FindObjectOfType<VideoPlayer>();
       //characterList = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name.Contains("Male") ? 0 : 1).ToArray();


        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[destinationPoints.Length];
        bp = new Vector2[buildingPoints.Length];    

        


        nBtn.onClick.AddListener(() => NoButtonClick());
        goHomeBtn.onClick.AddListener(()=> GoHomeButtonClick());
        
        for(int i = 0; i < destinationPoints.Length; i++){        
            if(i<crossPoints.Length)cp[i] = crossPoints[i].transform.position;
            dp[i] = destinationPoints[i].transform.position;
            bp[i] = buildingPoints[i].transform.position;
            int index = i;
            setDistinationBtns[index].onClick.AddListener(()=>SetDestination(index));     
        }

        int loadPosNum = PlayerPrefs.GetInt("DestinationPoinNum", setHome); // 씬 복원될떄 위치 번호 불러오기 없으면 setHome 위치 
    
        destinationNum = loadPosNum;
        beforeDestination = loadPosNum;
        if(!goHomeMode)this.transform.position = (dp[loadPosNum] + bp[loadPosNum])/2;

        
        if (anim == null) anim = GetComponent<Animator>(); 
        anim.runtimeAnimatorController = animList[user.gender]; 
        
        videoPlayer.loopPointReached += EndReached;
        if(destinationNum != setHome) {
            AskGoHomePopup();
        }
    }
    
    ///////////For Popup Button
    void NoButtonClick(){
        popup.SetActive(false);
        ActivateBuildingBtns();
    }
    void GoHomeButtonClick(){
        smartPhone.SetActive(true);
        
        //재생시킬 오디오 파일 선택
        audioSource.clip = audioClips[1];
        audioSource.Play(); //오디오 파일 재생

        SetHomeRoute();
        SetDestination(-1);
        ActivateBuildingBtns(); 
    }
   
    //////////// For Move 
    (int numB, int numP) SetCrossPoint(){
        int numB = int.MaxValue;
        float minB= float.MaxValue;
        for(int i =0; i < crossPoints.Length; i++){
            if(Mathf.Abs(dp[destinationNum].x - cp[i].x) < 0.01f 
            || Mathf.Abs(dp[destinationNum].y-cp[i].y) < 0.01f)
            {
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
    void SetDestination(int gotoHere){
        if(isMoveNow == false){
            if(!goHomeMode){
                if(gotoHere == setHome){
                    ShowPopup($"아직은 집으로 갈때가 아니에요!\n좀 더 동네를 탐험해볼까요?");
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
                        ShowPopup($"{BuildingName(homeRouteList[routeCheckCount])}으로 이동해주세요!!");
                    }
                }
            }  
        }else if(isMoveNow == true){
            ShowPopup("캐릭터가 이동 중 입니다!");
        }
    }
    IEnumerator GoSetDestination(){ 
        isMoveNow = true;
        beforePP = pp;

        if(beforeDestination != destinationNum || beforeDestination == setHome || goHomeMode){
            while(Vector2.Distance(pp,dp[beforeDestination])>=0.01f){
                this.transform.position = Vector2.MoveTowards(pp,dp[beforeDestination],moveSpeed* Time.deltaTime);
                yield return null;
            }
            beforePP = pp;

            var (numB, numP) = SetCrossPoint();

            if(!(Mathf.Abs(pp.x - dp[destinationNum].x) < 0.01f || Mathf.Abs(pp.y-dp[destinationNum].y)<0.01f)){
                while(Vector2.Distance(pp,cp[numP])>0.01f){
                    this.transform.position = Vector2.MoveTowards(pp,cp[numP],moveSpeed* Time.deltaTime);
                    yield return null;
                }
                beforePP = pp;

                while(Vector2.Distance(pp,cp[numB])>0.01f){
                    this.transform.position = Vector2.MoveTowards(pp,cp[numB],moveSpeed* Time.deltaTime);
                    if(Vector2.Distance(pp,dp[destinationNum])<0.01f)yield break;
                    yield return null;
                }
                beforePP = pp;
            }

            while(Vector2.Distance(pp,dp[destinationNum])>0.01f){
                this.transform.position = Vector2.MoveTowards(pp,dp[destinationNum],moveSpeed* Time.deltaTime);
                yield return null;
            }
            beforePP = pp;
        }
        while(Vector2.Distance(pp,bp[destinationNum])>=0.001f){
            this.transform.position = Vector2.MoveTowards(pp,bp[destinationNum],moveSpeed* Time.deltaTime);
            yield return null;
        }
        beforePP = pp;
        isMoveNow = false;
      
        PlayerPrefs.SetInt("DestinationPoinNum", destinationNum);
        PlayerPrefs.Save();
        
        beforeDestination = destinationNum;
        if(goHomeMode){ 
            if(destinationNum == setHome) {
                ShowPopup("집에 도착했어요! 저장완료!");
                goHomeMode = false;
            }else {
                SetDestination(-1);
            }
        }    
        
    }
    
    ///////////// For Animation
    void MoveAnimation(){
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
    
    //////////// For Popup To Return Home
    void AskGoHomePopup(){
        askText.text = "집으로 돌아갈까요?";
        DeActivateBuildingBtns();
        nBtn.gameObject.SetActive(true);
        goHomeBtn.gameObject.SetActive(true);
        popup.SetActive(true);

        //재생시킬 오디오 파일 선택
        // audioSource.clip = audioClips[0];
        // audioSource.Play(); //오디오 파일 재생
    }
    void SetHomeRoute(){
        popup.SetActive(false);
        goHomeMode = true;

        dbManager = new DatabaseManager();
        dbManager.Connect();

       
        List<HomeRoute> homeRoute = dbManager.getGoHomeRoute(destinationNum + 1);
        homeRouteList = homeRoute.Select(hr => hr.next_building-1).ToList();
//        Debug.Log(homeRouteList.Count+" Route"+BuildingName(destinationNum) +" : "+ string.Join(", ", homeRouteList));
        
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
    void ShowPopup(string showMessage){
        askText.text = showMessage;

        nBtn.gameObject.SetActive(false);
        goHomeBtn.gameObject.SetActive(false);

        DeActivateBuildingBtns(); 
        
        popup.SetActive(true);
        StartCoroutine(PopupCloseDelay(popupDelay));  
    }
    IEnumerator PopupCloseDelay(float closeTime){
        yield return new WaitForSeconds(closeTime);
        popup.SetActive(false);
        ActivateBuildingBtns();
    }
    
    /////////For Move Scenes
    private void OnTriggerEnter2D(Collider2D other) { 
        if (other.gameObject.tag == "BuildingPoint" && videoPlayer != null && !goHomeMode) 
        {
            string buildingName = other.gameObject.name.Split(".")[1];
            switch (buildingName)
            {
                case "School":
                    videoPlayer.clip = videoClips[0];
                    break;
                case "Cafe":
                    videoPlayer.clip = videoClips[1];
                    break;
                case "FireStation":
                    videoPlayer.clip = videoClips[2];
                    break;
                case "Library":
                    videoPlayer.clip = videoClips[3];
                    break;
                case "Home":
                    videoPlayer.clip = videoClips[4];
                    break;
                case "Mart":
                    videoPlayer.clip = videoClips[5];
                    break;
                case "Police":
                    videoPlayer.clip = videoClips[6];
                    break;
                case "Bank":
                    videoPlayer.clip = videoClips[7];
                    break;
                case "Hospital":
                    videoPlayer.clip = videoClips[8];
                    break;
            }    
          

           PlayerPrefs.SetInt(buildingName + "VisitCount", PlayerPrefs.GetInt(buildingName + "VisitCount") + 1);
     
        videoPlayer.Play();
        }
    }
    void EndReached(VideoPlayer vp)
    {
        PlayerPrefs.SetString("BuildingName", vp.clip.name.Split("_")[0]);
        PlayerPrefs.Save();
        SceneManager.LoadScene("InformationScene");
    }
    void OnApplicationQuit() {
        PlayerPrefs.DeleteAll();
    }

    ////////////For Me
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
    void ForCheckHomeRoute(){ //루트경로 체크하고 싶을 때 start에 넣으세용
        for (int i = 0; i < buildingPoints.Length; i++){
            dbManager = new DatabaseManager();
            dbManager.Connect();

            List<HomeRoute> homeRoute = dbManager.getGoHomeRoute(i+1);
            
            // homeRouteList는 int 타입을 저장하는 리스트로 초기화
            List<int> homeRouteList = homeRoute.Select(hr => hr.next_building).ToList();
            
            // List<int>를 string.Join으로 출력
            Debug.Log(homeRouteList.Count+" Route"+BuildingName(i) +" : "+ string.Join(", ", homeRouteList));
        }
    }
}
