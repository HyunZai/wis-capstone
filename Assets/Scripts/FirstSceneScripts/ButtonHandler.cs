using System;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public Button continueButton;
    public Button newStartButton;

    public ProgressbarHandler progressbarHandler;
    public InputFormHandler inputFormHandler;

    public TextMeshProUGUI title;

    private DatabaseManager dbManager;
    void Start()
    {
        continueButton.onClick.AddListener(ContinueButtonClick);
        newStartButton.onClick.AddListener(NewStartButtonClick);
    }

    void ContinueButtonClick() 
    {
        HideButtons();

        User user = dbManager.login();

        if (user.name != null)
        {
            if (progressbarHandler != null) progressbarHandler.StartLoading();
        }
        else 
        {
            //DB에 등록되어 있는 사용자 정보가 없는 경우
        }        
    }

    void NewStartButtonClick() 
    {
        HideButtons();
        title.gameObject.SetActive(false); // 게임 타이틀 숨기기
        if (inputFormHandler != null) inputFormHandler.ShowInputForm();
    }

    private void HideButtons()
    {
        continueButton.gameObject.SetActive(false);
        newStartButton.gameObject.SetActive(false);
    }
}
