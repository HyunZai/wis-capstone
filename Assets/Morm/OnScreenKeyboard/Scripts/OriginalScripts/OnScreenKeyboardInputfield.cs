using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class OnScreenKeyboardInputfield : MonoBehaviour, IPointerDownHandler
{
    public OnScreenKeyboard targetOnScreenKeyboard;
    public TMP_InputField _inputField;

    public GameObject[] moveObjects;
    public string inputtedString;

    private Vector3[] defaultPosition = new Vector3[3];

    private void Awake()
    {
        if (_inputField == null) return;

        _inputField.shouldHideMobileInput = true;
    }

    public void SaveInputedString(string _inputStr)
    {
        inputtedString = _inputStr;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetOnScreenKeyboard)
        {
            RectTransform rectTransform = moveObjects[0].GetComponent<RectTransform>();
            if ((_inputField.name == "Address" || _inputField.name == "PhoneNumber") && rectTransform.anchoredPosition.y < 480) {    
                foreach (GameObject item in moveObjects) {
                    defaultPosition.Append(item.transform.position);
                    item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + 2.4f, 0);
                }
            }

            targetOnScreenKeyboard.gameObject.SetActive(true);
            targetOnScreenKeyboard.ShowKeyboard(_inputField, this);
        }
    }

    public void FormPositionMove() {
        RectTransform rectTransform = moveObjects[0].GetComponent<RectTransform>();
        if (rectTransform.anchoredPosition.y > 480)
        {
            for (int i = 0; i < moveObjects.Length; i++) {
                moveObjects[i].transform.position = new Vector3(moveObjects[i].transform.position.x, moveObjects[i].transform.position.y - 2.4f, 0);
            }
        }
    }
}
