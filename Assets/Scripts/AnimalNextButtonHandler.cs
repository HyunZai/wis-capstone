using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalNextButtonHandler : MonoBehaviour
{
    public Button nextButton;
    private AnimalRandomizer animalRandomizer;
    // Start is called before the first frame update
    void Start()
    {
        animalRandomizer = FindObjectOfType<AnimalRandomizer>();
        
        nextButton.onClick.AddListener(NextButtonClick);
    }

    void NextButtonClick()
    {
        if (animalRandomizer != null) animalRandomizer.SetRandomAnimalName();

        ClearAllLines();
    }

    void ClearAllLines()
    {
        LineRenderer[] lines = FindObjectsOfType<LineRenderer>();
        foreach (LineRenderer line in lines)
        {
            line.positionCount = 0;
        }
    }
}
