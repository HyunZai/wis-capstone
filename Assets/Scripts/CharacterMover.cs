using System.Collections;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public string portName = "COM3"; // 시리얼 포트 이름
    // public string portName = "/dev/tty.usbmodem21301";
    public int baudRate = 115200;      // 시리얼 통신 속도
    private SerialPort serialPort;
    private Thread serialThread;
    private bool isRunning = false;
    private ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

    // 시리얼 포트 통신 테스트용
    private LogManager logManager;

    void Start()
    {
        logManager = new LogManager();
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 1000;

        try
        {
            serialPort.Open();
            isRunning = true;
            serialThread = new Thread(ReadSerialData);
            serialThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial port could not be opened: " + e.Message);
        }
    }

    void Update()
    {
        if (dataQueue.TryDequeue(out string sensorData))
        {
            MoveCharacter(sensorData);
        }
    }

    private void ReadSerialData()
    {
        while (isRunning)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    string sensorData = serialPort.ReadLine();
                    dataQueue.Enqueue(sensorData.Trim()); // 읽은 데이터를 큐에 추가
                }
                catch (System.TimeoutException)
                {
                    // 데이터가 없으면 무시
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading from serial port: " + e.Message);
                }
            }
        }
    }

    void MoveCharacter(string data)
    {
        logManager.Log($"시리얼 포트로부터 받은 데이터: {data}", "", LogType.Log);
        Debug.Log($"받은 데이터: {data}");
        // switch (data.Trim())
        // {
        //     case "01":
        //         Debug.Log("받은 데이터: 01");
        //         break;
        //     case "02":
        //         Debug.Log("받은 데이터: 01");
        //         break;
        //     case "03":
        //         Debug.Log("받은 데이터: 01");
        //         break;
        //     default:
        //         Debug.LogWarning("Unknown data: " + data);
        //         break;
        // }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }

        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join();
        }
    }
}
