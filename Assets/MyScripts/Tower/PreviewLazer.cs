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

        // 원하는 전략을 초기화하고 설정
        lineRendererStrategy = new LaserRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
    }

    void Update()
    {
        // 패턴 생성
        lineRendererStrategy.GeneratePattern(lineRenderer, laserStartPoint.position, transform, 8, detectionRange, detectionRange);
    }

    // 전략을 변경할 수 있는 메서드
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(lineRenderer);
    }
}
