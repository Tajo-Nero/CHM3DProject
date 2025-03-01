using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewLaser : MonoBehaviour
{
    private ILineRendererStrategy lineRendererStrategy;
    public int segments = 8; // ���׸�Ʈ ��
    public float detectionRange = 10.0f; // ���� ����
    [SerializeField] private Transform laserStartPoint; // ������ ������

    void Start()
    {
        // ���� ������ ���� ���� �� �ʱ�ȭ
        lineRendererStrategy = new LaserRendererStrategy();
        lineRendererStrategy.Setup(gameObject);

        // ���� ������ ���� �� ���� ����
        lineRendererStrategy.GeneratePattern(gameObject, laserStartPoint.position, transform, segments, detectionRange, 0);
    }

    void Update()
    {
        // ���� ������ ���� ������Ʈ
        lineRendererStrategy.GeneratePattern(gameObject, laserStartPoint.position, transform, segments, detectionRange, 0);
    }

    // ������ ������ ������ �� �ִ� �޼���
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(gameObject);
    }
}
