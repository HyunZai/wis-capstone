using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Mono.Data.Sqlite;


//using MySql.Data.MySqlClient;
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

    private DbConnection dbConnection;
    //private DatabaseManager dbManager;
    void Start()
    {
        continueButton.onClick.AddListener(ContinueButtonClick);
        newStartButton.onClick.AddListener(NewStartButtonClick);

        //dbManager = new DatabaseManager();
        //dbManager.Connect();
        string connectionString = "URI=file:" + Application.streamingAssetsPath + "/user.db";
        dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();
    }

    void ContinueButtonClick() 
    {
        HideButtons();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT * FROM user WHERE user_id = (SELECT COUNT(user_id) FROM user)";
        IDataReader dataReader = dbCommand.ExecuteReader();
        User user = new User();
        while (dataReader.Read()) 
        {
            user.name = dataReader.GetString(1);
            user.age = dataReader.GetInt32(2);
            user.parent_phone = dataReader.GetString(3);
            user.address = dataReader.GetString(4);
            user.gender = dataReader.GetInt32(5);
            user.registered = dataReader.GetString(6);
        }
        // User user = dbManager.login();
        // dbManager.Disconnect();
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
