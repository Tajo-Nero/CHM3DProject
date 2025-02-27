//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LaserTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 10f; // Ž�� ����
//    [SerializeField] private Transform laserStartPoint; // �������� ���۵Ǵ� ��ġ
//    private LineRenderer lineRenderer; // �������� �׸� LineRenderer
//    private bool isAttacking = false; // ���� �� ����
//    private bool showGizmos = false; // Gizmos ǥ�� ����
//    [SerializeField] private float laserLength = 10f; // ������ ����

//    void Start()
//    {
//        towerAttackPower = 50; // Ÿ���� ���ݷ� ����
//        attackSpeed = 4f; // ���� �ӵ� ����
//        installationCost = 15; // ��ġ ��� ����

//        SetRange(detectionRange); // Ž�� ���� ����
//        lineRenderer = GetComponent<LineRenderer>(); // LineRenderer ������Ʈ ����

//        // LineRenderer�� ������ �߰�
//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>();
//        }
//    }

//    void Update()
//    {
//        DetectEnemiesInRange(); // ���� �� �� Ž��
//    }

//    public override void DetectEnemiesInRange()
//    {
//        List<Transform> targets = new List<Transform>(); 

//        // �ڽ� �����ɽ�Ʈ�� ���� �߽����� ������ ����
//        Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (laserLength / 2);
//        Vector3 boxHalfExtents = new Vector3(0.2f, 0.2f, laserLength / 2);

//        // �ڽ� �����ɽ�Ʈ
//        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, laserStartPoint.rotation);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                targets.Add(hitCollider.transform); 
//                Debug.Log("�ڽ� �����ɽ�Ʈ�� �� ������: " + hitCollider.transform.name);
//            }
//        }

//        if (targets.Count > 0)
//        {

//            StartCoroutine(AttackRoutine(targets));
//        }
//        else
//        {
//            Debug.Log("���� �� �� �������� ����.");
//        }

//        // �ڽ� �����ɽ�Ʈ �ð�ȭ
//        //Debug.DrawLine(boxCenter - boxHalfExtents, boxCenter + boxHalfExtents, Color.red, 1.0f);
//    }


//    private IEnumerator AttackRoutine(List<Transform> targets)
//    {
//        isAttacking = true; // ���� �� ���·� ����
//        showGizmos = true; // Gizmos ǥ��
//        while (targets.Count > 0)
//        {
//            TowerAttack(targets); // ��� Ÿ���� ����
//            yield return new WaitForSeconds(attackSpeed); // ���� �ӵ���ŭ ���

//            // Ÿ�� ����Ʈ ���� (���ŵ� Ÿ�� ����)
//            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);
//        }
//        lineRenderer.enabled = false; // LineRenderer ��Ȱ��ȭ
//        showGizmos = false; // Gizmos ����
//        isAttacking = false; // ���� �� ���� ����
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
//        detectionRange = range; // Ž�� ���� ����
//        Debug.Log("Ž�� ���� ������: " + detectionRange);
//    }

//    void OnDrawGizmosSelected()
//    {
//        if (showGizmos)
//        {
//            {
//                // �ڽ� ������ Gizmos �ð�ȭ
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
//                Debug.Log("�ڽ� �����ɽ�Ʈ�� �� ������: " + hitCollider.transform.name);
//            }
//        }

//        if (targets.Count > 0)
//        {
//            StartCoroutine(AttackRoutine(targets));
//        }
//        else
//        {
//            Debug.Log("���� �� �� �������� ����.");
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
//                Debug.Log("�� " + target.name + "���� ���ظ� �������ϴ�! ������: " + towerAttackPower);
//            }
//            else
//            {
//                Debug.LogWarning("EnemyBase ������Ʈ�� ã�� �� �����ϴ�: " + target.name);
//            }
//        }
//    }

//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//        Debug.Log("Ž�� ���� ����: " + detectionRange);
//    }

//    private void OnDrawGizmos()
//    {
//        if (showGizmos)
//        {
//            Gizmos.color = Color.red;
//            Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (laserLength / 2);
//            Vector3 boxSize = new Vector3(0.6f, 0.6f, laserLength); // �ڽ��� ũ�⸦ ����
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
//                Debug.Log("�ڽ� ����ĳ��Ʈ�� �� ������: " + hitCollider.transform.name);
//            }
//        }

//        if (targets.Count > 0)
//        {
//            StartCoroutine(AttackRoutine(targets));
//        }
//        else
//        {
//            Debug.Log("���� �� �� �������� ����.");
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
//                Debug.Log("�� " + target.name + "���� ���ظ� �������ϴ�! ������: " + towerAttackPower);
//            }
//            else
//            {
//                Debug.LogWarning("EnemyBase ������Ʈ�� ã�� �� �����ϴ�: " + target.name);
//            }
//        }
//    }

//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//        Debug.Log("Ž�� ���� ����: " + detectionRange);
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f;//������ ����
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
                Debug.Log("�� " + target.name + "���� ���ظ� �������ϴ�! ������: " + towerAttackPower);
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
