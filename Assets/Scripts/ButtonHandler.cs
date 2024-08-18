using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartButtonClick);
    }

    void StartButtonClick()
    {
        string buildingName = PlayerPrefs.GetString("BuildingName");
        SceneManager.LoadScene(buildingName + "GameScene");
    }

    public Image startButtonImg;
    public Sprite defaultSprite;
    public Sprite pressedSprite;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (startButtonImg != null && pressedSprite != null) startButtonImg.sprite = pressedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (startButtonImg != null && defaultSprite != null) startButtonImg.sprite = defaultSprite;
    }
}
