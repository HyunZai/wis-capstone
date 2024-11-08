using System.Collections;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialReader : MonoBehaviour
{
    SerialPort serialPort;
    public string portName = "COM3"; // 사용 중인 포트 이름으로 변경
    public int baudRate = 115200;

    Thread serialThread;
    private bool isRunning = false;
    private ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

    void Start()
    {
        Debug.Log("Serial port read on!");
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 1000;

        try
        {
            serialPort.Open();
            isRunning = true;
            // 시리얼 포트 데이터를 별도의 스레드에서 읽기 시작
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
        // 시리얼 포트에서 받은 데이터를 메인 스레드에서 처리
        if (dataQueue.TryDequeue(out string sensorData))
        {
            ProcessSensorData(sensorData);
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
                    dataQueue.Enqueue(sensorData); // 읽은 데이터를 큐에 추가
                }
                catch (System.TimeoutException)
                {
                    // 타임아웃 예외 처리 (데이터가 없으면 무시)
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading from serial port: " + e.Message);
                }
            }
        }
    }

    void ProcessSensorData(string data)
    {
        Debug.Log(data);
        // 받은 센서 ID에 따라 Unity에서 특정 동작을 실행
        // switch (data.Trim())
        // {
        //     case "1":
        //         Debug.Log("Sensor 1 activated");
        //         break;
        //     case "2":
        //         Debug.Log("Sensor 2 activated");
        //         break;
        //     default:
        //         Debug.LogWarning("Unknown sensor data: " + data);
        //         break;
        // }
    }

    void OnApplicationQuit()
    {
        isRunning = false; // 스레드 종료를 위한 플래그 설정
        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join(); // 스레드 종료 대기
        }
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();  // 시리얼 포트 닫기
        }

        // 스레드 종료 대기 - 최대 1초로 설정하여 무기한 대기 방지
        // if (serialThread != null && serialThread.IsAlive)
        // {
        //     if (!serialThread.Join(1000))  // 1초 대기
        //     {
        //         Debug.LogWarning("Serial thread did not terminate in time.");
        //     }
        // }
    }
}
