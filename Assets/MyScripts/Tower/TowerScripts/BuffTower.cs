using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTower : TowerBase
{
    [SerializeField] private float buffRange = 10f; // 버프 범위
    [SerializeField] private float attackPowerMultiplier = 1.2f; // 공격력 배수 (20% 증가)
    [SerializeField] private float buffDuration = 5f; // 버프 지속 시간
    [SerializeField] private float buffInterval = 6f; // 버프 주기
    private List<TowerBase> towersInRange = new List<TowerBase>(); // 범위 내 타워 목록
    private ILineRendererStrategy lineRendererStrategy;
    public int segments = 50; // 라인 렌더러 세그먼트 수

    void Start()
    {
        // 라인 렌더러 전략 설정
        lineRendererStrategy = new CircleRendererStrategy();

        // 라인 렌더러 설정 및 패턴 생성
        lineRendererStrategy.Setup(gameObject);
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, transform, segments, buffRange, 0);

        StartCoroutine(BuffTowersInRange()); // 버프 루틴 시작

        // 설치 비용 설정
        installationCost = 15;
    }

    void Update()
    {
        // 라인 렌더러를 사용하여 버프 범위 시각화
        GenerateRangeVisualization();
    }

    private void GenerateRangeVisualization()
    {
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, transform, segments, buffRange, 0);
    }

    private IEnumerator BuffTowersInRange()
    {
        while (true)
        {
            DetectTowersInRange(); // 범위 내 타워 감지
            ApplyBuffToTowers(); // 타워에 버프 적용
            yield return new WaitForSeconds(buffInterval); // 버프 주기 대기
        }
    }

    private void DetectTowersInRange()
    {
        towersInRange.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRange); // 범위 내 콜라이더 탐지
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Towers"))
            {
                TowerBase tower = hitCollider.GetComponent<TowerBase>();
                if (tower != null && !towersInRange.Contains(tower))
                {
                    towersInRange.Add(tower); // 범위 내 타워 목록에 추가
                }
            }
        }
    }

    private void ApplyBuffToTowers()
    {
        StartCoroutine(ApplyBuffForDuration()); // 버프 지속 시간 동안 적용
    }

    private IEnumerator ApplyBuffForDuration()
    {
        List<TowerBase> buffedTowers = new List<TowerBase>(towersInRange);

        // 타워들의 공격력 증가
        foreach (var tower in buffedTowers)
        {
            tower.towerAttackPower *= attackPowerMultiplier;
        }

        yield return new WaitForSeconds(buffDuration); // 버프 지속 시간 대기

        // 타워들의 공격력 원래대로 복원
        foreach (var tower in buffedTowers)
        {
            if (tower != null)
            {
                tower.towerAttackPower /= attackPowerMultiplier;
            }
        }
    }

    public override void TowerAttack(List<Transform> targets)
    {
        // 버프 타워는 공격 기능이 없으므로 구현하지 않습니다.
    }

    public override void SetRange(float range)
    {
        buffRange = range; // 버프 범위 설정
        Debug.Log("버프 범위 설정: " + buffRange);
    }

    public override void TowerPowUp()
    {
        Debug.Log("버프 타워가 파워업되었습니다!");
    }

    public override void DetectEnemiesInRange()
    {
        // 이 메서드는 버프 타워에 필요하지 않습니다.
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffRange); // 버프 범위 시각화
    }
}
