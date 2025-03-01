using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCannon : MonoBehaviour
{
    private ILineRendererStrategy lineRendererStrategy;
    private LineRenderer lineRenderer;
    public int segments = 50; // 세그먼트 수
    public float parameter = 8.0f; // 파라미터 (예: 반경 또는 길이)

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>(); // LineRenderer 컴포넌트를 오브젝트에 추가
        }

        // 렌더러 전략 초기화하고 설정
        lineRendererStrategy = new CircleRendererStrategy();
        lineRendererStrategy.Setup(gameObject);

        lineRenderer.positionCount = segments + 1; // 세그먼트 수 설정
    }

    void Update()
    {
        // 패턴 생성
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, transform, segments, parameter, parameter);
    }

    // 렌더러 전략을 설정할 수 있는 메서드
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(gameObject);
    }
}
