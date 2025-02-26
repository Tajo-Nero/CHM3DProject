using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTower : TowerBase
{
    [SerializeField] private float buffRange = 10f; // 강화 범위
    [SerializeField] private float attackPowerMultiplier = 1.2f; // 공격력 배율 (20% 증가)
    [SerializeField] private float buffDuration = 5f; // 강화 지속 시간
    [SerializeField] private float buffInterval = 6f; // 강화 주기
    private List<TowerBase> towersInRange = new List<TowerBase>(); // 범위 내 타워 목록

    void Start()
    {
        // 주기적으로 범위 내 타워를 감지하고 강화하는 코루틴 시작
        StartCoroutine(BuffTowersInRange());

        // 설치 비용 설정
        installationCost = 15;
    }

    private IEnumerator BuffTowersInRange()
    {
        while (true)
        {
            DetectTowersInRange(); // 범위 내 타워 감지
            ApplyBuffToTowers(); // 감지된 타워 강화
            yield return new WaitForSeconds(buffInterval); // 강화 주기마다 범위 내 타워를 감지하고 강화
        }
    }

    private void DetectTowersInRange()
    {
        towersInRange.Clear(); // 기존 타워 목록 초기화
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRange); // 범위 내 충돌체 감지
        foreach (var hitCollider in hitColliders)
        {
            TowerBase tower = hitCollider.GetComponent<TowerBase>();
            if (tower != null && !towersInRange.Contains(tower))
            {
                towersInRange.Add(tower); // 감지된 타워를 목록에 추가
            }
        }
    }

    private void ApplyBuffToTowers()
    {
        StartCoroutine(ApplyBuffForDuration()); // 강화 지속 시간을 적용하는 코루틴 시작
    }

    private IEnumerator ApplyBuffForDuration()
    {
        List<TowerBase> buffedTowers = new List<TowerBase>(towersInRange); // 복사한 타워 목록 생성

        // 타워들의 공격력 증가
        foreach (var tower in buffedTowers)
        {
            tower.towerAttackPower *= attackPowerMultiplier; // 공격력 증가
        }

        yield return new WaitForSeconds(buffDuration); // 강화 지속 시간 동안 대기

        // 타워들의 공격력 원래대로 복구
        foreach (var tower in buffedTowers)
        {
            if (tower != null) // 타워가 아직 존재하는지 확인
            {
                tower.towerAttackPower /= attackPowerMultiplier; // 공격력 복구
            }
        }
    }

    public override void TowerAttack(List<Transform> targets)
    {
        // 버프 타워는 공격 기능이 없으므로 비워둡니다.
    }

    public override void SetRange(float range)
    {
        buffRange = range; // 강화 범위 설정
        Debug.Log("강화 범위 설정됨: " + buffRange);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffRange); // 강화 범위를 시각적으로 표시
    }
}
