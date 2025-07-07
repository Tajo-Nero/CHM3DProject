using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public abstract class TowerBase : MonoBehaviour
{
    [Header("Tower Stats")]
    public float towerAttackPower;
    public float towerPenetrationPower;
    public float criticalHitRate;
    public float attackSpeed;
    public int installationCost;
    public bool isAttackUp = false;

    [Header("Range Display")]
    [SerializeField] protected Material rangeMaterial; // Inspector에서 할당할 Material
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected Color rangeColor = Color.red;

    public enum RangeType
    {
        Circle,      // 원형
        Rectangle,   // 직사각형
        Fan          // 부채꼴
    }

    [SerializeField] protected RangeType rangeType = RangeType.Circle;
    [SerializeField] protected float rangeWidth = 4f;  // 직사각형 폭

    protected DecalProjector rangeDecal;
    protected bool isRangeVisible = false;

    protected virtual void Start()
    {
        SetupRangeDecal(); // 이것만 호출
    }

    protected virtual void SetupRangeDecal()
    {
        // 범위 표시 전용 GameObject 생성
        GameObject rangeDisplayObject = new GameObject($"{gameObject.name}_RangeDisplay");
        rangeDisplayObject.transform.SetParent(transform);

        // 위치와 회전 설정
        rangeDisplayObject.transform.localPosition = Vector3.up * 2.5f;
        rangeDisplayObject.transform.localRotation = Quaternion.Euler(90, 0, 0);

        // Decal Projector 추가
        rangeDecal = rangeDisplayObject.AddComponent<DecalProjector>();
        rangeDecal.enabled = false;

        // Inspector에서 할당한 Material 사용
        if (rangeMaterial != null)
        {
            // Material 복사본 생성 (색상 개별 조정을 위해)
            Material instanceMaterial = new Material(rangeMaterial);
            instanceMaterial.color = new Color(rangeColor.r, rangeColor.g, rangeColor.b, 0.3f);
            rangeDecal.material = instanceMaterial;

            Debug.Log($"Material 할당 완료: {rangeMaterial.name}");
        }
        else
        {
            Debug.LogError($"Range Material이 할당되지 않았습니다: {gameObject.name}");

            // 임시 Material 생성 (fallback)
            CreateFallbackMaterial();
        }

        // 범위 타입에 따라 크기 설정
        SetDecalSize();
    }

    // Material이 없을 때 임시로 생성
    private void CreateFallbackMaterial()
    {
        Material fallbackMaterial = new Material(Shader.Find("Unlit/Transparent"));
        fallbackMaterial.color = new Color(rangeColor.r, rangeColor.g, rangeColor.b, 0.3f);
        rangeDecal.material = fallbackMaterial;
        Debug.LogWarning($"임시 Material 사용: {gameObject.name}");
    }

    protected virtual void SetDecalSize()
    {
        if (rangeDecal == null) return;

        switch (rangeType)
        {
            case RangeType.Circle:
                rangeDecal.size = new Vector3(detectionRange * 2, detectionRange * 2, 15f);
                break;

            case RangeType.Rectangle:
                rangeDecal.size = new Vector3(rangeWidth, detectionRange, 15f);
                break;

            case RangeType.Fan:
                rangeDecal.size = new Vector3(detectionRange * 2, detectionRange * 2, 15f);
                // Fan의 경우 타워 방향에 맞게 회전 필요
                rangeDecal.transform.localRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
                break;
        }
    }

    public virtual void ShowRange()
    {
        if (rangeDecal != null)
        {
            rangeDecal.enabled = true;
            isRangeVisible = true;
            Debug.Log($"범위 표시: {gameObject.name}");
        }
    }

    public virtual void HideRange()
    {
        if (rangeDecal != null)
        {
            rangeDecal.enabled = false;
            isRangeVisible = false;
        }
    }

    public virtual void SetRangeSize(float newRange)
    {
        detectionRange = newRange;
        if (rangeDecal != null)
        {
            SetDecalSize();
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"타워 클릭됨: {gameObject.name}");

        if (TowerSelectionManager.Instance != null)
        {
            TowerSelectionManager.Instance.SelectTower(this);
        }
        else
        {
            ToggleRange();
        }
    }

    protected void ToggleRange()
    {
        if (isRangeVisible)
            HideRange();
        else
            ShowRange();
    }

    // 추상 메서드들
    public abstract void TowerPowUp();
    public abstract void TowerAttack(List<Transform> targets);
    public abstract void SetRange(float range);
    public abstract void DetectEnemiesInRange();
}