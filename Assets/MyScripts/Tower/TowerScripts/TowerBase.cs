using UnityEngine;
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

    [Header("Range Settings")]
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected Material rangeMaterial;

    // 범위 표시 컴포넌트
    protected TowerRangeDisplay rangeDisplay;
    protected bool isRangeVisible = false;

    protected virtual void Start()
    {
        // 범위 표시 컴포넌트 추가
        SetupRangeDisplay();
    }

    protected virtual void SetupRangeDisplay()
    {
        // 이미 있으면 추가하지 않음
        rangeDisplay = GetComponent<TowerRangeDisplay>();

        if (rangeDisplay == null)
        {
            rangeDisplay = gameObject.AddComponent<TowerRangeDisplay>();
            rangeDisplay.range = detectionRange;
            rangeDisplay.rangeMaterial = rangeMaterial;
            rangeDisplay.ShowRange(false);
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
        if (rangeDisplay != null)
        {
            isRangeVisible = !isRangeVisible;
            rangeDisplay.ShowRange(isRangeVisible);
        }
    }

    public virtual void ShowRange()
    {
        if (rangeDisplay != null)
        {
            rangeDisplay.ShowRange(true);
            isRangeVisible = true;
        }
    }

    public virtual void HideRange()
    {
        if (rangeDisplay != null)
        {
            rangeDisplay.ShowRange(false);
            isRangeVisible = false;
        }
    }

    public virtual void SetRangeSize(float newRange)
    {
        detectionRange = newRange;
        if (rangeDisplay != null)
        {
            rangeDisplay.range = newRange;
            rangeDisplay.UpdateRangeMesh();
        }
    }

    // 추상 메서드들
    public abstract void TowerPowUp();
    public abstract void TowerAttack(List<Transform> targets);
    public abstract void SetRange(float range);
    public abstract void DetectEnemiesInRange();
}