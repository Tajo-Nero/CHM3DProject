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

    // 버프 관리
    private HashSet<TowerBase> towersInRange = new HashSet<TowerBase>();
    private HashSet<TowerBase> currentlyBuffedTowers = new HashSet<TowerBase>();
    private Dictionary<TowerBase, Coroutine> buffCoroutines = new Dictionary<TowerBase, Coroutine>();

    // 성능 관리
    private float lastTowerDetectionTime = 0f;

    protected override void Start()
    {
        installationCost = 15;
        detectionRange = buffRange;

        base.Start(); // TowerBase의 Start 호출

        // 버프 타워는 원형 범위
        if (rangeDisplay != null)
        {
            rangeDisplay.shape = TowerRangeDisplay.RangeShape.Circle;
            rangeDisplay.UpdateRangeMesh();
        }

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
            ApplyBuffToDetectedTowers();
            yield return new WaitForSeconds(buffInterval);
        }
    }

    private void DetectTowersInRange()
    {
        towersInRange.Clear();

        // buffRange 사용 (detectionRange와 동일하게 설정됨)
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
        List<TowerBase> invalidTowers = new List<TowerBase>();

        foreach (var tower in towersInRange)
        {
            if (tower == null || !tower.gameObject.activeInHierarchy)
            {
                invalidTowers.Add(tower);
                continue;
            }

            if (!currentlyBuffedTowers.Contains(tower))
            {
                if (buffCoroutines.ContainsKey(tower) && buffCoroutines[tower] != null)
                {
                    StopCoroutine(buffCoroutines[tower]);
                }

                Coroutine buffCoroutine = StartCoroutine(ApplyBuffForDuration(tower));
                buffCoroutines[tower] = buffCoroutine;
            }
        }

        foreach (var tower in invalidTowers)
        {
            towersInRange.Remove(tower);
        }
    }

    private IEnumerator ApplyBuffForDuration(TowerBase tower)
    {
        if (tower == null || !tower.gameObject.activeInHierarchy)
        {
            yield break;
        }

        currentlyBuffedTowers.Add(tower);
        tower.towerAttackPower *= attackPowerMultiplier;

        ApplyBuffVisualEffect(tower, true);

        yield return new WaitForSeconds(buffDuration);

        RemoveBuff(tower);
    }

    private void RemoveBuff(TowerBase tower)
    {
        if (tower != null && tower.gameObject.activeInHierarchy && currentlyBuffedTowers.Contains(tower))
        {
            tower.towerAttackPower /= attackPowerMultiplier;
            ApplyBuffVisualEffect(tower, false);
        }

        currentlyBuffedTowers.Remove(tower);

        if (buffCoroutines.ContainsKey(tower))
        {
            buffCoroutines.Remove(tower);
        }
    }

    private void ApplyBuffVisualEffect(TowerBase tower, bool isActive)
    {
        if (tower == null) return;

        Renderer renderer = tower.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (isActive)
            {
                renderer.material.EnableKeyword("_EMISSION");
            }
            else
            {
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
        detectionRange = range;
        SetRangeSize(range); // TowerBase의 메서드 호출
    }

    public override void TowerPowUp()
    {
        attackPowerMultiplier += 0.1f;
        buffRange += 2f;
        SetRange(buffRange);
    }

    public override void DetectEnemiesInRange()
    {
        // 버프 타워는 적을 감지하지 않음
    }

    void OnDestroy()
    {
        List<TowerBase> buffedTowers = new List<TowerBase>(currentlyBuffedTowers);
        foreach (var tower in buffedTowers)
        {
            RemoveBuff(tower);
        }

        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffRange);
    }
}