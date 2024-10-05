using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameController gameController;

    void OnMouseDown()
    {
        if (gameController != null)
        {
            gameController.ItemClicked(gameObject);
        }
    }
}
