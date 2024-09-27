using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public Sprite defaultSprite;
    public Sprite clearSprite;

    void NextButtonClick()
    {
        GameObject[] starList = GameObject.FindGameObjectsWithTag("Star"); // Tag Name으로 star object 가져오기
        GameObject star_1 = null;
        GameObject star_2 = null;
        GameObject star_3 = null;

        foreach (GameObject star in starList)
        {
            switch(star.name)
            {
                case "Star_1": star_1 = star; break;
                case "Star_2": star_2 = star; break;
                case "Star_3": star_3 = star; break;
            }
        }

        //Vector3 defaultScale = starList[0].transform.localScale;
        Vector3 newScale = starList[0].transform.localScale; // 채워진 별 아이콘이랑 빈 별 아이콘 크기 차이떄문에 스케일(크기)도 같이 조정
        newScale.x = 0.35f;
        newScale.y = 0.35f;
        if (star_1.GetComponent<SpriteRenderer>().sprite.name == "icons_7") // "icons_7"이 빈 별
        {
            star_1.GetComponent<SpriteRenderer>().sprite = clearSprite;
            star_1.transform.localScale = newScale;

            if (animalRandomizer != null) animalRandomizer.SetRandomAnimalName(false);
        }
        else if (star_2.GetComponent<SpriteRenderer>().sprite.name == "icons_7")
        {
            star_2.GetComponent<SpriteRenderer>().sprite = clearSprite;
            star_2.transform.localScale = newScale;

            if (animalRandomizer != null) animalRandomizer.SetRandomAnimalName(false);
        }
        else if (star_3.GetComponent<SpriteRenderer>().sprite.name == "icons_7")
        {
            star_3.GetComponent<SpriteRenderer>().sprite = clearSprite;
            star_3.transform.localScale = newScale;

            if (animalRandomizer != null) animalRandomizer.SetRandomAnimalName(true);

            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "끝내기";
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }

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
