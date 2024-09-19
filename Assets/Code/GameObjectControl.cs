using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectControl : MonoBehaviour
{
    // panel
    Vector2 basePanelPos = new Vector2(0,-7.5f);
    Vector2 playtPanelPos = new Vector2(0,-3.5f);
    
    //truck Pos
    Vector2 baseTruckPos = new Vector2(15,0);
    Vector2 playTruckPos = new Vector2(0,0.5f);
    Vector2 endTruckPos = new Vector2(-15,0);

    Vector2 baseImagePanelPos = new Vector2(0.38f,3f);
    Vector2 playImagePanelPos = new Vector2(0.38f,2.9f);

    


    public float moveSpeed = 3.0f;

    public GameObject truck;
    public GameObject itemPanel;
    public GameObject showImagePanel;
    public Button startBtn;
    bool startGame = false;


    
    // Start is called before the first frame update
    void Start()
    {
        startBtn.interactable = true;
        startBtn.onClick.AddListener(GameStart);
    }

    // Update is called once per frame
    void Update()
    {
        if(startGame == true){
            MoveTruckPos();
            ShowItemWindow();
        }
    }
    void GameStart () {
        startGame = true;
        startBtn.gameObject.SetActive(false);
    }
    void ShowImagePenal(){
         showImagePanel.gameObject.transform.position =  Vector2.Lerp(showImagePanel.gameObject.transform.position , playImagePanelPos, moveSpeed * Time.deltaTime);
   

    }
    void MoveTruckPos()
    {
        truck.gameObject.transform.position =  Vector2.Lerp(truck.gameObject.transform.position , playTruckPos, moveSpeed * Time.deltaTime);
    }
    
    void ShowItemWindow()
    {
        itemPanel.gameObject.transform.position =  Vector2.Lerp(itemPanel.gameObject.transform.position , playtPanelPos, moveSpeed * Time.deltaTime);
 
    }

}
