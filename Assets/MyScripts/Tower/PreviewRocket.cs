using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewRocket : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public int segments = 20;
    [SerializeField] private Transform rocketLaunchPoint;
    [SerializeField] private float attackConeAngle = 45f;
    [SerializeField] private float detectionRange = 10f;
    private ILineRendererStrategy lineRendererStrategy;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRendererStrategy = new FanRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = segments + 2;
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
    }

    void Update()
    {
        lineRendererStrategy.GeneratePattern(lineRenderer, rocketLaunchPoint.position, transform, segments, attackConeAngle, detectionRange);
    }

    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(lineRenderer);
    }
}
