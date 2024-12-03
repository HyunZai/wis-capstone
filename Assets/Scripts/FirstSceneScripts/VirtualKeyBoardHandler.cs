using System.Diagnostics;
using TMPro;
using UnityEngine;

public class VirtualKeyBoardHandler : MonoBehaviour
{
    public TMP_InputField[] inputFields; // TMP_InputField 배열
    private TouchScreenKeyboard keyboard;
    private TMP_InputField currentField;

    private void Start()
    {
        // 모든 TMP_InputField에 이벤트 리스너 추가
        foreach (var inputField in inputFields)
        {
            inputField.onSelect.AddListener((string text) => OpenKeyboard(inputField));
            inputField.onDeselect.AddListener((string text) => CloseKeyboard());
        }
    }

    private void OpenKeyboard(TMP_InputField selectedField)
    {
        // 현재 선택된 InputField를 저장
        currentField = selectedField;

        // TouchScreenKeyboard 열기
        keyboard = TouchScreenKeyboard.Open(selectedField.text, TouchScreenKeyboardType.Default);
    }

    private void Update()
    {
        // 키보드가 열려있고 입력 중인 상태에서
        if (keyboard != null && keyboard.active)
        {
            // 입력 값이 변경되면 필드에 텍스트 반영
            if (currentField != null && keyboard.text != currentField.text)
            {
                currentField.text = keyboard.text;
            }

            // 키보드 상태가 완료되었으면 종료 처리
            if (keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                keyboard = null; // 리소스 해제
            }
        }
    }

    private void CloseKeyboard()
    {
        // 키보드가 열려있으면 닫기
        if (keyboard != null && keyboard.active)
        {
            keyboard.active = false; // 키보드 비활성화
            keyboard = null; // 리소스 해제
        }
    }

    // public TMP_InputField[] inputFields;
    // private Process onScreenKeyboardProcess;

    // private void Start()
    // {
    //     if (!Application.platform.ToString().Contains("Windows")) return;

    //     foreach (TMP_InputField field in inputFields) 
    //     {
    //         field.onSelect.AddListener(OnInputFieldSelected);
    //         field.onDeselect.AddListener(OnInputFieldDeselected);
    //     }
    // }

    // private void OnInputFieldSelected(string text)
    // {
    //     ShowOnScreenKeyboard();
    // }

    // private void OnInputFieldDeselected(string text)
    // {
    //     CloseOnScreenKeyboard();
    // }

    // private void ShowOnScreenKeyboard()
    // {
    //     // Windows의 가상 키보드 실행
    //     if (onScreenKeyboardProcess == null || onScreenKeyboardProcess.HasExited)
    //     {
    //         onScreenKeyboardProcess = Process.Start(new ProcessStartInfo("osk.exe")
    //         {
    //             UseShellExecute = true // OS-level 실행
    //         });
    //     }
    // }

    // private void CloseOnScreenKeyboard()
    // {
    //     if (onScreenKeyboardProcess != null && !onScreenKeyboardProcess.HasExited)
    //     {
    //         // 실행 중인 키보드 프로세스 종료
    //         onScreenKeyboardProcess.Kill();
    //         onScreenKeyboardProcess.Dispose(); // 리소스 정리
    //         onScreenKeyboardProcess = null;
    //     }
    // }
}
