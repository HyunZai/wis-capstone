
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

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
    public int homeRoute1, homeRoute2;
    public GameObject popup;
    public float popupDelay = 2.0f;
    TextMeshProUGUI askText;
    public bool goHomeMode, isMoveNow = false;
    
    public GameObject buildingBtns;
    
    public int setHome= 4;
    public Button nBtn, goHomeBtn;
    public VideoClip[] videoClips;
    private VideoPlayer videoPlayer;
    string buildingName;

    //시리얼 포트 통신 코드 추가
    public string portName = "COM3"; // 시리얼 포트 이름
    public int baudRate = 115200;      // 시리얼 통신 속도
    private SerialPort serialPort;
    private Thread serialThread;
    private bool isRunning = false;
    private ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

    // 시리얼 포트 통신 테스트용
    private LogManager logManager;


    ///////////Codes
    void Awake(){
        SetBeforeStart();
    }
    void Start()
    {
        //시리얼 포트 통신 코드 추가
        logManager = new LogManager();
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 1000;

        try
        {
            serialPort.Open();
            isRunning = true;
            serialThread = new Thread(ReadSerialData);
            serialThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial port could not be opened: " + e.Message);
        }
    }
    void Update()
    {
        //시리얼 포트 통신 코드 추가
        if (dataQueue.TryDequeue(out string sensorData)) MoveCharacter(sensorData);

        MoveAnimation();
        pp = player.transform.position; 
    }
    ///////////For BeforeStart    
    void SetBeforeStart(){ 
        Time.timeScale = 1f;
        characterList = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name).ToArray();
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DestinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("BuildingPoint").OrderBy(building =>  building.name).ToArray();
        setDistinationBtns = buildingBtns.GetComponentsInChildren<Button>().OrderBy(btn => btn.name).ToArray();
        askText = popup.GetComponentInChildren<TextMeshProUGUI>();
        videoPlayer = FindObjectOfType<VideoPlayer>();

        
        nBtn.onClick.AddListener(() => NoButtonClick());
        goHomeBtn.onClick.AddListener(()=> GoHomeButtonClick());

        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[destinationPoints.Length];
        bp = new Vector2[buildingPoints.Length];

        for(int i = 0; i < destinationPoints.Length; i++){        //건물 클릭 시 반응
            if(i<crossPoints.Length)cp[i] = crossPoints[i].transform.position;
            dp[i] = destinationPoints[i].transform.position;
            bp[i] = buildingPoints[i].transform.position;
            int index = i;
            setDistinationBtns[index].onClick.AddListener(()=>SetDestination(index));     
        }

        int loadPosNum = PlayerPrefs.GetInt("DestinationPoinNum", setHome); // 씬 복원될떄 위치 번호 불러오기 없으면 setHome 위치 
    
        destinationNum = loadPosNum;
        beforeDestination = loadPosNum;
        if(!goHomeMode)player.transform.position = (dp[loadPosNum] + bp[loadPosNum])/2;

        if (anim == null) anim = GetComponent<Animator>();  
        
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
    public void SetDestination(int gotoHere){
        if(isMoveNow == false){
            if(!goHomeMode)
            {
                if(gotoHere == setHome){
                    ShowPopup($"아직은 집으로 갈때가 아니에요!\n좀 더 동네를 탐험해볼까요?");
                }else{
                    destinationNum = gotoHere;
                    StartCoroutine(GoSetDestination());
                }
            }
            else if(goHomeMode){
                if (homeRoute1 == gotoHere && destinationNum != homeRoute2 && destinationNum != homeRoute1 && destinationNum != setHome){
                    destinationNum = gotoHere;
                    StartCoroutine(GoSetDestination());
                }else if(beforeDestination == homeRoute1 && gotoHere == homeRoute2 ){
                    destinationNum = gotoHere;
                    StartCoroutine(GoSetDestination()); 
                }else if(beforeDestination == homeRoute2 && gotoHere == setHome){
                    destinationNum = gotoHere;
                    StartCoroutine(GoSetDestination()); 
                }else if(beforeDestination != homeRoute1 && beforeDestination != homeRoute2 && gotoHere != homeRoute1 && gotoHere!= setHome){
                    ShowPopup($"먼저 {BuildingName(homeRoute1)}으로 이동해주세요!");
                }else if(beforeDestination == homeRoute1 && gotoHere != homeRoute2 ){
                    ShowPopup($"{BuildingName(homeRoute2)}으로 이동해주세요!");
                }else if(beforeDestination == homeRoute2 && gotoHere != setHome){
                    ShowPopup($"{BuildingName(setHome)}으로 이동해주세요!");
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
                player.transform.position = Vector2.MoveTowards(pp,dp[beforeDestination],moveSpeed* Time.deltaTime);
                yield return null;
            }
            beforePP = pp;

            var (numB, numP) = SetCrossPoint();

            if(!(Mathf.Abs(pp.x - dp[destinationNum].x) < 0.01f || Mathf.Abs(pp.y-dp[destinationNum].y)<0.01f)){
                while(Vector2.Distance(pp,cp[numP])>0.01f){
                    player.transform.position = Vector2.MoveTowards(pp,cp[numP],moveSpeed* Time.deltaTime);
                    yield return null;
                }
                beforePP = pp;

                while(Vector2.Distance(pp,cp[numB])>0.01f){
                    player.transform.position = Vector2.MoveTowards(pp,cp[numB],moveSpeed* Time.deltaTime);
                    if(Vector2.Distance(pp,dp[destinationNum])<0.01f)yield break;
                    yield return null;
                }
                beforePP = pp;
            }

            while(Vector2.Distance(pp,dp[destinationNum])>0.01f){
                player.transform.position = Vector2.MoveTowards(pp,dp[destinationNum],moveSpeed* Time.deltaTime);
                yield return null;
            }
            beforePP = pp;
        }
        while(Vector2.Distance(pp,bp[destinationNum])>=0.001f){
            player.transform.position = Vector2.MoveTowards(pp,bp[destinationNum],moveSpeed* Time.deltaTime);
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
    }
    void SetHomeRoute(){
        popup.SetActive(false);
        goHomeMode = true;

        int cpPoint = 0;
        float shortCPNum = float.MaxValue;

        float shortCPNum2 = float.MaxValue;
        float shortCPNum3 = float.MaxValue;

        for(int i = 0; i < cp.Length; i++){
            if(Vector2.Distance(bp[destinationNum],cp[i])< shortCPNum){
                shortCPNum = Vector2.Distance(bp[destinationNum],cp[i]);
                cpPoint = i;
            }
        }
        
        for(int i= 0; i< bp.Length; i++){
            if(destinationNum == i||setHome == i)continue;
            if(Vector2.Distance(cp[cpPoint], dp[i]) < shortCPNum2){
                if(Vector2.Distance(cp[cpPoint], dp[i]) < shortCPNum3){
                    shortCPNum3 = Vector2.Distance(cp[cpPoint], dp[i]);
                    homeRoute1 = i;
                    continue;
                }
                shortCPNum2 = Vector2.Distance(cp[cpPoint], dp[i]);
                homeRoute2 =i;
            }
        }
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
            buildingName = other.gameObject.name.Split(".")[1];
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
        //시리얼 포트 통신 코드 추가
        isRunning = false;
        if (serialPort != null && serialPort.IsOpen) serialPort.Close();
        if (serialThread != null && serialThread.IsAlive) serialThread.Join();


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

    //시리얼 포트 통신 코드 추가
    private void ReadSerialData()
    {
        while (isRunning)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    string sensorData = serialPort.ReadLine();
                    dataQueue.Enqueue(sensorData.Trim()); // 읽은 데이터를 큐에 추가
                }
                catch (System.TimeoutException)
                {
                    // 데이터가 없으면 무시
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading from serial port: " + e.Message);
                }
            }
        }
    }

    private string previousData;
    void MoveCharacter(string data)
    {
        //logManager.Log($"시리얼 포트로부터 받은 데이터: {data}", "", LogType.Log);
        Debug.Log($"받은 데이터: {data}");
        if (string.IsNullOrEmpty(previousData))
        {
            previousData = data;
        }
        else if (previousData != data)
        {
            SetDestination(data.Trim().ToCharArray()[1] - 1);
            previousData = data;
        }
    }

}
