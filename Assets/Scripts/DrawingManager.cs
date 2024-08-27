using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    public GameObject linePrefab;
    private GameObject currentLine;
    public LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();

    void Start() {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateNewLine();
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            if (!points.Contains(mousePos))
            {
                points.Add(mousePos);
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPosition(points.Count - 1, mousePos);
            }
        }
    }

    void CreateNewLine()
    {
        currentLine = Instantiate(linePrefab);
        currentLine.transform.parent = this.transform;
        lineRenderer = currentLine.GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.35f;
        lineRenderer.endWidth = 0.35f;

        lineRenderer.sortingLayerName = "Foreground";
        lineRenderer.sortingOrder = 5;

        points.Clear();
    }
}
