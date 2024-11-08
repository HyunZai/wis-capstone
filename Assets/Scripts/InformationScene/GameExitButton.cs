using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameExitButton : MonoBehaviour
{
    public Button exitGameButton;
    public void Start()
    {
        exitGameButton.onClick.AddListener(ExitGameButtonClick);
    }

    void ExitGameButtonClick() 
    {
        //들어오면서 카운팅 된 VisitCount - 1
        string buildingName = PlayerPrefs.GetString("BuildingName");
        PlayerPrefs.SetInt(buildingName + "VisitCount", PlayerPrefs.GetInt(buildingName + "VisitCount") - 1);

        SceneManager.LoadScene("MapScene");
    }
}