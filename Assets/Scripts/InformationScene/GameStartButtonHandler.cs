using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
        int BuildingVisitCount = PlayerPrefs.GetInt(buildingName + "VisitCount");

        List<string> scenes = new List<string>();

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            scenes.Add(sceneName);
        }

        List<string> gameScenes = scenes.Where(scene => scene.Contains(buildingName) && scene.Contains("GameScene")).ToList();

        int index = BuildingVisitCount - 1;

        if (gameScenes.Count > 1)
        {
            if (BuildingVisitCount > gameScenes.Count) index = (BuildingVisitCount - 1) % gameScenes.Count;
        }
        else index = 0;
        
        SceneManager.LoadScene((gameScenes.Count > 0 && Application.CanStreamedLevelBeLoaded(gameScenes[index])) ? gameScenes[index] : "MapScene");
    }

    public Image startButtonImg;
    public Sprite pressedSprite, defaultSprite;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (startButtonImg != null && pressedSprite != null) startButtonImg.sprite = pressedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (startButtonImg != null && defaultSprite != null) startButtonImg.sprite = defaultSprite;
    }
}
