using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startfirestationgame : MonoBehaviour
{
    public void GameScenesCtrl()
    {
        SceneManager.LoadScene("FIrestationGame"); // 소방서 게임1 씬으로 이동
    }
}
