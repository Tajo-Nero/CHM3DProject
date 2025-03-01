using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewRocket : MonoBehaviour
{
    private ILineRendererStrategy lineRendererStrategy;
    public int segments = 20;
    [SerializeField] private Transform rocketLaunchPoint;
    [SerializeField] private float attackConeAngle = 45f;
    [SerializeField] private float detectionRange = 10f;

    void Start()
    {
        // ���� ������ ���� ���� �� �ʱ�ȭ
        lineRendererStrategy = new FanRendererStrategy();
        lineRendererStrategy.Setup(gameObject);

        // ���� ������ ���� �� ���� ����
        lineRendererStrategy.GeneratePattern(gameObject, rocketLaunchPoint.position, transform, segments, attackConeAngle, detectionRange);
    }

    void Update()
    {
        // ���� ������ ���� ������Ʈ
        lineRendererStrategy.GeneratePattern(gameObject, rocketLaunchPoint.position, transform, segments, attackConeAngle, detectionRange);
    }

    // ������ ������ ������ �� �ִ� �޼���
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(gameObject);
    }
}
