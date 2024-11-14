using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Text.RegularExpressions;
using System.Threading;

public class SmartPhone : MonoBehaviour
{
    public Button[] numberBtns;
    public Button backspaceBtn;
    public TextMeshProUGUI phoneNumText;

    private DatabaseManager dbManager;

    // Start is called before the first frame update
    void Start()
    {
        dbManager = new DatabaseManager();
        dbManager.Connect();
        string parentPhoneNumber = dbManager.getParentPhoneNumber(1);

        foreach (Button btn in numberBtns) 
        {
            btn.onClick.AddListener(() => {
                if (phoneNumText.text.Length != 13) phoneNumText.text += (phoneNumText.text.Length == 3 || phoneNumText.text.Length == 8) ? $"-{btn.name}" : btn.name;
                
                string inputted = Regex.Replace(phoneNumText.text, @"\D", ""); // 숫자만 남기고 다 제거
                parentPhoneNumber = Regex.Replace(parentPhoneNumber, @"\D", ""); // 숫자만 남기고 다 제거

                if (inputted == parentPhoneNumber) 
                {
                    //gameObject.SetActive(false);
                    foreach (Button button in numberBtns) 
                    {
                        button.onClick.RemoveAllListeners();
                        backspaceBtn.onClick.RemoveAllListeners();

                        gameObject.SetActive(false);
                    }
                }
            });
        }

        backspaceBtn.onClick.AddListener(() => {
            if (phoneNumText.text.Length > 0) phoneNumText.text = phoneNumText.text.Remove(phoneNumText.text.Length - ((phoneNumText.text.Length == 10 || phoneNumText.text.Length == 5) ? 2 : 1));
        });
    }
}
