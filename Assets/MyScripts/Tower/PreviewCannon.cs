//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static UnityEngine.Rendering.HableCurve;

//public class PreviewCannon : MonoBehaviour
//{
//    private LineRenderer lineRenderer; // 라인 렌더러
//    public int segments = 50; // 세그먼트 수

//    void Start()
//    {
//        // LineRenderer가 이미 존재하는지 확인
//        lineRenderer = GetComponent<LineRenderer>();
//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>(); // LineRenderer가 없으면 추가

//        }
//        lineRenderer.positionCount = segments + 1; // 추가된 세그먼트 수 설정
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
//전략패턴 사용후 너무 깔끔해졌따

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCannon : MonoBehaviour
{
    private ILineRendererStrategy lineRendererStrategy;
    private LineRenderer lineRenderer;
    public int segments = 50; // 세그먼트 수
    public float parameter = 8.0f; // 파라미터 (예: 반지름 또는 각도)

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>(); // LineRenderer를 게임 오브젝트에 추가
        }

        // 원하는 전략을 초기화하고 설정
        lineRendererStrategy = new CircleRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = segments + 1; // 세그먼트 수 설정
    }

    void Update()
    {
        // 패턴 생성
        lineRendererStrategy.GeneratePattern(lineRenderer, transform.position, transform, segments, parameter, parameter);
    }

    // 원하는 전략으로 변경할 수 있는 메서드
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(lineRenderer);
    }
}

