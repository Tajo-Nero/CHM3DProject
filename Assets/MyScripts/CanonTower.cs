using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f;//�ϴ��� 10���� �����ϰ� ���߿� ��ŸƮ�� ����
    private Transform target;//Ÿ���� �ٶ󺸰� ��
    private bool isAttacking = false;//�����Ǵ�
    [SerializeField] private Transform cannonTowerTransform;//ĳ��Ÿ���� ȸ���� �Ѳ� ���� �ٶ󺸰� �Ұ���

    void Start()
    {
        towerAttackPower = 40;
        towerPenetrationPower = 25;
        criticalHitRate = 0.05f;
        attackSpeed = 0.7f;
        installationCost = 8;

        SetRange(detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange();        
        if (target != null)
        {
            RotateTowardsTarget();
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        while (target != null)
        {
            TowerAttack();
            yield return new WaitForSeconds(attackSpeed);
        }
        isAttacking = false;
    }

    public override void TowerAttack()
    {
        if (target != null)
        {
            EnemyHp enemyHp = target.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(towerAttackPower);
                Debug.Log("ĳ�� Ÿ���� " + target.name + "��(��) �����մϴ�! ���ݷ�: " + towerAttackPower);
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("Ž�� ���� ������: " + detectionRange);
    }

    public override void DetectEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                target = hitCollider.transform;
                Debug.Log("�� �߰�: " + hitCollider.name);
                break;
            }
        }

        if (hitColliders.Length == 0)
        {
            target = null;
        }
    }
    private void RotateTowardsTarget()
    {
        Vector3 direction = (target.position - cannonTowerTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        cannonTowerTransform.rotation = Quaternion.Slerp(cannonTowerTransform.rotation, lookRotation, Time.deltaTime * 5f);
        // ���� �ٶ󺸾��� �� ���� ����
        if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
