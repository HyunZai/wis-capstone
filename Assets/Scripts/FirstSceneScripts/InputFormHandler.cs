using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class InputFormHandler : MonoBehaviour
{
    public GameObject inputForm;
    public Button saveAndStartButton;

    public TextMeshProUGUI title;

    public ProgressbarHandler progressbarHandler;
    

    // Start is called before the first frame update
    void Start()
    {
        inputForm.SetActive(false);
        saveAndStartButton.onClick.AddListener(SaveAndStartButtonClick);
    }

    public void ShowInputForm()
    {
        inputForm.SetActive(true);
    }

    void SaveAndStartButtonClick()
    {
        inputForm.SetActive(false);
        title.gameObject.SetActive(true);
        progressbarHandler.StartLoading();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
