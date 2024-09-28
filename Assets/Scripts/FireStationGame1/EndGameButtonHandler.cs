using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameButtonHandler : MonoBehaviour
{
    public Button endGameButton;
    public void Start()
    {
        endGameButton.onClick.AddListener(EndGameButtonClick);
    }

    void EndGameButtonClick() 
    {
        SceneManager.LoadScene("MapScene");
    }
}
