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
        // 라인 렌더러 전략 설정 및 초기화
        lineRendererStrategy = new FanRendererStrategy();
        lineRendererStrategy.Setup(gameObject);

        // 라인 렌더러 설정 및 패턴 생성
        lineRendererStrategy.GeneratePattern(gameObject, rocketLaunchPoint.position, transform, segments, attackConeAngle, detectionRange);
    }

    void Update()
    {
        // 라인 렌더러 패턴 업데이트
        lineRendererStrategy.GeneratePattern(gameObject, rocketLaunchPoint.position, transform, segments, attackConeAngle, detectionRange);
    }

    // 렌더러 전략을 설정할 수 있는 메서드
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(gameObject);
    }
}
