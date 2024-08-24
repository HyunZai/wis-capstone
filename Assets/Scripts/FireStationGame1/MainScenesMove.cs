using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScen : MonoBehaviour
{
    public void MainScenesCtrl()
    {
        SceneManager .LoadScene("SampleScene");
    }
}
