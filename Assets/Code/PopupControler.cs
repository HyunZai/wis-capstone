using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupControler : MonoBehaviour
{
    public GameObject[] popupPanel = new GameObject[2];// 1 success 2fail
    public GameObjectControler goc;

    void Start()
    {
        SetPopup();     
    }
 
    public void SetPopup(){
        //disable popup
        for(int i= 0; i< popupPanel.Length; i++){
            popupPanel[i].gameObject.SetActive(false);
        }
    }
    public void PopupEvent(GameObject panel){
        panel.gameObject.SetActive(true);
        if(goc.ImageChecker.success == true)
        StartCoroutine(ClosePopupAfterDelay(3, panel));
    }
    private IEnumerator ClosePopupAfterDelay(float delay, GameObject panel)
    {
        yield return new WaitForSeconds(delay);  // delay만큼 기다림
        panel.SetActive(false);  // 창 닫기
    }

}
