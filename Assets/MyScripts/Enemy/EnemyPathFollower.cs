using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPathFollower : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData;
    public float enemy_attackDamage;
    public float enemy_attackSpeed;
    public float enemy_health;

    [Header("Movement")]
    public float moveSpeed = 5f; // 기본값 설정
    public float rotationSpeed = 360f;
    public float waypointReachDistance = 1f;

    private MyHealthBar healthBar;
    private bool isAttacking = false;
    public List<Vector3> currentPath;
    public int currentWaypointIndex = 0;
    private GameObject target;
    private EnemyPool enemyPool;
    private TowerGenerator towerGenerator;
    private MyHealthBar myHealthBar;
    public GameObject healthBarPrefab;
    protected Animator animator;

    public delegate void DeathHandler(GameObject enemy);
    public static event DeathHandler OnAnyEnemyDeath;

    protected virtual void Awake()
    {
        InitializeComponents();
        InitializeEnemyData();

        // 자식에서 체력바 찾기
        healthBar = GetComponentInChildren<MyHealthBar>();

        if (healthBar == null)
        {
            Debug.LogWarning($"{gameObject.name}에 체력바가 없습니다!");
        }
    }
    protected virtual void Start()
    {
        // 체력바 초기화
        if (healthBar != null && enemyData != null)
        {
            healthBar.Initialize(enemyData.health);
            // 강제로 한 번 더 위치 업데이트
            healthBar.transform.position = transform.position + healthBar.offset;
        }
    }

    void OnEnable()
    {
        // 재활성화될 때 경로가 없으면 초기화
        currentPath = null;
        currentWaypointIndex = 0;
    }
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        enemyPool = FindObjectOfType<EnemyPool>();
        target = FindObjectOfType<Nexus>()?.gameObject;
        towerGenerator = FindObjectOfType<TowerGenerator>();
    }

    private void InitializeEnemyData()
    {
        if (enemyData != null)
        {
            enemy_attackDamage = enemyData.attackPower;
            enemy_health = enemyData.health;

            // moveSpeed 설정 (0이면 기본값 사용)
            if (enemyData.moveSpeed > 0)
            {
                moveSpeed = enemyData.moveSpeed;
            }

        }
        else
        {
            Debug.LogWarning($"EnemyData가 설정되지 않았습니다! 기본값 사용 - 속도: {moveSpeed}");
        }
    }


    public void SetPath(List<Vector3> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogError($"{gameObject.name}: 빈 경로가 전달되었습니다!");
            return;
        }

        currentPath = new List<Vector3>(waypoints);
        currentWaypointIndex = 0;

        Debug.Log($"{gameObject.name}: 경로 설정 완료 - {currentPath.Count}개 지점");
    }

    public void InitializeHealth()
    {
        if (enemyData != null)
        {
            enemy_health = enemyData.health;
        }

        if (myHealthBar == null && healthBarPrefab != null)
        {
            GameObject healthBar = Instantiate(healthBarPrefab, transform);
            myHealthBar = healthBar.GetComponent<MyHealthBar>();
        }

        if (myHealthBar != null)
        {
            myHealthBar.Initialize(enemy_health);
        }
    }

    protected virtual void Update()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            return;
        }

        MoveAlongPath();
        CheckForAttack();
    }

    void MoveAlongPath()
    {
        if (currentWaypointIndex >= currentPath.Count)
        {
            Debug.Log($"모든 웨이포인트에 도달했습니다: {gameObject.name}");
            return;
        }

        Vector3 targetWaypoint = currentPath[currentWaypointIndex];
        Vector3 direction = (targetWaypoint - transform.position).normalized;

        // 이동 거리 계산
        float moveDistance = moveSpeed * Time.deltaTime;

        // 이동
        transform.position += direction * moveDistance;

        // 회전
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 애니메이션 업데이트
        if (animator != null)
        {
            animator.SetFloat("Run", moveSpeed);
        }

        // 웨이포인트 도달 체크
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint);
        if (distanceToWaypoint < waypointReachDistance)
        {
            currentWaypointIndex++;
            Debug.Log($"웨이포인트 {currentWaypointIndex}/{currentPath.Count} 도달");
        }

        // 디버그용 - 현재 목표 웨이포인트 표시
        Debug.DrawLine(transform.position, targetWaypoint, Color.red, 0.1f);
    }

    void CheckForAttack()
    {
        if (target == null || isAttacking) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget <= 2f) // 공격 거리
        {
            StartCoroutine(AttackTarget());
        }
    }

    private IEnumerator AttackTarget()
    {
        isAttacking = true;
        Debug.Log($"{gameObject.name}이(가) 넥서스를 공격합니다!");

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        while (Vector3.Distance(transform.position, target.transform.position) <= 2f)
        {
            if (target != null)
            {
                Nexus targetNexus = target.GetComponent<Nexus>();
                if (targetNexus != null)
                {
                    targetNexus.TakeDamage(enemy_attackDamage);
                }
            }
            yield return new WaitForSeconds(enemy_attackSpeed);
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        enemy_health -= damage;
        Debug.Log($"{gameObject.name}이(가) {damage} 데미지를 받았습니다. 남은 체력: {enemy_health}");

        // 체력바 업데이트 - healthBar 사용 (myHealthBar 아님!)
        if (healthBar != null)
        {
            healthBar.TakeDamage(damage); // UpdateHealth 대신 TakeDamage 사용
        }

        if (enemy_health <= 0)
        {
            Die();
        }
    }

    public float GetCurrentHealth()
    {
        return enemy_health;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 죽었습니다!");

        // 죽음 이벤트 발생
        OnAnyEnemyDeath?.Invoke(gameObject);

        if (towerGenerator != null)
        {
            towerGenerator.NotifyObservers(gameObject, "EnemyDefeated");
        }

        if (enemyPool != null)
        {
            enemyPool.ReturnEnemy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Victory()
    {
        if (animator != null)
        {
            animator.SetBool("isVictory", true);
            animator.SetTrigger("Attack");
        }
    }

    // 디버그용 - 경로 시각화
    void OnDrawGizmos()
    {
        if (currentPath != null && currentPath.Count > 1)
        {
            Gizmos.color = Color.blue;

            // 현재 위치에서 다음 웨이포인트까지 선 그리기
            if (currentWaypointIndex < currentPath.Count)
            {
                Gizmos.DrawLine(transform.position, currentPath[currentWaypointIndex]);
            }

            // 전체 경로 그리기
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                if (i >= currentWaypointIndex)
                {
                    Gizmos.color = Color.yellow; // 아직 가지 않은 경로
                }
                else
                {
                    Gizmos.color = Color.gray; // 이미 지나간 경로
                }

                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
                Gizmos.DrawWireSphere(currentPath[i], 0.3f);
            }

            // 마지막 웨이포인트
            if (currentPath.Count > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(currentPath[currentPath.Count - 1], 0.5f);
            }
        }
    }
}