using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewEnforceTower : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public int segments = 45;
    public float radius = 10.0f;
    private ILineRendererStrategy lineRendererStrategy;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // ���� �ʱ�ȭ �� ����
        lineRendererStrategy = new CircleRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
    }

    private void Update()
    {
        lineRendererStrategy.GeneratePattern(lineRenderer, transform.position, transform, segments, radius, radius);
    }


    // ������ ������ �� �ִ� �޼���
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(lineRenderer);
    }
}
