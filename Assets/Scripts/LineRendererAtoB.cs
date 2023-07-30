using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class LineRendererAtoB : MonoBehaviour
{
    LineRenderer lineRenderer;
    float current = 3;
    public float Length { get { return current; } }
    float minScale = 3;
    float maxScale = 50;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 4;
        lineRenderer.enabled = true;
        var temp = new List<Vector3> { new Vector3(-0.6f, -3, 0), new Vector3(-0.6f, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -3, 0) };
        lineRenderer.SetPositions(temp.ToArray());
    }

    public void AddLength(float length)
    {
        if (current + length < minScale || current + length > maxScale)
            return;

        length = -(current + length);
        lineRenderer.SetPosition(0, new Vector3(-0.6f, length));
        lineRenderer.SetPosition(3, new Vector3(1, length));
        current = -length;
    }

    public bool CanShorten()
    {
        return current - minScale > 0.1f;
    }
}
