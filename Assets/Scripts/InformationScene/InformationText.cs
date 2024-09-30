using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI informationText;

    // Start is called before the first frame update
    void Start()
    {
        string buildingName = PlayerPrefs.GetString("BuildingName");
        switch(buildingName)
        {
            case "Cafe":
                informationText.text = "안녕하세요. 이 곳은 [카페]입니다!\n맛있는 음료와 빵을 먹을 수 있는 곳이에요!";
                break;
            case "Bank":
                informationText.text = "안녕하세요. 이 곳은 [은행]입니다!\n통장에 돈을 저금하기 위해 오는 장소에요!";
                break;
            // case "Home":
            //     ChangeBackground(0);
            //     break;
            case "FireStation":
                informationText.text = "안녕하세요. 이 곳은 [소방서]입니다!\n우리 동네에 불이 났을 때 여기서\n소방관 아저씨들이 출동해요!";
                break;
            case "Hospital":
                informationText.text = "안녕하세요. 이 곳은 [병원]입니다!\n몸이 아플 때 치료를 받으러 오는 곳이에요!";
                break;
            case "School":
                informationText.text = "안녕하세요. 이 곳은 [학교]입니다!\n선생님께 수업을 듣고 공부를 하는 곳이에요!";
                break;
            case "Police":
                informationText.text = "안녕하세요. 이 곳은 [경찰서]입니다!\n범죄를 저지르는 나쁜 사람들을 혼내주는\n경찰 아저씨들이 계시는 곳이에요!";
                break;
            case "Library":
                informationText.text = "안녕하세요. 이 곳은 [도서관]입니다!\n책을 읽거나 빌릴 수 있는 곳이에요!";
                break;
            case "Mart":
                informationText.text = "안녕하세요. 이 곳은 [마트]입니다!\n필요한 물건이나 음식을 구매할\n수 있는 곳이에요!";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
