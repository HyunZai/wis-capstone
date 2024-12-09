using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoadPainterAi : MonoBehaviour
{   public GameObject player;
    private Vector2 pp; //painter Pos
    private float moveSpeed = 50.0f;
    private GameObject[] crossPoints ,destinationPoints ,buildingPoints;
    private int home;
    private int startPointNum, endPointNum , routeCheckCount; 
    private Vector2 [] cp, dp, bp;
    private bool painterAiIsMoveNow , goHomeMode = false;
    
    

    ///////////Codes
    void Awake(){
        FindGameObjectInScene();
        StartCoroutine(StartGoHomeMode());
    }
    void Start(){
        transform.position = player.transform.position;
    }
    void Update(){
        if(!goHomeMode) transform.position = player.transform.position;
       pp = transform.position; 
    }   
    public IEnumerator StartGoHomeMode(){
        routeCheckCount = 0;
        startPointNum = PlayerPrefs.GetInt("DestinationPointNum",home);

        while (true){
            goHomeMode = player.GetComponent<PlayerController>().goHomeMode;
            transform.position = player.transform.position;

            if(goHomeMode){
                transform.position += new Vector3(0.1f , 0f, 0);
                yield break;
            }
            yield return null;
        }
    }
    private void OnTriggerStay2D(Collider2D other) { 
        if (other.gameObject.tag == "Player" && goHomeMode && painterAiIsMoveNow == false && endPointNum != home) {
            painterAiIsMoveNow = true;
            endPointNum = player.GetComponent<PlayerController>().homeRouteList[routeCheckCount];
            StartCoroutine(GoSetDestination());  
        }else if (other.gameObject.tag == "Player" && goHomeMode && painterAiIsMoveNow == false && endPointNum == home){
            StartCoroutine(StartGoHomeMode());
        }
    }
    private void FindGameObjectInScene(){
        crossPoints = GameObject.FindGameObjectsWithTag("CrossPoint").OrderBy(crossingPoint => crossingPoint.name).ToArray();
        destinationPoints = GameObject.FindGameObjectsWithTag("DestinationPoint").OrderBy(distinationPoint => distinationPoint.name).ToArray(); 
        buildingPoints = GameObject.FindGameObjectsWithTag("BuildingPoint").OrderBy(building =>  building.name).ToArray();
        home = player.GetComponent<PlayerController>().setHome;
        
        cp = new Vector2[crossPoints.Length];
        dp = new Vector2[destinationPoints.Length];
        bp = new Vector2[buildingPoints.Length]; 

        for(int i = 0; i < destinationPoints.Length; i++){        
            if(i < crossPoints.Length) cp[i] = crossPoints[i].transform.position;
            dp[i] = destinationPoints[i].transform.position;
            bp[i] = buildingPoints[i].transform.position;
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
    private IEnumerator GoSetDestination(){ 
        if(startPointNum == home || goHomeMode){
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
        painterAiIsMoveNow = false;
        routeCheckCount+=1;
        startPointNum = endPointNum;

        
        
    }  
}
