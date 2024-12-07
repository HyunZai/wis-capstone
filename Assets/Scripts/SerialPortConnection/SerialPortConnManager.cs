using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using UnityEngine;

public class SerialPortConnManager
{
    private string portName = "COM3"; // 시리얼 포트 이름
    private int baudRate = 115200;      // 시리얼 통신 속도
    private SerialPort serialPort;
    private Thread serialThread;
    public bool isSerialPortRunning = false;
    public ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

    private LogManager logManager;
    public void Connect() 
    {
        if (logManager == null) logManager = new LogManager();

        //직렬 포트 이름 가져와서 세팅 (여러 개의 직렬 포트가 연결되어있을 경우는 처리 안함)
        string[] portNames = SerialPort.GetPortNames();
        if (portNames.Length == 1) portName = portNames[0];

        if (serialPort == null || !serialPort.IsOpen) {
            //시리얼 포트 연결 및 세팅
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 1000;
            try
            {
                Thread.Sleep(500); //OS 캐싱 이슈로 인해 "엑세스가 거부되었습니다." 에러 발생 -> 해결하기 위해 0.5초 딜레이

                serialPort.Open();
                isSerialPortRunning = true;
                serialThread = new Thread(ReadSerialData);
                serialThread.Start();
            }
            catch (IOException e)
            {
                Debug.LogError($"Serial port could not be opened: {e.Message}");
                logManager.Log($"Serial port could not be opened: {e.Message}", "", LogType.Error);
            }
        }
    }
    // Start is called before the first frame update
    

    private void ReadSerialData()
    {
        while (isSerialPortRunning)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    string sensorData = serialPort.ReadLine();
                    dataQueue.Enqueue(sensorData.Trim()); // 읽은 데이터를 큐에 추가
                }
                catch (TimeoutException) {} //데이터가 없으면 무시
                catch (Exception e) { Debug.LogError("Error reading from serial port: " + e.Message); }
            }
        }
    }

    public void Disconnect()
    {
        isSerialPortRunning = false;
        if (serialPort != null && serialPort.IsOpen) serialPort.Close();
        if (serialThread != null && serialThread.IsAlive) serialThread.Join();
    }
}
