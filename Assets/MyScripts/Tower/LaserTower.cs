//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LaserTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 10f; // 탐지 범위
//    [SerializeField] private Transform laserStartPoint; // 레이저가 시작되는 위치
//    private LineRenderer lineRenderer; // 레이저를 그릴 LineRenderer
//    private bool isAttacking = false; // 공격 중 여부
//    private bool showGizmos = false; // Gizmos 표시 여부
//    [SerializeField] private float laserLength = 10f; // 레이저 길이

//    void Start()
//    {
//        towerAttackPower = 50; // 타워의 공격력 설정
//        attackSpeed = 4f; // 공격 속도 설정
//        installationCost = 15; // 설치 비용 설정

//        SetRange(detectionRange); // 탐지 범위 설정
//        lineRenderer = GetComponent<LineRenderer>(); // LineRenderer 컴포넌트 참조

//        // LineRenderer가 없으면 추가
//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>();
//        }
//    }

//    void Update()
//    {
//        DetectEnemiesInRange(); // 범위 내 적 탐지
//    }

//    public override void DetectEnemiesInRange()
//    {
//        List<Transform> targets = new List<Transform>(); 

//        // 박스 레이케스트를 위한 중심점과 반지름 설정
//        Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (laserLength / 2);
//        Vector3 boxHalfExtents = new Vector3(0.2f, 0.2f, laserLength / 2);

//        // 박스 레이케스트
//        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, laserStartPoint.rotation);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                targets.Add(hitCollider.transform); 
//                Debug.Log("박스 레이케스트로 적 감지됨: " + hitCollider.transform.name);
//            }
//        }

//        if (targets.Count > 0)
//        {

//            StartCoroutine(AttackRoutine(targets));
//        }
//        else
//        {
//            Debug.Log("범위 내 적 감지되지 않음.");
//        }

//        // 박스 레이케스트 시각화
//        //Debug.DrawLine(boxCenter - boxHalfExtents, boxCenter + boxHalfExtents, Color.red, 1.0f);
//    }


//    private IEnumerator AttackRoutine(List<Transform> targets)
//    {
//        isAttacking = true; // 공격 중 상태로 설정
//        showGizmos = true; // Gizmos 표시
//        while (targets.Count > 0)
//        {
//            TowerAttack(targets); // 모든 타겟을 공격
//            yield return new WaitForSeconds(attackSpeed); // 공격 속도만큼 대기

//            // 타겟 리스트 갱신 (제거된 타겟 제거)
//            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);
//        }
//        lineRenderer.enabled = false; // LineRenderer 비활성화
//        showGizmos = false; // Gizmos 숨김
//        isAttacking = false; // 공격 중 상태 해제
//    }
//    public override void TowerAttack(List<Transform> targets)
//    {
//        lineRenderer.enabled = true;
//        lineRenderer.SetPosition(0, laserStartPoint.position);
//        lineRenderer.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * laserLength);

//        foreach (var target in targets)
//        {
//            EnemyBase enemyHp = target.GetComponent<EnemyBase>();
//            if (enemyHp != null)
//            {
//                enemyHp.TakeDamage(towerAttackPower);
//                Debug.Log(target.name + towerAttackPower);
//            }
//            else
//            {
//                Debug.LogWarning(target.name);
//            }
//        }
//    }   

//    public override void SetRange(float range)
//    {
//        detectionRange = range; // 탐지 범위 설정
//        Debug.Log("탐지 범위 설정됨: " + detectionRange);
//    }

//    void OnDrawGizmosSelected()
//    {
//        if (showGizmos)
//        {
//            {
//                // 박스 형태의 Gizmos 시각화
//                Gizmos.color = Color.red;
//                Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (laserLength / 2);
//                Vector3 boxHalfExtents = new Vector3(0.2f, 0.2f, laserLength / 2);
//                Gizmos.DrawWireCube(boxCenter, boxHalfExtents * 2);
//            }
//        }
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LaserTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 10f;
//    [SerializeField] private Transform laserStartPoint;
//    private LineRenderer lineRenderer;
//    private bool isAttacking = false;
//    private bool showGizmos = false;
//    [SerializeField] private float laserLength = 10f;

//    void Start()
//    {
//        towerAttackPower = 50;
//        attackSpeed = 4f;
//        installationCost = 15;

//        SetRange(detectionRange);
//        lineRenderer = GetComponent<LineRenderer>();

//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>();
//        }
//    }

//    void Update()
//    {
//        DetectEnemiesInRange();
//    }

//    public override void DetectEnemiesInRange()
//    {
//        List<Transform> targets = new List<Transform>();

//        Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
//        Vector3 boxHalfExtents = new Vector3(0.6f, 0.6f, detectionRange / 2);

//        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, laserStartPoint.rotation);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                targets.Add(hitCollider.transform);
//                Debug.Log("박스 레이케스트로 적 감지됨: " + hitCollider.transform.name);
//            }
//        }

//        if (targets.Count > 0)
//        {
//            StartCoroutine(AttackRoutine(targets));
//        }
//        else
//        {
//            Debug.Log("범위 내 적 감지되지 않음.");
//        }

