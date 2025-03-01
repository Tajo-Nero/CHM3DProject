using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCannon : MonoBehaviour
{
    private ILineRendererStrategy lineRendererStrategy;
    private LineRenderer lineRenderer;
    public int segments = 50; // ���׸�Ʈ ��
    public float parameter = 8.0f; // �Ķ���� (��: �ݰ� �Ǵ� ����)

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>(); // LineRenderer ������Ʈ�� ������Ʈ�� �߰�
        }

        // ������ ���� �ʱ�ȭ�ϰ� ����
        lineRendererStrategy = new CircleRendererStrategy();
        lineRendererStrategy.Setup(gameObject);

        lineRenderer.positionCount = segments + 1; // ���׸�Ʈ �� ����
    }

    void Update()
    {
        // ���� ����
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, transform, segments, parameter, parameter);
    }

    // ������ ������ ������ �� �ִ� �޼���
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(gameObject);
    }
}
