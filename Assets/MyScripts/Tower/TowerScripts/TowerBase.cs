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
    [SerializeField] protected Material rangeMaterial; // Inspector���� �Ҵ��� Material
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected Color rangeColor = Color.red;

    public enum RangeType
    {
        Circle,      // ����
        Rectangle,   // ���簢��
        Fan          // ��ä��
    }

    [SerializeField] protected RangeType rangeType = RangeType.Circle;
    [SerializeField] protected float rangeWidth = 4f;  // ���簢�� ��

    protected DecalProjector rangeDecal;
    protected bool isRangeVisible = false;

    protected virtual void Start()
    {
        SetupRangeDecal(); // �̰͸� ȣ��
    }

    protected virtual void SetupRangeDecal()
    {
        // ���� ǥ�� ���� GameObject ����
        GameObject rangeDisplayObject = new GameObject($"{gameObject.name}_RangeDisplay");
        rangeDisplayObject.transform.SetParent(transform);

        // ��ġ�� ȸ�� ����
        rangeDisplayObject.transform.localPosition = Vector3.up * 2.5f;
        rangeDisplayObject.transform.localRotation = Quaternion.Euler(90, 0, 0);

        // Decal Projector �߰�
        rangeDecal = rangeDisplayObject.AddComponent<DecalProjector>();
        rangeDecal.enabled = false;

        // Inspector���� �Ҵ��� Material ���
        if (rangeMaterial != null)
        {
            // Material ���纻 ���� (���� ���� ������ ����)
            Material instanceMaterial = new Material(rangeMaterial);
            instanceMaterial.color = new Color(rangeColor.r, rangeColor.g, rangeColor.b, 0.3f);
            rangeDecal.material = instanceMaterial;

            Debug.Log($"Material �Ҵ� �Ϸ�: {rangeMaterial.name}");
        }
        else
        {
            Debug.LogError($"Range Material�� �Ҵ���� �ʾҽ��ϴ�: {gameObject.name}");

            // �ӽ� Material ���� (fallback)
            CreateFallbackMaterial();
        }

        // ���� Ÿ�Կ� ���� ũ�� ����
        SetDecalSize();
    }

    // Material�� ���� �� �ӽ÷� ����
    private void CreateFallbackMaterial()
    {
        Material fallbackMaterial = new Material(Shader.Find("Unlit/Transparent"));
        fallbackMaterial.color = new Color(rangeColor.r, rangeColor.g, rangeColor.b, 0.3f);
        rangeDecal.material = fallbackMaterial;
        Debug.LogWarning($"�ӽ� Material ���: {gameObject.name}");
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
                // Fan�� ��� Ÿ�� ���⿡ �°� ȸ�� �ʿ�
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
            Debug.Log($"���� ǥ��: {gameObject.name}");
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
        Debug.Log($"Ÿ�� Ŭ����: {gameObject.name}");

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

    // �߻� �޼����
    public abstract void TowerPowUp();
    public abstract void TowerAttack(List<Transform> targets);
    public abstract void SetRange(float range);
    public abstract void DetectEnemiesInRange();
}