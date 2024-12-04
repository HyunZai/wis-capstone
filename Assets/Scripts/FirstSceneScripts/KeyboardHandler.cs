using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardHandler : MonoBehaviour
{
    public List<Button> buttons;
    // Start is called before the first frame update
    void Start()
    {
        //ShowKeyboard();
        buttons.ForEach(btn => { btn.onClick.AddListener(() => Pressedkey(btn)); });
    }

    private void Pressedkey(Button key) {
        Debug.Log(key.name);
    }

    public void ShowKeyboard() {
        gameObject.transform.localPosition = new Vector3(-130, -250, 0);
    }

    public void HideKeyboard() {
        gameObject.transform.localPosition = new Vector3(-130, -1000, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
