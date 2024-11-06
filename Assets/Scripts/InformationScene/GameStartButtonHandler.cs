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

        List<string> gameScenes = new List<string>();

        DirectoryInfo scenesDirPath = new DirectoryInfo(Application.dataPath + "/Scenes");
        List<FileInfo> scenesFiles = scenesDirPath.GetFiles().ToList().FindAll(scene => !scene.Name.Contains(".meta") && scene.Name.Contains("GameScene") && scene.Name.Contains(buildingName));
    
        foreach(FileInfo file in scenesFiles) gameScenes.Add(file.Name.Split(".")[0]);

        int index = BuildingVisitCount - 1;

        if (gameScenes.Count > 1)
        {
            if (BuildingVisitCount > gameScenes.Count)
            {
                index = BuildingVisitCount % gameScenes.Count - 1;
            }
        }
        else
        {
            index = 0;
        }
        
        if (gameScenes.Count > 0 && Application.CanStreamedLevelBeLoaded(gameScenes[index]))
        {
            SceneManager.LoadScene(gameScenes[index]);
        }
        else
        {
            SceneManager.LoadScene("MapScene");
        }
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
