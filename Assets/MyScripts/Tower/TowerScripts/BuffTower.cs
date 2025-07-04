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

    // 성능 최적화: 타워 탐지 주기 설정
    [SerializeField] private float towerDetectionInterval = 0.5f; // 0.5초마다 타워 탐지
    private float lastTowerDetectionTime = 0f;

    // 버프 시스템 안전성 강화
    private HashSet<TowerBase> currentlyBuffedTowers = new HashSet<TowerBase>(); // 현재 버프 받는 타워들
    private Dictionary<TowerBase, Coroutine> buffCoroutines = new Dictionary<TowerBase, Coroutine>(); // 버프 코루틴 관리

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
        // 수정: 주기적으로만 타워 탐지 (매 프레임 대신)
        if (Time.time - lastTowerDetectionTime >= towerDetectionInterval)
        {
            DetectTowersInRange();
            lastTowerDetectionTime = Time.time;
        }

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
            // 개선: 탐지와 버프 적용을 분리
            ApplyBuffToDetectedTowers(); // 이미 탐지된 타워들에게만 버프 적용
            yield return new WaitForSeconds(buffInterval); // 버프 주기 대기
        }
    }

    private void DetectTowersInRange()
    {
        // 성능 최적화: 기존 리스트 재사용
        towersInRange.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRange); // 범위 내 콜라이더 탐지

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Towers"))
            {
                TowerBase tower = hitCollider.GetComponent<TowerBase>();
                if (tower != null && tower != this) // ⭐ 자기 자신 제외
                {
                    towersInRange.Add(tower); // 범위 내 타워 목록에 추가
                }
            }
        }
    }

    private void ApplyBuffToDetectedTowers()
    {
        // 개선: 유효한 타워들만 필터링
        List<TowerBase> validTowers = new List<TowerBase>();

        foreach (var tower in towersInRange)
        {
            if (tower != null && tower.gameObject.activeInHierarchy)
            {
                validTowers.Add(tower);
            }
        }

        if (validTowers.Count > 0)
        {
            foreach (var tower in validTowers)
            {
                // 중복 버프 방지
                if (!currentlyBuffedTowers.Contains(tower))
                {
                    StartCoroutine(ApplyBuffForDuration(tower));
                }
            }
        }
    }

    private IEnumerator ApplyBuffForDuration(TowerBase tower)
    {
        // 안전성 체크
        if (tower == null || !tower.gameObject.activeInHierarchy)
        {
            yield break;
        }

        // 버프 적용
        currentlyBuffedTowers.Add(tower);
        tower.towerAttackPower *= attackPowerMultiplier;

        Debug.Log($"버프 적용: {tower.name} - 공격력: {tower.towerAttackPower}");

        yield return new WaitForSeconds(buffDuration); // 버프 지속 시간 대기

        // 버프 해제 시 안전성 검사
        if (tower != null && tower.gameObject.activeInHierarchy && currentlyBuffedTowers.Contains(tower))
        {
            tower.towerAttackPower /= attackPowerMultiplier;
            currentlyBuffedTowers.Remove(tower);

            Debug.Log($"버프 해제: {tower.name} - 공격력: {tower.towerAttackPower}");
        }
        else if (currentlyBuffedTowers.Contains(tower))
        {
            // 타워가 파괴된 경우에도 집합에서 제거
            currentlyBuffedTowers.Remove(tower);
            Debug.Log("파괴된 타워의 버프 정보 정리됨");
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
        // 파워업 시 버프 효과 강화
        attackPowerMultiplier += 0.1f; // 버프 배수 증가 (1.2 → 1.3)
        buffRange += 2f; // 버프 범위 증가

        Debug.Log($"버프 타워가 파워업되었습니다! 버프 배수: {attackPowerMultiplier}, 범위: {buffRange}");
    }

    public override void DetectEnemiesInRange()
    {
        // 이 메서드는 버프 타워에 필요하지 않습니다.
    }

    // 정리 함수 추가
    void OnDestroy()
    {
        // 타워가 파괴될 때 모든 버프 해제
        foreach (var tower in currentlyBuffedTowers)
        {
            if (tower != null && tower.gameObject.activeInHierarchy)
            {
                tower.towerAttackPower /= attackPowerMultiplier;
            }
        }

        currentlyBuffedTowers.Clear();

        // 실행 중인 모든 코루틴 정리
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffRange); // 버프 범위 시각화
    }
}