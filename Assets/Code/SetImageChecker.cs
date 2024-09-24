using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SetImageChecker : MonoBehaviour
{
    public GameObject answer; //정답 오브젝트
    public GameObject[] imageList; // 오브젝트 목록
    public GameObject[] showingImages = new GameObject[3]; //결정된 오브젝트 3개
    public GameObject[] targets = new GameObject[3]; // 결정오브젝트 이동위치
    public bool success;
    public GameObjectControler goc;

    public int[] randN = new int[3];
    void Start()
    {
        StartSetting();
    }

    void StartSetting()
    {
        goc.startBtn.onClick.AddListener(() => ActiveclickBtns()); //메인화면 시작 버튼 클릭시 호출
        ChangeImage();
    }

   void ActiveclickBtns()
{
    for (int i = 0; i < showingImages.Length; i++)
    {
        goc.clickBtns[i].gameObject.SetActive(true);
        int index = i;  // 지역 변수로 저장
        goc.clickBtns[index].onClick.AddListener(() => CheckMatchImage(index));
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
        if (targets[i].GetComponent<SpriteRenderer>().sprite.name == answer.GetComponent<SpriteRenderer>().sprite.name)
        {
            success = true;
            goc.popupControler.PopupEvent(goc.popupControler.popupPanel[0]);
        }
        else 
        {
            success = false; 
            goc.popupControler.PopupEvent(goc.popupControler.popupPanel[1]);
        }
        
    }
}

