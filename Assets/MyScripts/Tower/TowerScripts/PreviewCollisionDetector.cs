using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollisionDetector : MonoBehaviour, IObserver
{
    private TowerGenerator towerManager;
    private Material[] materials;
    private TowerRangeDisplay rangeDisplay;

    // 충돌 카운터
    private int towerCollisionCount = 0;
    private int powerUpCollisionCount = 0;

    // Raycast 설정
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private LayerMask floorLayerMask = -1; // Inspector에서 설정

    public void Setup(TowerGenerator manager, Material[] mats)
    {
        towerManager = manager;
        materials = mats;
        towerManager.AddObserver(this);

        // 범위 표시 컴포넌트 찾기
        rangeDisplay = GetComponent<TowerRangeDisplay>();

        // Floor 레이어가 설정되지 않았으면 자동 설정
        if (floorLayerMask == -1)
        {
            floorLayerMask = LayerMask.GetMask("Floor");
        }

        CheckPlacementValidity();

        // Material 확실히 설정
        StartCoroutine(EnsureMaterialSetup());
    }

    void OnDestroy()
    {
        if (towerManager != null)
        {
            towerManager.RemoveObserver(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Towers"))
        {
            towerCollisionCount++;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("TowerPowUp"))
        {
            powerUpCollisionCount++;
            CheckPlacementValidity();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Towers"))
        {
            towerCollisionCount = Mathf.Max(0, towerCollisionCount - 1);
            CheckPlacementValidity();
        }
        else if (other.CompareTag("TowerPowUp"))
        {
            powerUpCollisionCount = Mathf.Max(0, powerUpCollisionCount - 1);
            CheckPlacementValidity();
        }
    }

    public void OnNotify(GameObject obj, string eventMessage)
    {
        if (eventMessage == "PreviewCreated" && obj == gameObject)
        {
            CheckPlacementValidity();
        }
        else if (eventMessage == "TowerPlaced" && obj == gameObject)
        {
            CheckPlacementValidity();
        }
    }

    private bool CheckGroundBelow()
    {
        // 여러 지점에서 체크 (중앙 + 모서리)
        Vector3[] checkPoints = new Vector3[]
        {
            transform.position,                          // 중앙
            transform.position + Vector3.forward * 0.5f, // 앞
            transform.position - Vector3.forward * 0.5f, // 뒤
            transform.position + Vector3.right * 0.5f,   // 오른쪽
            transform.position - Vector3.right * 0.5f    // 왼쪽
        };

        foreach (Vector3 point in checkPoints)
        {
            Ray ray = new Ray(point + Vector3.up * 0.5f, Vector3.down);

            if (Physics.Raycast(ray, groundCheckDistance, floorLayerMask))
            {
                return true; // 하나라도 바닥에 닿으면 OK
            }
        }

        return false;
    }

    private void SetMaterial(Material material)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
    }

    private void CheckPlacementValidity()
    {
        // Raycast로 바닥 체크
        bool isOnGround = CheckGroundBelow();
        bool hasNoTowerConflict = towerCollisionCount == 0;
        bool isOnPowerUp = powerUpCollisionCount > 0;

        // 설치 가능 조건: 바닥에 있고 (타워와 겹치지 않거나 파워업 위치)
        bool canPlace = isOnGround && (hasNoTowerConflict || isOnPowerUp);

        if (canPlace)
        {
            // 설치 가능 - 초록색
            SetMaterial(materials[0]);
            towerManager.SetCanPlaceTower(true);
            UpdateRangeMaterial(materials[0]);

            // 파워업 효과 적용 여부
            towerManager.canApplyAttackUp = isOnPowerUp;
        }
        else
        {
            // 설치 불가 - 빨간색
            SetMaterial(materials[1]);
            towerManager.SetCanPlaceTower(false);
            UpdateRangeMaterial(materials[1]);
        }
    }

    // 범위 표시 Material 업데이트
    private void UpdateRangeMaterial(Material mat)
    {
        if (rangeDisplay != null && rangeDisplay.rangeObject != null)
        {
            MeshRenderer meshRenderer = rangeDisplay.rangeObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = mat;
            }
        }
    }

    // Material 설정 보장
    IEnumerator EnsureMaterialSetup()
    {
        // RangeDisplay가 생성될 때까지 대기
        while (rangeDisplay == null)
        {
            rangeDisplay = GetComponent<TowerRangeDisplay>();
            yield return null;
        }

        // rangeObject가 생성될 때까지 대기
        while (rangeDisplay.rangeObject == null)
        {
            yield return null;
        }

        // 초기 상태 다시 체크
        CheckPlacementValidity();
    }

    // 디버그용 Gizmo
    void OnDrawGizmos()
    {
        // 바닥 체크 Ray 시각화
        Gizmos.color = CheckGroundBelow() ? Color.green : Color.red;

        Vector3[] checkPoints = new Vector3[]
        {
            transform.position,
            transform.position + Vector3.forward * 0.5f,
            transform.position - Vector3.forward * 0.5f,
            transform.position + Vector3.right * 0.5f,
            transform.position - Vector3.right * 0.5f
        };

        foreach (Vector3 point in checkPoints)
        {
            Gizmos.DrawLine(point + Vector3.up * 0.5f, point + Vector3.down * (groundCheckDistance - 0.5f));
        }
    }
}