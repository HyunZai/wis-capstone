using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using System.Linq;
using Unity.VisualScripting;
>>>>>>> origin/dev
using UnityEngine;

public class InformationBackground : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;
    public Sprite[] backgrounds;

    // Start is called before the first frame update
    void Start()
    {
<<<<<<< HEAD
        string buildingName = PlayerPrefs.GetString("BuildingName");
=======
        if (backgroundRenderer == null) backgroundRenderer = GetComponent<SpriteRenderer>();

        string buildingName = PlayerPrefs.GetString("BuildingName");
        backgrounds.Where(img => 
            img.name.Contains(buildingName)
            );
>>>>>>> origin/dev
        switch(buildingName)
        {
            case "Cafe":
                ChangeBackground(0);
                break;
            case "Bank":
                ChangeBackground(2);
                break;
            // case "Home":
            //     ChangeBackground(0);
            //     break;
            case "FireStation":
<<<<<<< HEAD
                Debug.Log("[InformationBackground.cs - buildingName: FireStation]");
=======
>>>>>>> origin/dev
                ChangeBackground(3);
                break;
            case "Hospital":
                ChangeBackground(5);
                break;
            case "School":
                ChangeBackground(13);
                break;
            case "Police":
                ChangeBackground(11);
                break;
            case "Library":
                ChangeBackground(7);
                break;
            case "Mart":
                ChangeBackground(9);
                break;
        }
<<<<<<< HEAD

        if (backgroundRenderer == null)
        {
            backgroundRenderer = GetComponent<SpriteRenderer>();
        }    
=======
>>>>>>> origin/dev
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         ChangeBackground(0);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         ChangeBackground(1);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha3))
    //     {
    //         ChangeBackground(2);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha4))
    //     {
    //         ChangeBackground(3);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Alpha5))
    //     {
    //         ChangeBackground(4);
    //     }
    // }

    public void ChangeBackground(int index)
    {
        if (index >= 0 && index < backgrounds.Length)
        {
            backgroundRenderer.sprite = backgrounds[index];
        }
        else
        {
            Debug.LogError("Invalid background index");
        }
    }
}
