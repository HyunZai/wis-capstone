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
        SceneManager.LoadScene("MapScene");
    }
}