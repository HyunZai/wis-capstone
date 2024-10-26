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

        login("continue");

        if (progressbarHandler != null) progressbarHandler.StartLoading();

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

    void login(string buttonType) {
        dbManager = new DatabaseManager();
        dbManager.Connect();


        User user = null;
        if (buttonType == "continue") 
        {
            string query = "SELECT * FROM USER WHERE user_id = (SELECT COUNT(user_id) FROM USER)";
            user = dbManager.getUser(query);
            Debug.Log("name : " + user.name);
        }
        else 
        {

        }
    }
}
