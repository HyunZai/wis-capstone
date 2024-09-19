using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MatchImage : MonoBehaviour
{
    public Button[] Clickbtns = new Button[3]; // 버튼 클릭 
    public GameObject answer; //정답 오브젝트
    public GameObject[] imageList; // 오브젝트 목록
    public GameObject[] showingImages = new GameObject[3]; //결정된 오브젝트 3개
    public GameObject[] targets = new GameObject[3]; // 결정오브젝트 이동위치
    public bool currect;
    public GameObjectControl gameObjectControl;

    public int[] randN = new int[3];
    void Start()
    {
        for (int i = 0; i < showingImages.Length; i++)
        {
            Clickbtns[i].interactable = false;
        }
        StartBtnSetting();
    }

    void Update()
    {
    }

    void StartBtnSetting()
    {
        gameObjectControl.startBtn.onClick.AddListener(() => ActiveClickBtns());
        for (int i = 0; i < showingImages.Length; i++)
        {
            Clickbtns[i].interactable = false;
        }
        ChangeImage();
    }

    void ActiveClickBtns()
    {
        for (int i = 0; i < showingImages.Length; i++)
        {
            Clickbtns[i].interactable = true;
            Clickbtns[i].onClick.AddListener(() => CheckMatchImage(i));
        }
    }

    void ChangeImage()
    {

        for (int i = 0; i < randN.Length; i++)
        {
            randN[i] = Random.Range(0, imageList.Length);
            for (int j = 0; j < i; j++) // j는 i보다 작아야 중복 검사 가능
            {
                if (randN[i] == randN[j])
                {
                    i--; // 중복 발생 시 다시 뽑기
                    break;
                }
            }
        }

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].gameObject.GetComponent<SpriteRenderer>().sprite = imageList[randN[i]].gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        SetAnswer();
    }

    void SetAnswer()
    {
        int randAnswer = Random.Range(0, showingImages.Length);
        answer.GetComponent<SpriteRenderer>().sprite = targets[randAnswer].GetComponent<SpriteRenderer>().sprite;
    }

    void CheckMatchImage(int i)
    {
        if (showingImages[i].GetComponent<SpriteRenderer>().sprite == answer.GetComponent<SpriteRenderer>().sprite)
        {
            currect = true;
        }
        else
        {
            currect = false;
        }
    }

    void moveImagePos()
    {
        for (int i = 0; i < 3; i++)
        {
            showingImages[i].transform.position = targets[i].gameObject.transform.position;
        }
    }
}