//        //Debug.DrawRay(laserStartPoint.position, laserStartPoint.forward * laserLength, Color.red, 1.0f);
//        //Gizmos.color = Color.red;
//        //Gizmos.DrawWireCube(boxCenter, boxHalfExtents);
//        Debug.DrawLine(boxCenter - boxHalfExtents, boxCenter + boxHalfExtents, Color.red, 1.0f);
//    }

//    private IEnumerator AttackRoutine(List<Transform> targets)
//    {
//        isAttacking = true;
//        showGizmos = true;
//        while (targets.Count > 0)
//        {
//            TowerAttack(targets);
//            yield return new WaitForSeconds(attackSpeed);

//            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);
//        }
//        lineRenderer.enabled = false;
//        showGizmos = false;
//        isAttacking = false;
//    }

//    public override void TowerAttack(List<Transform> targets)
//    {
//        lineRenderer.enabled = true;
//        lineRenderer.SetPosition(0, laserStartPoint.position);
//        lineRenderer.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * laserLength);

//        foreach (var target in targets)
//        {
//            EnemyBase enemyHp = target.GetComponent<EnemyBase>();
//            if (enemyHp != null)
//            {
//                enemyHp.TakeDamage(towerAttackPower);
//                Debug.Log("적 " + target.name + "에게 피해를 입혔습니다! 데미지: " + towerAttackPower);
//            }
//            else
//            {
//                Debug.LogWarning("EnemyBase 컴포넌트를 찾을 수 없습니다: " + target.name);
//            }
//        }
//    }

//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//        Debug.Log("탐지 범위 설정: " + detectionRange);
//    }

//    private void OnDrawGizmos()
//    {
//        if (showGizmos)
//        {
//            Gizmos.color = Color.red;
//            Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (laserLength / 2);
//            Vector3 boxSize = new Vector3(0.6f, 0.6f, laserLength); // 박스의 크기를 설정
//            Gizmos.DrawWireCube(boxCenter, boxSize);
//        }
//    }
//}
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LaserTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 10f;
//    [SerializeField] private Transform laserStartPoint;
//    private bool isAttacking = false;
//    [SerializeField] private float laserLength = 10f;

//    void Start()
//    {
//        towerAttackPower = 50;
//        attackSpeed = 4f;
//        installationCost = 15;

//        SetRange(detectionRange);
//    }

//    void Update()
//    {
//        DetectEnemiesInRange();
//    }

//    public override void DetectEnemiesInRange()
//    {
//        List<Transform> targets = new List<Transform>();

//        Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
//        Vector3 boxHalfExtents = new Vector3(0.2f, 0.2f, detectionRange / 2);

//        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, laserStartPoint.rotation);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                targets.Add(hitCollider.transform);
//                Debug.Log("박스 레이캐스트로 적 감지됨: " + hitCollider.transform.name);
//            }
//        }

//        if (targets.Count > 0)
//        {
//            StartCoroutine(AttackRoutine(targets));
//        }
//        else
//        {
//            Debug.Log("범위 내 적 감지되지 않음.");
//        }
//    }

//    private IEnumerator AttackRoutine(List<Transform> targets)
//    {
//        isAttacking = true;
//        while (targets.Count > 0)
//        {
//            TowerAttack(targets);
//            yield return new WaitForSeconds(attackSpeed);

//            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);
//        }
//        isAttacking = false;
//    }

//    public override void TowerAttack(List<Transform> targets)
//    {
//        foreach (var target in targets)
//        {
//            EnemyBase enemyHp = target.GetComponent<EnemyBase>();
//            if (enemyHp != null)
//            {
//                enemyHp.TakeDamage(towerAttackPower);
//                Debug.Log("적 " + target.name + "에게 피해를 입혔습니다! 데미지: " + towerAttackPower);
//            }
//            else
//            {
//                Debug.LogWarning("EnemyBase 컴포넌트를 찾을 수 없습니다: " + target.name);
//            }
//        }
//    }

//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//        Debug.Log("탐지 범위 설정: " + detectionRange);
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f;//레이저 길이
    [SerializeField] private Transform laserStartPoint;
    private bool isAttacking = false;
    

    void Awake()
    {
        towerAttackPower = 100;
        attackSpeed = 4f;
        installationCost = 15;

        SetRange(detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange();
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>();

        Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
        Vector3 boxHalfExtents = new Vector3(1f, 1f, detectionRange / 2);

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, laserStartPoint.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform);
                
            }
        }

        if (targets.Count > 0&& !isAttacking)
        {
            StartCoroutine(AttackRoutine(targets));
        }
        
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets);
            
            yield return new WaitForSeconds(attackSpeed);
            
            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);
        }
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            EnemyBase enemyHp = target.GetComponent<EnemyBase>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(towerAttackPower);
                Debug.Log("적 " + target.name + "에게 피해를 입혔습니다! 데미지: " + towerAttackPower);
            }
            
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        
    }

    private void OnDrawGizmos()
    {
        if (laserStartPoint != null)
        {
            Gizmos.color = Color.black;
            Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
            Vector3 boxSize = new Vector3(1f, 1f, detectionRange);
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, laserStartPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
}
