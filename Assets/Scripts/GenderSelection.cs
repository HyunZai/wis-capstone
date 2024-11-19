using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenderSelection : MonoBehaviour
{
    public Toggle maleToggle;       // 남자 체크박스
    public Toggle femaleToggle;     // 여자 체크박스
    //public Text genderDisplayText;  // 선택된 성별 표시용 Text
    private DatabaseManager dbManager;
    private User user;

    void Start()
    {
        dbManager = new DatabaseManager();
        dbManager.Connect();

        user = new User();

        // 체크박스 변경 시 성별 설정 메서드 호출
        maleToggle.onValueChanged.AddListener(OnMaleToggleChanged);
        femaleToggle.onValueChanged.AddListener(OnFemaleToggleChanged);

        //UpdateGenderDisplay();
    }

    void OnMaleToggleChanged(bool isOn)
    {
        if (isOn)
        {
            user.gender = 0;       // 남자
            femaleToggle.isOn = false; // 여자 체크박스 해제
            //UpdateGenderDisplay();  // 성별 표시 업데이트
        }
    }

    void OnFemaleToggleChanged(bool isOn)
    {
        if (isOn)
        {
            user.gender = 1;       // 여자
            maleToggle.isOn = false;   // 남자 체크박스 해제
            //UpdateGenderDisplay();  // 성별 표시 업데이트
        }
    }

    // // 성별 정보를 텍스트에 업데이트
    // void UpdateGenderDisplay()
    // {
    //     genderDisplayText.text = "선택된 성별: " + (user.gender == 0 ? "남자" : "여자");
    // }

    public void RegisterUser()
    {
        dbManager.register(user);
    }
}
