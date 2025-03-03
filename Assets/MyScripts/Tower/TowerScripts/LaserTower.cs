using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 20f; // Ž�� �Ÿ�
    [SerializeField] private Transform laserStartPoint; // ������ ������
    [SerializeField] private ParticleSystem laserImpactEffectPrefab; // ������ �浹 ��ƼŬ ȿ�� ������
    private ILineRendererStrategy lineRendererStrategy;
    private bool isAttacking = false; // ���� ������ ����

    void Awake()
    {
        towerAttackPower = 50; // ���ݷ� ����
        attackSpeed = 1f; // ���� �ӵ� ����
        installationCost = 15; // ��ġ ��� ����
    }

    void Start()
    {
        SetRange(detectionRange); // Ž�� �Ÿ� ����

        // ���� ������ ������ �ʱ�ȭ
        lineRendererStrategy = new LaserRendererStrategy();
        lineRendererStrategy.Setup(gameObject);
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, laserStartPoint, 4, detectionRange, detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange(); // �� Ž��
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>();

        // �ڱ� �ڽ��� ��ġ���� Ž�� �ڽ� ����
        Vector3 boxCenter = transform.position + transform.forward * (detectionRange / 2);
        Vector3 boxHalfExtents = new Vector3(2f, 2f, detectionRange / 2);
        Quaternion boxOrientation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        // �ڽ� ĳ��Ʈ�� �� Ž��
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter, boxHalfExtents, transform.forward, boxOrientation, detectionRange);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                targets.Add(hit.transform);
                Debug.Log("Detected Enemy: " + hit.transform.name);
            }
        }

        if (targets.Count > 0 && !isAttacking)
        {
            StartCoroutine(AttackRoutine(targets)); // ���� ��ƾ ����
        }
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets); // Ÿ�� ����
            yield return new WaitForSeconds(attackSpeed); // ���� ��� �ð�

            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf); // ��Ȱ��ȭ�� Ÿ�� ����
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
                enemyHp.TakeDamage(towerAttackPower); // ������ ������ ����
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
        yield return new WaitForSeconds(0.5f); // ��ƼŬ ȿ�� ��� �ð�
        laserImpactEffect.Stop();
        Destroy(laserImpactEffect.gameObject); // ��ƼŬ ������Ʈ �ı�
    }

    public override void SetRange(float range)
    {
        detectionRange = range; // Ž�� �Ÿ� ����
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2; // ���ݷ� �� ��� ����
        Debug.Log("�Ŀ���! Ÿ���� ���ݷ��� �� ��� �����߽��ϴ�: " + towerAttackPower);
    }

    // ����� �׸��� �Լ�
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 boxCenter = transform.position + transform.forward * (detectionRange / 2);
            Vector3 boxHalfExtents = new Vector3(2f, 2f, detectionRange / 2);
            Quaternion boxOrientation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            Gizmos.color = Color.red; // ����� ���� ����
            Gizmos.DrawWireCube(boxCenter, boxHalfExtents * 2); // �ڽ� ĳ��Ʈ ����� ����� �׸���
        }
    }
}
