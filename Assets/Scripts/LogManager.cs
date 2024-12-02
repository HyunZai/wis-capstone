using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    private string logFilePath = "wis_log.txt";
    // Start is called before the first frame update
    void Start()
    {
        Application.logMessageReceived += Log;
    }

    public void Log(string logString, string stackTrace, LogType type) 
    {
        File.AppendAllText(logFilePath, logString + "\n");
    }
}
