using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class RoadPainter : MonoBehaviour
{   public GameObject buildingButtons, player;
    private int basePosNum;
    private Vector2 pp, beforePP, isMoved;
    private float moveSpeed = 10.0f;
    private GameObject[] crossPoints ,destinationPoints ,buildingPoints;
    private Button[] setDestinationBtns;// 빌딩목록 넣기
    private int destinationNum ,beforeDestinationNum, routeCheckCount; 
    private Vector2 [] cp, dp, bp;
    private List<int> homeRouteList;
    private bool isMoveNow;
    private bool goHomeMode;
    private DatabaseManager dbManager;
    private User user;

    ///////////Codes
    void Awake(){
        
        SetBeforeStart();
    }
    void Start(){
        Time.timeScale = 1f;
        
    }
    void Update(){
        Player();
    }
    private void SetBeforeStart(){ 
        StopAllCoroutines();
        basePosNum =  PlayerPrefs.GetInt("DestinationPoinNum", 4); 

        LoadDBManager();
        FindGameObjectInScene();
        AddButtonListener();
        }
    //beforeStart
    private void FindGameObjectInScene(){
        //points
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DestinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("BuildingPoint").OrderBy(building =>  building.name).ToArray();

        //set popup
        
        //buttons
        setDestinationBtns = buildingButtons.GetComponentsInChildren<Button>().OrderBy(btn => btn.name).ToArray();

        //for utility
        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[destinationPoints.Length];
        bp = new Vector2[buildingPoints.Length]; 
    }
    private void Player(){
        pp = this.transform.position; 
    }
    private void AddButtonListener(){

        for(int i = 0; i < destinationPoints.Length; i++){        
            
            if(i < crossPoints.Length) cp[i] = crossPoints[i].transform.position;
            
            dp[i] = destinationPoints[i].transform.position;
            bp[i] = buildingPoints[i].transform.position;
            int index = i;
            setDestinationBtns[index].onClick.AddListener(() => SetDestination(index));     
        }
    }
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
                if(gotoHere == basePosNum){
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
                        
                    }
                }
            }  
        }else if(isMoveNow == true){
            
        }
    }
    private IEnumerator GoSetDestination(){ 
        isMoveNow = true;
        beforePP = pp;
        
        PlayerPrefs.SetInt("DestinationPoinNum", destinationNum);
        PlayerPrefs.Save();

        if(beforeDestinationNum != destinationNum || beforeDestinationNum == basePosNum || goHomeMode){
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

        
    }
    
    private void SetHomeRoute(){
        dbManager = new DatabaseManager();
        dbManager.Connect();
       
        List<HomeRoute> homeRoute = dbManager.getGoHomeRoute(destinationNum + 1);
        homeRouteList = homeRoute.Select(hr => hr.next_building-1).ToList();
    }
    private void LoadDBManager(){
        dbManager = new DatabaseManager();
        dbManager.Connect();
        user = dbManager.login();
    }
}
