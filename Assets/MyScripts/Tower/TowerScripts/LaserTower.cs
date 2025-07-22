using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private ParticleSystem laserImpactEffectPrefab;
    private bool isAttacking = false;

    void Awake()
    {
        towerAttackPower = 50;
        attackSpeed = 1f;
        installationCost = 15;
    }

    protected override void Start()
    {
        detectionRange = 20f;

        base.Start(); // TowerBase�� Start ȣ��

        // ������ Ÿ���� ���簢�� ����
        if (rangeDisplay != null)
        {
            rangeDisplay.shape = TowerRangeDisplay.RangeShape.Rectangle;
            rangeDisplay.rectangleWidth = 4f;
            rangeDisplay.UpdateRangeMesh();
        }

        SetRange(detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange();
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>();

        // ���簢�� ������ �°� ����
        float halfWidth = rangeDisplay != null ? rangeDisplay.rectangleWidth / 2f : 2f;
        Vector3 boxCenter = transform.position + transform.forward * (detectionRange / 2);
        Vector3 boxHalfExtents = new Vector3(halfWidth, 2f, detectionRange / 2);

        // Box ������ Ÿ�� ȸ���� ����
        Collider[] hitColliders = Physics.OverlapBox(
            boxCenter,
            boxHalfExtents,
            transform.rotation
        );

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform);
            }
        }

        if (targets.Count > 0 && !isAttacking)
        {
            StartCoroutine(AttackRoutine(targets));
        }
    }

    // ����׿� Gizmo �߰�
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        float halfWidth = rangeDisplay != null ? rangeDisplay.rectangleWidth / 2f : 2f;
        Vector3 boxCenter = transform.position + transform.forward * (detectionRange / 2);
        Vector3 boxSize = new Vector3(halfWidth * 2, 4f, detectionRange);

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
        Gizmos.matrix = oldMatrix;
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
            EnemyPathFollower enemyHp = target.GetComponent<EnemyPathFollower>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(towerAttackPower);
                Debug.Log("Laser hit " + target.name + " for " + towerAttackPower + " damage!");

                // ������ �浹 ��ƼŬ ȿ�� ���
                if (laserImpactEffectPrefab != null)
                {
                    ParticleSystem laserImpactEffect = Instantiate(laserImpactEffectPrefab, target.position, Quaternion.LookRotation(target.position - laserStartPoint.position));
                    StartCoroutine(PlayParticleEffect(laserImpactEffect));
                }
            }
        }
    }

    private IEnumerator PlayParticleEffect(ParticleSystem laserImpactEffect)
    {
        laserImpactEffect.Play();
        yield return new WaitForSeconds(0.5f);
        laserImpactEffect.Stop();
        Destroy(laserImpactEffect.gameObject);
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        SetRangeSize(range); // TowerBase�� �޼��� ȣ��
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2;
        Debug.Log("�Ŀ���! Ÿ���� ���ݷ��� �� ��� �����߽��ϴ�: " + towerAttackPower);
    }
}