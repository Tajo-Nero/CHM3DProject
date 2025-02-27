using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour 
{
    //1.타워 공격력
    //public int towerAttackPower;
    ////2.타워 관통력
    //public int towerPenetrationPower;
    ////3.치명타율
    //public float criticalHitRate;
    ////4.공격속도
    //public float attackSpeed;
    ////5.설치 비용
    //public int installationCost;
    //6.범위

    //공격 하는 함수,범위내의 적 감지 함수,범위 설정하는 함수
    //void TowerAttak()//타워 공격하는 기능
    //{
    //    
    //}
    //void SetRange()//타워 공격 가능한 범위
    //{
    //    
    //}
    //void DetectEnemiesInRange()//타워 범위내의 적 감지
    //{
    //    
    //}  

    //각각의 타워는 TowerBase를 상속받음

    //캐논 타워 단일대상 공격 캐논타워 
    //1 : 80 2: 25 3: 5% 4: 0.7 5 : 8 6 : 원형범위

    //레이저 타워 광통 공격 레이저 쏘는타워 
    //1 : 250 2: 0 3: 15% 4: 2.5 5: 11 6: 일직선범위

    //로켓 타워 스플레쉬공격하는 타워 
    //1 : 50 2: 100 3: 5% 4: 1.5 5:10 6: 넓은 부채꼴 범위
    //스플래쉬 범위, 지속데미지 변수 추가
    //스플래쉬 공격 함수, 지속 데미지를 주는 함수 추가

    //강화 타워 범위내의 타워가 있으면 강화시켜주고 범위내의 모든적 공격 가능
    //1 : 14 2: 100 3: 5% 4: 3.5 5: 14 6: 좁은 부채꼴 범위
    //강화지속시간, 공격력 올려줄 변수 추가
    //범위내의 Towers 태그 붙어있는 타워들 공격력 수치 증가하는 함수 추가

    // 1. 타워 공격력
    [Header("Tower Settings")]
    public float towerAttackPower;
    // 2. 타워 관통력
    public float towerPenetrationPower;
    // 3. 치명타 확률
    public float criticalHitRate;
    // 4. 공격 속도
    public float attackSpeed;
    // 5. 설치 비용
    public float installationCost;
    // 6. 공격 강화 가능여부
    public bool isAttackUp = false;

    // 적을 공격하는 함수
    public virtual void TowerAttack(List<Transform> targets)
    {
        // 공격 로직 구현        
    }

    // 범위를 설정하는 함수
    public virtual void SetRange(float range)
    {
        // 범위 설정 로직 구현
    }

    // 범위 내의 적을 탐지하는 함수
    public virtual void DetectEnemiesInRange()
    {
        // 적 탐지 로직 구현
    }
    public virtual void TowerPowUp()
    {
        if (isAttackUp)
        {
            towerAttackPower *= 2;
            isAttackUp = false;
        }
    }
}
