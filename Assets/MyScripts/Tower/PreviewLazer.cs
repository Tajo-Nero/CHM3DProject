using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewLazer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private float detectionRange;
    private ILineRendererStrategy lineRendererStrategy;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // ���ϴ� ������ �ʱ�ȭ�ϰ� ����
        lineRendererStrategy = new LaserRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
    }

    void Update()
    {
        // ���� ����
        lineRendererStrategy.GeneratePattern(lineRenderer, laserStartPoint.position, transform, 8, detectionRange, detectionRange);
    }

    // ������ ������ �� �ִ� �޼���
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(lineRenderer);
    }
}
