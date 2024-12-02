using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeMaterial : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.05f;
    private float minY = -7;
    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Current moveSpeed: {moveSpeed}");
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        if(transform.position.y < minY) {
            Destroy(gameObject);   // 재료가 사라짐
        }
    }

    public void SetMoveSpeed(float speed) {
        moveSpeed = speed;
    }
}
