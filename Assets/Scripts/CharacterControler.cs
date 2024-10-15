using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CC : MonoBehaviour
{   
    public GameObject player;    //메인 캐릭터 지정
    float moveSpeed = 4.0f;
    GameObject[] characterList;    //캐릭터 설정 리스트 ++++ 캐릭터 목록 추가점요
    GameObject[] crossPoints;   //사거리 통과 체크 패널
    GameObject[] distinationPoints;   //건물앞 앞 체크 패널
    Button[] setDistinationBtns;// 빌딩목록 넣기
    public int destinationNum;  //목표빌딩 번호
    public int setHome = 4; // 집
    Vector2 [] cp;
    Vector2 pp;
    Vector2 []dp;
    bool isMove = false;
    public Animator animator;


    void Start(){
        SetBeforeStart();
    }
    void Update(){
        pp = player.transform.position;  
    }
    void SetBeforeStart(){ //이름 순서대로 할당해주고  캐릭터 포지션을 집으로 이동 및 클릭버튼 활성화>> 클릭버튼은  추후 실물판에서 값 받아와서 바꿔주는걸로
        destinationNum = setHome;
        characterList = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name).ToArray();
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        distinationPoints = GameObject.FindGameObjectsWithTag("DistinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        setDistinationBtns = FindObjectsOfType<Button>().OrderBy(btn => btn.name).ToArray();

        player.transform.position = distinationPoints[setHome].transform.position;

        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[distinationPoints.Length];

        for(int i =0; i<distinationPoints.Length; i++){        //건물 클릭 시 반응
            if(i<crossPoints.Length)cp[i] = crossPoints[i].transform.position;
            dp[i] = distinationPoints[i].transform.position;
            int index = i;
            setDistinationBtns[index].onClick.AddListener(()=>SetDestination(index));
        }
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
        var (numB, numP) =SetCrossPoint();
        if(!(Mathf.Abs(pp.x - dp[destinationNum].x) < 0.1f || Mathf.Abs(pp.y-dp[destinationNum].y)<0.1f)){
            while(Vector2.Distance(pp,cp[numP])>0.1f){
                player.transform.position = Vector2.MoveTowards(pp,cp[numP],moveSpeed* Time.deltaTime);
                yield return null;
            }
            while(Vector2.Distance(pp,cp[numB])>0.1f){
                player.transform.position = Vector2.MoveTowards(pp,cp[numB],moveSpeed* Time.deltaTime);
                if(Vector2.Distance(pp,dp[destinationNum])<0.1f)yield break;
                yield return null;
            }
        }
        while(Vector2.Distance(pp,dp[destinationNum])>0.1f){
            player.transform.position = Vector2.MoveTowards(pp,dp[destinationNum],moveSpeed* Time.deltaTime);
            yield return null;
        }
        isMove = false;
    }
    void SetDestination(int destinationNumber){
        if(destinationNum != destinationNumber && isMove == false)
        {
            isMove = true;
            destinationNum = destinationNumber;
            StartCoroutine(GoDestination());
        }
        else if(destinationNum == destinationNumber)
        {
            Debug.Log("***PLEASE CLICK OTHER BUILDING*** :(");
        }
        else if(isMove == true)
        {
            Debug.Log("***PLAYER MOVE TO DESTINATION*** \n ***PLEASE WAIT***");
        }
    }
    void MoveAnimation(Vector2 beforePP){
        while(isMove){
            Vector2 isMoved = pp - beforePP;
            Debug.Log(isMoved);
            animator.SetFloat("MoveX", isMoved.x);
            animator.SetFloat("MoveY", isMoved.y);
            // 애니메이션 추가만 하면 끝난다잇
        }
    }
    void MoveDebuger(int i, int num){
        Debug.Log(i + "\n" +
            $"cp[i] 위치: (x: {cp[i].x}, y: {cp[i].y})\n" +
            $"cp[numB] 위치: (x: {cp[num].x}, y: {cp[num].y})\n" +
            $"플레이어 위치: (x: {pp.x}, y: {pp.y})\n" +
            $"Condition1 (x 동일?): {Mathf.Abs(cp[i].x - cp[num].x)}\n" +
            $"Condition2 (y 동일?): {Mathf.Abs(cp[i].y - cp[num].y)}\n" +
            $"Condition3 (플레이어 x 근접?): {Mathf.Abs(pp.x - cp[i].x)}\n" +
            $"Condition4 (플레이어 y 근접?): {Mathf.Abs(pp.y - cp[i].y)}"
        );
    }
  
}

