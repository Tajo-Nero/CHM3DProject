//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static UnityEngine.Rendering.HableCurve;

//public class PreviewCannon : MonoBehaviour
//{
//    private LineRenderer lineRenderer; // ���� ������
//    public int segments = 50; // ���׸�Ʈ ��

//    void Start()
//    {
//        // LineRenderer�� �̹� �����ϴ��� Ȯ��
//        lineRenderer = GetComponent<LineRenderer>();
//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>(); // LineRenderer�� ������ �߰�

//        }
//        lineRenderer.positionCount = segments + 1; // �߰��� ���׸�Ʈ �� ����
//        lineRenderer.loop = true;        
//        lineRenderer.startWidth = 0.2f;
//        lineRenderer.endWidth = 0.2f;
//    }
//    private void Update()
//    {
//        GenerateCircle(8);

//    }
//    private void GenerateCircle(float radius)
//    {       
//        float angleStep = 360f / segments;
//        for (int i = 0; i <= segments; i++)
//        {
//            float angle = i * angleStep * Mathf.Deg2Rad;
//            float x = Mathf.Cos(angle) * radius;
//            float z = Mathf.Sin(angle) * radius;
//            lineRenderer.SetPosition(i,transform.position+ new Vector3(x, 0, z));
//        }
//    }
//}
//�������� ����� �ʹ� ���������

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCannon : MonoBehaviour
{
    private ILineRendererStrategy lineRendererStrategy;
    private LineRenderer lineRenderer;
    public int segments = 50; // ���׸�Ʈ ��
    public float parameter = 8.0f; // �Ķ���� (��: ������ �Ǵ� ����)

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>(); // LineRenderer�� ���� ������Ʈ�� �߰�
        }

        // ���ϴ� ������ �ʱ�ȭ�ϰ� ����
        lineRendererStrategy = new CircleRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = segments + 1; // ���׸�Ʈ �� ����
    }

    void Update()
    {
        // ���� ����
        lineRendererStrategy.GeneratePattern(lineRenderer, transform.position, transform, segments, parameter, parameter);
    }

    // ���ϴ� �������� ������ �� �ִ� �޼���
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(lineRenderer);
    }
}

