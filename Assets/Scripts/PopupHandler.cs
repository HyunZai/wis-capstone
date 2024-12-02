using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    public GameObject popup;
    public Button nopeButton;

    // Start is called before the first frame update
    void Start()
    {
        nopeButton.onClick.AddListener(NopeButtonClick);
    }

    void NopeButtonClick()
    {
        popup.SetActive(false);
    }
}
