using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFormHandler : MonoBehaviour
{
    public GameObject inputForm;
    public Button saveAndStartButton;

    public TextMeshProUGUI title;

    public ProgressbarHandler progressbarHandler;
    
    public ToggleGroup genderToggleGroup;

    private DatabaseManager dbManager;

    void Start()
    {
        inputForm.SetActive(false);
        saveAndStartButton.onClick.AddListener(SaveAndStartButtonClick);

        dbManager = new DatabaseManager();
        dbManager.Connect();
    }

    public void ShowInputForm()
    {
        inputForm.SetActive(true);
    }

    void SaveAndStartButtonClick()
    {
        GameObject[] textboxs = GameObject.FindGameObjectsWithTag("Text box");

        User user = new User();

        user.registered = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        user.gender = GetSelectedGender();

        bool isAnyEmpty = true;
        foreach(GameObject obj in textboxs)
        {
            TMP_InputField tb = obj.GetComponent<TMP_InputField>();

            if (string.IsNullOrEmpty(tb.text)) 
            {
                isAnyEmpty = false;
                break;
            }

            switch(obj.name) 
            {
                case "Name": user.name = tb.text; break;
                case "Address": user.address = tb.text; break;
                case "PhoneNumber": user.parent_phone = tb.text; break;
            }
        }

        bool isOk = dbManager.register(user);

        if (!isAnyEmpty || user.gender == -1)
        {
            Debug.Log("잘못된 정보가 존재합니다. 다시 입력하세요!");
        }
        else
        {
            inputForm.SetActive(false);
            title.gameObject.SetActive(true);
            progressbarHandler.StartLoading();
        }
    }

    int GetSelectedGender()
    {
        // ToggleGroup에서 활성화된 Toggle을 찾기
        Toggle selectedToggle = genderToggleGroup.ActiveToggles().FirstOrDefault();

        if (selectedToggle != null)
        {
            return (selectedToggle.GetComponentInChildren<Text>().text == "남자") ? 0 : 1;
        }

        return -1;  // 선택된 값이 없을 경우
    }
}
