using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
using TMPro;
=======
using System.Threading;
using TMPro;
using Unity.VisualScripting;
>>>>>>> origin/dev
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class InputFormHandler : MonoBehaviour
{
    public GameObject inputForm;
    public Button saveAndStartButton;

    public TextMeshProUGUI title;

    public ProgressbarHandler progressbarHandler;
    
    public ToggleGroup genderToggleGroup;

<<<<<<< HEAD
=======
    private DatabaseManager dbManager;

>>>>>>> origin/dev
    // Start is called before the first frame update
    void Start()
    {
        inputForm.SetActive(false);
        saveAndStartButton.onClick.AddListener(SaveAndStartButtonClick);
<<<<<<< HEAD
=======

        dbManager = new DatabaseManager();
        dbManager.Connect();
>>>>>>> origin/dev
    }

    public void ShowInputForm()
    {
        inputForm.SetActive(true);
    }

    void SaveAndStartButtonClick()
    {
        GameObject[] textboxs = GameObject.FindGameObjectsWithTag("Text box");

<<<<<<< HEAD
        string name = null;
        string addr = null;
        string phoneNum = null;
        int gender = GetSelectedGender();
=======
        User user = new User();

        user.registered = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        user.gender = GetSelectedGender();
>>>>>>> origin/dev

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
<<<<<<< HEAD
                case "Name": name = tb.text; break;
                case "Address": addr = tb.text; break;
                case "PhoneNumber": phoneNum = tb.text; break;
            }            
        }

        if (!isAnyEmpty || gender == -1)
        {
            Debug.Log("잘못된 정보가 존재합니다. 다시 입력하세요!");
        }
=======
                case "Name": user.name = tb.text; break;
                case "Address": user.address = tb.text; break;
                case "PhoneNumber": user.parent_phone = tb.text; break;
            }
        }

        User isOk = dbManager.register(user);
        dbManager.Disconnect();

        if (!isAnyEmpty || user.gender == -1)
        {
            Debug.Log("잘못된 정보가 존재합니다. 다시 입력하세요!");
        }
        else if (isOk == null)
        {
            Debug.Log("등록에 실패했습니다. 다시 시도하십시오.");
        }
>>>>>>> origin/dev
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
            //return selectedToggle.GetComponentInChildren<Text>().text;  // Toggle의 텍스트를 가져옴
        }

        return -1;  // 선택된 값이 없을 경우
    }
}
