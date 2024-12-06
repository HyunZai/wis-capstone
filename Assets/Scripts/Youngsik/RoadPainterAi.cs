using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class RoadPainterAi : MonoBehaviour
{   public GameObject player;
    private Vector2 pp, isMoved;
    private float moveSpeed = 100.0f;
    private GameObject[] crossPoints ,destinationPoints ,buildingPoints;
    public int home;
    public Button[] setDestinationBtns;// 빌딩목록 넣기
    public int startPointNum, endPointNum , routeCheckCount; 
    private Vector2 [] cp, dp, bp;
    public List<int> homeRouteList;
    public bool isMoveNow , goHomeMode;
    private DatabaseManager dbManager;
    private LogManager logManager;

    ///////////Codes
    void Awake(){
        FindGameObjectInScene();
    }
    void Start(){
        Time.timeScale = 1f;    
        AddButtonListener();
    }

    void Update(){
       pp = this.transform.position; 
       goHomeMode = player.GetComponent<CharacterController>().goHomeMode;
       
    }   

    private void FindGameObjectInScene(){
        //points
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DestinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("BuildingPoint").OrderBy(building =>  building.name).ToArray();
        home = player.GetComponent<CharacterController>().setHome;

        startPointNum = PlayerPrefs.GetInt("DestinationPointNum",home);
        
        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[destinationPoints.Length];
        bp = new Vector2[buildingPoints.Length]; 

        for(int i = 0; i < destinationPoints.Length; i++){        
            if(i < crossPoints.Length) cp[i] = crossPoints[i].transform.position;
            dp[i] = destinationPoints[i].transform.position;
            bp[i] = buildingPoints[i].transform.position;
        }
        transform.position = (bp[startPointNum]+ dp[startPointNum])/2;
    }
    private void AddButtonListener(){
        for(int i = 0; i < destinationPoints.Length; i++){  
            int index = i;
            setDestinationBtns[index].onClick.AddListener(() => SetDestination(index));
            Debug.Log("isWork");
        }
    }
    private(int numB, int numP) SetCrossPoint(){
        int numB = int.MaxValue;
        float minB= float.MaxValue;

        for(int i =0; i < crossPoints.Length; i++){
            if(Mathf.Abs(dp[endPointNum].x - cp[i].x) < 0.01f 
            || Mathf.Abs(dp[endPointNum].y-cp[i].y) < 0.01f){

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
                if(gotoHere != home){
                    endPointNum = gotoHere;
                    StartCoroutine(GoSetDestination());
                }
            }
            else if(goHomeMode){     
                GetGoHomeRoute();
                if(gotoHere == homeRouteList[routeCheckCount]){
                    Debug.Log("isWorking");
                    endPointNum = gotoHere;
                    StartCoroutine(GoSetDestination());
                }
                
            }  
        }
    }
    private IEnumerator GoSetDestination(){ 
        transform.position = player.transform.position;
        
        if(endPointNum != home ||startPointNum == home || goHomeMode){
            while(Vector2.Distance(pp,dp[startPointNum]) >= 0.01f){
                this.transform.position = Vector2.MoveTowards(pp,dp[startPointNum],moveSpeed* Time.deltaTime);
                yield return null;
            }

            var (numB, numP) = SetCrossPoint();

            if( !( (Mathf.Abs(pp.x - dp[endPointNum].x) < 0.01f) || (Mathf.Abs(pp.y-dp[endPointNum].y) < 0.01f) )){
                while(Vector2.Distance(pp,cp[numP]) > 0.01f){
                    this.transform.position = Vector2.MoveTowards(pp,cp[numP],moveSpeed* Time.deltaTime);
                    yield return null;
                }
                while(Vector2.Distance(pp,cp[numB]) > 0.01f){
                    this.transform.position = Vector2.MoveTowards(pp,cp[numB],moveSpeed* Time.deltaTime);
                    if(Vector2.Distance(pp,dp[endPointNum])<0.01f)yield break;
                    yield return null;
                }
                
            }

            while(Vector2.Distance(pp,dp[endPointNum]) > 0.01f){
                this.transform.position = Vector2.MoveTowards(pp,dp[endPointNum],moveSpeed* Time.deltaTime);
                yield return null;
            }
        }

        while(Vector2.Distance(pp,bp[endPointNum]) >= 0.01f){
            this.transform.position = Vector2.MoveTowards(pp,bp[endPointNum],moveSpeed* Time.deltaTime);
            yield return null;
        }
        this.transform.position = bp[endPointNum];
        
        startPointNum = endPointNum;
        
    }
    private void GetGoHomeRoute(){
        dbManager = new DatabaseManager();
        dbManager.Connect();

        List<HomeRoute> homeRoute = dbManager.getGoHomeRoute(startPointNum + 1);

        // 12/03 김현재
        //소방서, 병원에서 집으로 돌아갈 때 DB에서 Select해오는 데이터의 순서가 바뀌어 마트를 거치지 않고 바로 집으로 이동하는 이슈 발생 -> 아래 코드를 임시방편으로 해결함(sql쿼리를 수정해서 근본적인 원인을 해결해야됨)
        if (homeRoute[0].building_id != startPointNum + 1 && homeRoute.Count == 2) {
            HomeRoute temp = homeRoute[0];
            homeRoute[0] = homeRoute[1];
            homeRoute[1] = temp;
        }

        //foreach(HomeRoute r in homeRoute) logManager.Log($"귀가 경로 : {r.building_id} -> {r.next_building}", "", LogType.Log);
        
        homeRouteList = homeRoute.Select(hr => hr.next_building-1).ToList();
    }
   
}
