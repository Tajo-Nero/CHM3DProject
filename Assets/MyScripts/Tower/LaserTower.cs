using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private Transform laserStartPoint;
    private LineRenderer lineRenderer;
    private bool isAttacking = false;
    [SerializeField] private float laserLength = 10f;
    private ILineRendererStrategy lineRendererStrategy;

    void Start()
    {
        towerAttackPower = 50;
        attackSpeed = 4f;
        installationCost = 15;

        SetRange(detectionRange);

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 원하는 전략을 초기화하고 설정
        lineRendererStrategy = new LaserRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = 5;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        DetectEnemiesInRange();
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>();

        Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
        Vector3 boxHalfExtents = new Vector3(2f, 2f, detectionRange / 2);

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, laserStartPoint.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform);
                Debug.Log("Detected Enemy: " + hitCollider.transform.name);
            }
        }

        if (targets.Count > 0 && !isAttacking)
        {
            StartCoroutine(AttackRoutine(targets));
        }

        // 패턴 생성
        lineRendererStrategy.GeneratePattern(lineRenderer, laserStartPoint.position, transform, 8, detectionRange, laserLength);
    
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            lineRenderer.enabled = true; // Enable line renderer
            TowerAttack(targets);

            yield return new WaitForSeconds(1.0f); // Show the laser for 1 second

            lineRenderer.enabled = false; // Disable line renderer
            yield return new WaitForSeconds(attackSpeed - 1.0f); // Wait for the rest of the attack speed

            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);
        }
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, laserStartPoint.position);
        lineRenderer.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * laserLength);

        foreach (var target in targets)
        {
            EnemyBase enemyHp = target.GetComponent<EnemyBase>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(towerAttackPower);
                Debug.Log("Laser hit " + target.name + " for " + towerAttackPower + " damage!");
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("Detection range set to: " + detectionRange);
    }

    private void OnDrawGizmos()
    {
        if (laserStartPoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
            Vector3 boxSize = new Vector3(2f, 2f, detectionRange);
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, laserStartPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
}
