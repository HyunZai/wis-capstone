using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public Button continueButton;
    public Button newStartButton;

    public ProgressbarHandler progressbarHandler;
    public InputFormHandler inputFormHandler;

    public TextMeshProUGUI title;
    void Start()
    {
        continueButton.onClick.AddListener(ContinueButtonClick);
        newStartButton.onClick.AddListener(NewStartButtonClick);
    }

    void ContinueButtonClick() 
    {
        HideButtons();
        if (progressbarHandler != null) progressbarHandler.StartLoading();
    }

    void NewStartButtonClick() 
    {
        HideButtons();
        title.gameObject.SetActive(false); // 게임 타이틀 숨기기
        if (inputFormHandler != null) inputFormHandler.ShowInputForm();
    }

    private void HideButtons()
    {
        continueButton.gameObject.SetActive(false);
        newStartButton.gameObject.SetActive(false);
    }
}
