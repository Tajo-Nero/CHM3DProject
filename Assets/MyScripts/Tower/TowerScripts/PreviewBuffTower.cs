using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBuffTower : MonoBehaviour
{
    private ILineRendererStrategy lineRendererStrategy;
    public int segments = 45; // ���׸�Ʈ ��
    public float radius = 10.0f; // �ݰ�

    void Start()
    {
        // ���� ������ ���� ���� �� �ʱ�ȭ
        lineRendererStrategy = new CircleRendererStrategy();
        lineRendererStrategy.Setup(gameObject);

        // ���� ������ ���� �� ���� ����
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, transform, segments, radius, 0);
    }

    void Update()
    {
        // ���� ������ ���� ������Ʈ
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, transform, segments, radius, 0);
    }

    // ������ ������ ������ �� �ִ� �޼���
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(gameObject);
    }
}
