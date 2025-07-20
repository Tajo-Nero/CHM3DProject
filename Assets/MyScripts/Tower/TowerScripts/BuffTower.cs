using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTower : TowerBase
{
    [Header("버프 설정")]
    [SerializeField] private float buffRange = 10f;
    [SerializeField] private float attackPowerMultiplier = 1.2f;
    [SerializeField] private float buffDuration = 5f;
    [SerializeField] private float buffInterval = 6f;
    [SerializeField] private float towerDetectionInterval = 0.5f;

    [Header("시각화")]
    public int segments = 50;

    // 버프 관리
    private HashSet<TowerBase> towersInRange = new HashSet<TowerBase>();
    private HashSet<TowerBase> currentlyBuffedTowers = new HashSet<TowerBase>();
    private Dictionary<TowerBase, Coroutine> buffCoroutines = new Dictionary<TowerBase, Coroutine>();

    // 성능 관리
    private float lastTowerDetectionTime = 0f;

    void Start()
    {
        // 기본 설정
        installationCost = 15;

        // 버프 루틴 시작
        StartCoroutine(BuffRoutine());
    }

    void Update()
    {
        // 주기적 타워 탐지
        if (Time.time - lastTowerDetectionTime >= towerDetectionInterval)
        {
            DetectTowersInRange();
            lastTowerDetectionTime = Time.time;
        }
    }

    private IEnumerator BuffRoutine()
    {
        while (true)
        {
            // 일정 간격으로 버프 적용
            ApplyBuffToDetectedTowers();
            yield return new WaitForSeconds(buffInterval);
        }
    }

    private void DetectTowersInRange()
    {
        // 탐지 전 기존 목록 초기화
        towersInRange.Clear();

        // 효율적인 충돌 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Towers"))
            {
                TowerBase tower = hitCollider.GetComponent<TowerBase>();
                if (tower != null && tower != this)
                {
                    towersInRange.Add(tower);
                }
            }
        }
    }

    private void ApplyBuffToDetectedTowers()
    {
        // 유효한 타워 확인
        List<TowerBase> invalidTowers = new List<TowerBase>();

        foreach (var tower in towersInRange)
        {
            if (tower == null || !tower.gameObject.activeInHierarchy)
            {
                invalidTowers.Add(tower);
                continue;
            }

            // 이미 버프 중인 타워는 스킵
            if (!currentlyBuffedTowers.Contains(tower))
            {
                // 이전 코루틴이 있다면 중지
                if (buffCoroutines.ContainsKey(tower) && buffCoroutines[tower] != null)
                {
                    StopCoroutine(buffCoroutines[tower]);
                }

                // 새 버프 코루틴 시작
                Coroutine buffCoroutine = StartCoroutine(ApplyBuffForDuration(tower));
                buffCoroutines[tower] = buffCoroutine;
            }
        }

        // 무효한 타워 제거
        foreach (var tower in invalidTowers)
        {
            towersInRange.Remove(tower);
        }
    }

    private IEnumerator ApplyBuffForDuration(TowerBase tower)
    {
        // 안전성 검사
        if (tower == null || !tower.gameObject.activeInHierarchy)
        {
            yield break;
        }

        // 버프 적용
        currentlyBuffedTowers.Add(tower);
        tower.towerAttackPower *= attackPowerMultiplier;

        // 시각적 효과 (옵션)
        ApplyBuffVisualEffect(tower, true);

        yield return new WaitForSeconds(buffDuration);

        // 버프 해제
        RemoveBuff(tower);
    }

    private void RemoveBuff(TowerBase tower)
    {
        // 안전성 검사
        if (tower != null && tower.gameObject.activeInHierarchy && currentlyBuffedTowers.Contains(tower))
        {
            tower.towerAttackPower /= attackPowerMultiplier;
            ApplyBuffVisualEffect(tower, false);
        }

        // 버프 목록에서 제거
        currentlyBuffedTowers.Remove(tower);

        // 코루틴 참조 제거
        if (buffCoroutines.ContainsKey(tower))
        {
            buffCoroutines.Remove(tower);
        }
    }

    private void ApplyBuffVisualEffect(TowerBase tower, bool isActive)
    {
        // 버프 시각 효과 (발광, 파티클 등)
        if (tower == null) return;

        // 예: 머티리얼 변경이나 파티클 효과
        Renderer renderer = tower.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (isActive)
            {
                // 버프 활성화 효과
                renderer.material.EnableKeyword("_EMISSION");
            }
            else
            {
                // 버프 비활성화 효과
                renderer.material.DisableKeyword("_EMISSION");
            }
        }
    }

    // TowerBase 구현 메서드
    public override void TowerAttack(List<Transform> targets)
    {
        // 버프 타워는 직접 공격하지 않음
    }

    public override void SetRange(float range)
    {
        buffRange = range;
    }

    public override void TowerPowUp()
    {
        // 파워업 효과
        attackPowerMultiplier += 0.1f;
        buffRange += 2f;

    }
    public override void DetectEnemiesInRange()
    {
        // 버프 타워는 적을 감지하지 않음
    }

    // 정리 및 안전성
    void OnDestroy()
    {
        // 모든 버프 해제
        List<TowerBase> buffedTowers = new List<TowerBase>(currentlyBuffedTowers);
        foreach (var tower in buffedTowers)
        {
            RemoveBuff(tower);
        }

        // 코루틴 정리
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        // 버프 범위 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffRange);
    }
}