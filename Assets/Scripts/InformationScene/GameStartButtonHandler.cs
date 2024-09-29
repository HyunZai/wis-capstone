using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            string sceneName = Path.GetFileNameWithoutExtension(scene.path);
            if (sceneName.Contains("GameScene") && sceneName.Contains(buildingName)) gameScenes.Add(sceneName);
        }

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
        
        if(Application.CanStreamedLevelBeLoaded(gameScenes[index])) //해당 게임 씬 존재하는지
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
