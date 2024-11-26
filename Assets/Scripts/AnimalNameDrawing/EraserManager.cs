using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EraserManager : MonoBehaviour
{
    private float elapsedTime = 0.0f;
    private float longClickTime = 1.0f;
    private bool isClicked = false;
    private bool isLongClick = false;
    
    public void OnMouseDown() { isClicked = true; }

    public void OnMouseUp() 
    { 
        isClicked = false;
        isLongClick = false;
        elapsedTime = 0.0f;   
    }
    // Update is called once per frame
    void Update()
    {
        if (isClicked)
        {
            elapsedTime += Time.deltaTime;
            if (longClickTime < elapsedTime) isLongClick = true;
        }

        if (isLongClick)
        {
            LineRenderer[] lines = FindObjectsOfType<LineRenderer>();
            foreach (LineRenderer line in lines) line.positionCount = 0;
        }
    }
}
