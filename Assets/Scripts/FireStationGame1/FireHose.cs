using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHose : MonoBehaviour
{
    public float fixedY = -4.0f;  // 소방호스의 고정 y좌표
    public float moveSpeed = 5f;   // 소방호스의 이동 속도

    void Update()
    {
        // 마우스 클릭을 감지
        if (Input.GetMouseButton(0))
        {
            MoveHoseToMousePosition();
        }
    }

    void MoveHoseToMousePosition()
    {
        // 마우스 클릭 위치를 월드 좌표로 변환
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;  // 2D 게임에서는 z좌표를 0으로 설정

        // 소방호스의 위치를 클릭한 x좌표로 이동, y좌표는 고정
        Vector3 targetPosition = new Vector3(mousePosition.x, fixedY, 0f);

        // 소방호스가 이동하도록 하는 방법
        // 간단한 보간을 사용하여 이동 속도를 적용
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
