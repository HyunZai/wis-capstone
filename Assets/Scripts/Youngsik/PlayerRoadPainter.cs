using System.Collections.Generic;
using UnityEngine;

public class ObjectPainter : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();
    private Gradient gradient;
    public Transform player; // 플레이어 Transform 연결

    void Start()
    {
        // 무지개 색상 그라디언트 생성
        gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.red, 0.0f),
            new GradientColorKey(Color.yellow, 0.2f),
            new GradientColorKey(Color.green, 0.4f),
            new GradientColorKey(Color.cyan, 0.6f),
            new GradientColorKey(Color.blue, 0.8f),
            new GradientColorKey(Color.magenta, 1.0f)
        };
        gradient.alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1.0f, 0.0f),
            new GradientAlphaKey(1.0f, 1.0f)
        };

        // LineRenderer의 색상 그라디언트 설정
        lineRenderer.colorGradient = gradient;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;

        // 새로운 점 추가 조건
        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], currentPosition) > 0.1f)
        {
            points.Add(currentPosition);
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }

        // 플레이어가 밟은 구간 삭제
        RemovePathAtPlayerPosition();
    }

    void RemovePathAtPlayerPosition()
    {
        if (points.Count == 0) return;

        float removeRadius = 0.1f; // 지워지는 반경 설정
        Vector3 playerPosition = player.position;

        // 플레이어 위치와 가까운 점들을 제거
        for (int i = points.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(points[i], playerPosition) < removeRadius)
            {
                points.RemoveAt(i);
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
        }
    }
}