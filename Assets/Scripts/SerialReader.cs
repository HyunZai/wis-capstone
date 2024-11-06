using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SerialReader : MonoBehaviour
{
    SerialPort serialPort;
    public string portNum = "포트넘버";
    public int baudRate = 19200;

    // Start is called before the first frame update
    void Start()
    {
        serialPort = new SerialPort(portNum, baudRate);
        try 
        {
            serialPort.Open();
        }
        catch(Exception ex) 
        {
            Debug.LogError("Serial port open error : " + ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (serialPort != null && serialPort.IsOpen) 
        {
            try
            {
                string buildingNum = serialPort.ReadLine();
                Debug.Log("Data received: " + buildingNum);
            }
            catch(TimeoutException){}
        }
    }

    private void OnDestory() {
        if (serialPort != null && serialPort.IsOpen) {
            serialPort.Close();
        }
    }
}
