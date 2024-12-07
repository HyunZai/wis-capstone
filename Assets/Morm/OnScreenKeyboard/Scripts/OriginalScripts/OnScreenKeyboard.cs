using System;
using System.Collections;
using System.Linq;
using System.Text;
using HangulVirtualKeynoard;
using Microsoft.Win32;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenKeyboard : MonoBehaviour
{
    public enum CurLang {
        KR,
        EN,
    }
    public enum Caps {
        Caps,
        Uncaps
    }

    private OnScreenKeyboardInputfield currentOskInputfield;
    private TMP_InputField targetInputField;
    private string inputtedString, currentString;

    [SerializeField]
    private CurLang curLang;
    private CurLang beforeLang;
    [SerializeField]
    private Caps curCaps;
    private Caps beforeCaps;
    
    [SerializeField]
    private Button bgCloseBtn;
    [SerializeField]
    private Text showTextField;
    [SerializeField]
    private GameObject KrNormal,KrNormalCpas,EnNormal,EnNormalCpas;
    [SerializeField]
    private Button exitBtn, korBtn, engBtn, capsBtn;

    Event fakeEvent;

    void Awake()
    {
        AddListenerToButtons();

        
        curLang = CurLang.KR;
        beforeLang = CurLang.KR;
        curCaps = Caps.Uncaps;
        ClearAllStrValue();
        CloseKeyboard();
    }

    private void Update()
    {
        if (targetInputField == null) return;

        //hide a mobile keyboard
        ForceToCloseMobileKeyboard();

        //Detect Language 
        if (beforeLang != curLang)
        {
            beforeLang = curLang;
            ClearAllStrValue();
            ChangeKeyboardType(curLang, curCaps);
        }

        //Detect Cap 
        if (beforeCaps != curCaps)
        {
            beforeCaps = curCaps;
            ChangeKeyboardType(curLang, curCaps);
        }
    }

    void AddListenerToButtons()
    {
        bgCloseBtn.onClick.AddListener(CloseKeyboard);
        exitBtn.onClick.AddListener(CloseKeyboard);
        korBtn.onClick.AddListener(() => { curLang = CurLang.KR; });
        engBtn.onClick.AddListener(() => { curLang = CurLang.EN; });
        capsBtn.onClick.AddListener(() => {
            switch (curCaps) {
                case Caps.Caps:
                    curCaps = Caps.Uncaps;
                    break;
                case Caps.Uncaps:
                    curCaps = Caps.Caps;
                    break;
            }
        });
    }
    
    public void SendKey(string value) {
        switch (value) {
            case "backspace":
            value = "";
            inputtedString = inputtedString.Length > 1 ? inputtedString.Substring(0, inputtedString.Length - 1) : "";
            if (curLang == CurLang.KR)
                fakeEvent = null;
            else
            {
                currentString = currentString.Length > 1 ? currentString.Substring(0, currentString.Length - 1) : "";
                fakeEvent = Event.KeyboardEvent("backspace");
                fakeEvent.keyCode = KeyCode.Backspace;
            }
            break;
            case "space":
            fakeEvent = Event.KeyboardEvent(value);
            fakeEvent.keyCode = KeyCode.Space;
            InputFieldProcessUpdate(fakeEvent);
            value = " ";
            break;
            case "&":
            fakeEvent = Event.KeyboardEvent("a");
            fakeEvent.keyCode = KeyCode.Ampersand;
            fakeEvent.character = value[0];
            return;
            case "^":
            fakeEvent = Event.KeyboardEvent("a");
            fakeEvent.keyCode = KeyCode.Caret;
            fakeEvent.character = value[0];
            return;
            case "%":
            fakeEvent = Event.KeyboardEvent("a");
            fakeEvent.keyCode = KeyCode.Percent;
            fakeEvent.character = value[0];
            return;
            case "#":
            fakeEvent = Event.KeyboardEvent("a");
            fakeEvent.keyCode = KeyCode.Hash;
            fakeEvent.character = value[0];
            return;
            default:
            if (value.Length != 1) {
                Debug.LogError("Ignoring spurious multi-character key value: " + value);
            }
            fakeEvent = Event.KeyboardEvent(value);
            char keyChar = value[0];
            fakeEvent.character = keyChar;
            if (Char.IsUpper(keyChar)) {
                fakeEvent.modifiers |= EventModifiers.Shift;
            }
            break;
        }

        //부모 전화번호 입력란에는 숫자만 입력되도록 처리
        if (targetInputField.name == "PhoneNumber" && !value.All(char.IsDigit) && fakeEvent.keyCode != KeyCode.Backspace) return;

        InputProcess(value);
    }

    public void ShowKeyboard(TMP_InputField inputField, OnScreenKeyboardInputfield oskInputField) {
        Initialize(inputField, oskInputField);
        InputProcess();
        gameObject.SetActive(true);
    }

    private void CloseKeyboard()
    {
        if (currentOskInputfield != null) 
        {
            currentOskInputfield.FormPositionMove();
        }
        currentOskInputfield?.SaveInputedString(inputtedString);
        gameObject.SetActive(false);
    }
    
    private void InputProcess(string value)
    {
        inputtedString += value;
        currentString += value;

        InputProcess();
    }
    
    private void InputProcess()
    {
        InputFieldProcessUpdate(fakeEvent);

        if (curLang == CurLang.KR)
        {
            StartCoroutine(HangulDelayUpdate());
        }
        else
        {
            targetInputField.text = currentString;
            currentString = GetInputFieldText();
            showTextField.text =  GetInputFieldText();
        }
    }
    
    private void Initialize(TMP_InputField inputField, OnScreenKeyboardInputfield oskInputField)
    {
        targetInputField = inputField;
        currentOskInputfield = oskInputField;
        inputtedString = oskInputField.inputtedString;

        //Set default state
        if (string.IsNullOrEmpty(targetInputField.text))
        {
            //Do check your language
            curCaps = Caps.Uncaps;
            curLang = CurLang.KR;
        }
        
        ChangeKeyboardType(curLang, curCaps);
        ForceToCloseMobileKeyboard();
    }
    
    void ChangeKeyboardType(CurLang curLang, Caps caps) {
        KrNormal.gameObject.SetActive(false);
        KrNormalCpas.gameObject.SetActive(false);
        EnNormal.gameObject.SetActive(false);
        EnNormalCpas.gameObject.SetActive(false);

        this.curLang = curLang;
        this.curCaps = caps;
        
        switch (curLang) {
            case CurLang.KR:
                korBtn.gameObject.SetActive(false);
                engBtn.gameObject.SetActive(true);
                switch (caps) {
                    case Caps.Caps:
                        KrNormalCpas.gameObject.SetActive(true);
                        break;
                    case Caps.Uncaps:
                        KrNormal.gameObject.SetActive(true);
                        break;
                }
                break;
            case CurLang.EN:
                korBtn.gameObject.SetActive(true);
                engBtn.gameObject.SetActive(false);
                switch (caps) {
                    case Caps.Caps:
                        EnNormalCpas.gameObject.SetActive(true);
                        break;
                    case Caps.Uncaps:
                        EnNormal.gameObject.SetActive(true);
                        break;
                }
                break;
        }
    }

    private void SetInputFieldText(string str)
    {
        if (targetInputField)
            targetInputField.text = str;
    }

    private string GetInputFieldText()
    {
        if (targetInputField)
            return targetInputField.text;
        else
            return "";
    }

    private void InputFieldProcessUpdate(Event fakeEvent)
    {
        if (targetInputField)
        {
            if(fakeEvent != null)
                targetInputField.ProcessEvent(fakeEvent);
            targetInputField.ForceLabelUpdate();
        }
    }

    IEnumerator HangulDelayUpdate()
    {
        HangulHelper helper = new HangulHelper();
        StringBuilder stringBuilder = new StringBuilder();

        //한글 완성을 위한 한글조합 넣기
        for (int i = 0; i < inputtedString.Length; i++) {
            helper.Input(stringBuilder, inputtedString[i]); // ㅎ
        }

        SetInputFieldText(stringBuilder.ToString());
        
        yield return null;
        currentString = GetInputFieldText();
        showTextField.text =  GetInputFieldText();
    }
    
    private void ForceToCloseMobileKeyboard()
    {
        if (targetInputField == null) return;

        // if (targetInputField.touchScreenKeyboard != null)
        //     targetInputField.touchScreenKeyboard.active = false;
    }
    
    private void ClearAllStrValue()
    {
        inputtedString = "";
        currentString = "";
        showTextField.text = "";
        
        if(targetInputField)
            targetInputField.text = "";
    }

}
