using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialPortManager : MonoBehaviour
{
    private string portName = "COM3"; // 시리얼 포트 이름
    private int baudRate = 115200;      // 시리얼 통신 속도
    private SerialPort serialPort;
    private Thread serialThread;
    private bool isSerialPortRunning = false;
    private ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

    private CC CharacterController;

    // 시리얼 포트 통신 테스트용
    private LogManager logManager;
    // Start is called before the first frame update
    void Start()
    {
        if (CharacterController == null) CharacterController = new CC();
        if (serialPort != null) serialPort.Close();

        if (serialPort == null || !serialPort.IsOpen) {
            //시리얼 포트 연결 및 세팅
            logManager = new LogManager();
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 1000;
            try
            {
                serialPort.Open();
                isSerialPortRunning = true;
                serialThread = new Thread(ReadSerialData);
                serialThread.Start();
            }
            catch (IOException e)
            {
                Debug.LogError("Serial port could not be opened: " + e.Message);
                logManager.Log($"Serial port could not be opened: {e.Message}", "", LogType.Error);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //시리얼 포트 통신 코드 추가
        if (dataQueue.TryDequeue(out string sensorData)) CharacterController.MoveCharacter(sensorData);
    }

    private string previousData;
    //시리얼 포트 통신 코드 추가
    private void ReadSerialData()
    {
        while (isSerialPortRunning)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    string sensorData = serialPort.ReadLine();

                    if (string.IsNullOrEmpty(previousData)) 
                    {
                        previousData = sensorData;
                    }
                    else if (previousData != sensorData && sensorData != "00") 
                    {
                        dataQueue.Enqueue(sensorData.Trim()); // 읽은 데이터를 큐에 추가
                    }
                    else
                    {
                        previousData = sensorData;
                    }
                    
                }
                catch (TimeoutException)
                {
                    // 데이터가 없으면 무시
                }
                catch (Exception e)
                {
                    Debug.LogError("Error reading from serial port: " + e.Message);
                }
            }
        }
    }

    // private string previousData; //센서값 변동 체크하기 위한 변수
    // void MoveCharacter(string data)
    // {
    //     logManager.Log($"Received data: {data}", "", LogType.Log);
    //     //Debug.Log($"받은 데이터: {data}");

    //     if (string.IsNullOrEmpty(previousData))
    //     {
    //         previousData = data;
    //     }
    //     else if (previousData != data && !isMoveNow && data != "00")
    //     {
    //         SetDestination(Int32.Parse(data.Trim()) - 1);
    //         previousData = data;
    //     }
    //     else previousData = data;
    // }

    public void Disconnect() 
    {
        isSerialPortRunning = false;
        if (serialPort != null && serialPort.IsOpen) serialPort.Close();
        if (serialThread != null && serialThread.IsAlive) serialThread.Join();
    }

    void OnApplicationQuit() { Disconnect(); }
}
