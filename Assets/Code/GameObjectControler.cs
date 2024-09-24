using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectControler : MonoBehaviour
{
    // panel
    Vector2 basePanelPos = new Vector2(0, -7.5f);
    Vector2 playtPanelPos = new Vector2(0, -3.5f);
    
    // truck Pos
    Vector2 baseTruckPos = new Vector2(15, 0);
    Vector2 playTruckPos = new Vector2(0, 0.5f);
    Vector2 endTruckPos = new Vector2(-15, 0);

    Vector2 playImagePanelPos = new Vector2(-6f, 3f);

    public Button[] clickBtns = new Button[3];     
    public GameObject truck;
    public GameObject itemPanel;
    public GameObject showImagePanel;

    float moveSpeed = 3.0f;
    public Button startBtn;
    public SetImageChecker ImageChecker;
    public PopupControler popupControler;

    private bool isGameStarted = false; // 게임 시작 상태 체크
    private float moveTime = 0.0f; // 이동 시간 체크

    void Start()
    {
        StartSetting();
    }

    void StartSetting()
    {
        startBtn.gameObject.SetActive(true);
        for (int i = 0; i < clickBtns.Length; i++) {
            clickBtns[i].gameObject.SetActive(false);  
        }
        startBtn.onClick.AddListener(GameStart);
    }

    void GameStart()
    {
        startBtn.gameObject.SetActive(false);
        isGameStarted = true; // 게임 시작 상태 설정
        moveTime = 0.0f; // 이동 시간 초기화
    }

    void Update()
    {
        if (isGameStarted)
        {
            moveTime += Time.deltaTime; // 경과 시간 증가
            float t = moveTime * moveSpeed* Time.deltaTime;

            showImagePanel.transform.position = Vector2.Lerp(showImagePanel.transform.position, playImagePanelPos, t);
            truck.transform.position = Vector2.Lerp(truck.transform.position, playTruckPos, t);
            itemPanel.transform.position = Vector2.Lerp(itemPanel.transform.position, playtPanelPos, t);

            if (Vector2.Distance(showImagePanel.transform.position, playImagePanelPos) < 0.1f &&
                Vector2.Distance(truck.transform.position, playTruckPos) < 0.1f &&
                Vector2.Distance(itemPanel.transform.position, playtPanelPos) < 0.1f)
            {
                isGameStarted = false; // 게임 시작 상태 종료
            }
        }
    }
}
