using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Text.RegularExpressions;
using System.Threading;
using System;

public class SmartPhone : MonoBehaviour
{
    public Button[] numberBtns;
    public Button backspaceBtn, enterBtn, hintBtn;
    public Image hintPopup;
    public TextMeshProUGUI phoneNumText;
    public TextShake textShake;

    private DatabaseManager dbManager;

    // Start is called before the first frame update
    private bool isShow = false;
    private bool isHide = false;
    private bool isComplate = false;
    void Start()
    {
        //DB 연결
        dbManager = new DatabaseManager();
        dbManager.Connect();
        
        //현재 접속한 사용자의 부모님 전화번호 가져옴
        string parentPhoneNumber = dbManager.getParentPhoneNumber(1);

        //힌트버튼 숨김
        hintBtn.gameObject.SetActive(false);

        //DB에 저장된 전화번호는 '-'이 없어 추가해서 hint text에 세팅
        TextMeshProUGUI hintPhoneNumber = hintPopup.GetComponentInChildren<TextMeshProUGUI>();
        string hintParentPhoneNum = parentPhoneNumber.Insert(3, "-").Insert(8, "-");
        hintPhoneNumber.text = hintParentPhoneNum;

        //힌트(부모님 전화번호) 숨김
        hintPopup.gameObject.SetActive(false);

        foreach (Button btn in numberBtns) 
        {
            btn.onClick.AddListener(() => {
                if (phoneNumText.text.Length != 13) phoneNumText.text += (phoneNumText.text.Length == 3 || phoneNumText.text.Length == 8) ? $"-{btn.name}" : btn.name;
            });
        }

        backspaceBtn.onClick.AddListener(() => {
            if (phoneNumText.text.Length > 0) phoneNumText.text = phoneNumText.text.Remove(phoneNumText.text.Length - ((phoneNumText.text.Length == 10 || phoneNumText.text.Length == 5) ? 2 : 1));
        });

        enterBtn.onClick.AddListener(() => {
            string inputted = Regex.Replace(phoneNumText.text, @"\D", ""); // 숫자만 남기고 다 제거
            parentPhoneNumber = Regex.Replace(parentPhoneNumber, @"\D", ""); // 숫자만 남기고 다 제거

            if (inputted == parentPhoneNumber) 
            {
                isComplate = true; //스마트폰 오브젝트 이동할 때 사용
                
                foreach (Button button in numberBtns) 
                {
                    //사용자가 입력한 번호가 일치하면 버튼들 리스너 제거해서 작동 안하도록 처리
                    button.onClick.RemoveAllListeners();
                    backspaceBtn.onClick.RemoveAllListeners();
                }
            }
            else 
            {
                textShake.ShakeText();
                hintBtn.gameObject.SetActive(true); //사용자가 입력한 전화번호가 틀렸을 경우 힌트버튼 가시화
            }
        });

        hintBtn.onClick.AddListener(() => {
            hintPopup.gameObject.SetActive(!hintPopup.isActiveAndEnabled);
        });
    }

    void Update() {
        if (transform.position == new Vector3(12, 0, 0) && isActiveAndEnabled) isShow = true;
        if (isComplate) isHide = true;

        if (isShow) transform.position = Vector3.Lerp(transform.position, new Vector3(5.4f, 0, 0), 7f * Time.deltaTime);
        if (isHide) transform.position = Vector3.MoveTowards(transform.position, new Vector3(12, 0, 0), 50f * Time.deltaTime);
    }
}
