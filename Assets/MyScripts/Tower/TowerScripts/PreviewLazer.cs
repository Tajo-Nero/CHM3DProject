using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewLaser : MonoBehaviour
{
    private ILineRendererStrategy lineRendererStrategy;
    public int segments = 8; // 세그먼트 수
    public float detectionRange = 10.0f; // 감지 범위
    [SerializeField] private Transform laserStartPoint; // 레이저 시작점

    void Start()
    {
        // 라인 렌더러 전략 설정 및 초기화
        lineRendererStrategy = new LaserRendererStrategy();
        lineRendererStrategy.Setup(gameObject);

        // 라인 렌더러 설정 및 패턴 생성
        lineRendererStrategy.GeneratePattern(gameObject, laserStartPoint.position, transform, segments, detectionRange, 0);
    }

    void Update()
    {
        // 라인 렌더러 패턴 업데이트
        lineRendererStrategy.GeneratePattern(gameObject, laserStartPoint.position, transform, segments, detectionRange, 0);
    }

    // 렌더러 전략을 설정할 수 있는 메서드
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(gameObject);
    }
}
