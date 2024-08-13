using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationBackground : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;
    public Sprite[] backgrounds;

    // Start is called before the first frame update
    void Start()
    {
        if (backgroundRenderer == null)
        {
            backgroundRenderer = GetComponent<SpriteRenderer>();
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeBackground(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeBackground(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeBackground(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeBackground(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeBackground(4);
        }
    }

    public void ChangeBackground(int index)
    {
        if (index >= 0 && index < backgrounds.Length)
        {
            backgroundRenderer.sprite = backgrounds[index];
        }
        else
        {
            Debug.LogError("Invalid background index");
        }
    }
}
