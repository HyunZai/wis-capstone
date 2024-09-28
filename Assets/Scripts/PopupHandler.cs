using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    public GameObject popup;
    public Button homeButton;
    public Button nopeButton;

    // Start is called before the first frame update
    void Start()
    {
        homeButton.onClick.AddListener(HomeButtonClick);
        nopeButton.onClick.AddListener(NopeButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HomeButtonClick()
    {

    }

    void NopeButtonClick()
    {
        popup.SetActive(false);
    }
}
