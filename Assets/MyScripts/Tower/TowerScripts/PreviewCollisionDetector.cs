using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollisionDetector : MonoBehaviour, IObserver
{
    private TowerGenerator towerManager;
    private Material[] materials;
    private TowerRangeDisplay rangeDisplay;

    // �浹 ī����
    private int towerCollisionCount = 0;
    private int powerUpCollisionCount = 0;

    // Raycast ����
    [SerializeField] private float groundCheckDistance = 2f;
    [SerializeField] private LayerMask floorLayerMask = -1; // Inspector���� ����

    public void Setup(TowerGenerator manager, Material[] mats)
    {
        towerManager = manager;
        materials = mats;
        towerManager.AddObserver(this);

        // ���� ǥ�� ������Ʈ ã��
        rangeDisplay = GetComponent<TowerRangeDisplay>();

        // Floor ���̾ �������� �ʾ����� �ڵ� ����
        if (floorLayerMask == -1)
        {
            floorLayerMask = LayerMask.GetMask("Floor");
        }

        CheckPlacementValidity();

        // Material Ȯ���� ����
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
        // ���� �������� üũ (�߾� + �𼭸�)
        Vector3[] checkPoints = new Vector3[]
        {
            transform.position,                          // �߾�
            transform.position + Vector3.forward * 0.5f, // ��
            transform.position - Vector3.forward * 0.5f, // ��
            transform.position + Vector3.right * 0.5f,   // ������
            transform.position - Vector3.right * 0.5f    // ����
        };

        foreach (Vector3 point in checkPoints)
        {
            Ray ray = new Ray(point + Vector3.up * 0.5f, Vector3.down);

            if (Physics.Raycast(ray, groundCheckDistance, floorLayerMask))
            {
                return true; // �ϳ��� �ٴڿ� ������ OK
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
        // Raycast�� �ٴ� üũ
        bool isOnGround = CheckGroundBelow();
        bool hasNoTowerConflict = towerCollisionCount == 0;
        bool isOnPowerUp = powerUpCollisionCount > 0;

        // ��ġ ���� ����: �ٴڿ� �ְ� (Ÿ���� ��ġ�� �ʰų� �Ŀ��� ��ġ)
        bool canPlace = isOnGround && (hasNoTowerConflict || isOnPowerUp);

        if (canPlace)
        {
            // ��ġ ���� - �ʷϻ�
            SetMaterial(materials[0]);
            towerManager.SetCanPlaceTower(true);
            UpdateRangeMaterial(materials[0]);

            // �Ŀ��� ȿ�� ���� ����
            towerManager.canApplyAttackUp = isOnPowerUp;
        }
        else
        {
            // ��ġ �Ұ� - ������
            SetMaterial(materials[1]);
            towerManager.SetCanPlaceTower(false);
            UpdateRangeMaterial(materials[1]);
        }
    }

    // ���� ǥ�� Material ������Ʈ
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

    // Material ���� ����
    IEnumerator EnsureMaterialSetup()
    {
        // RangeDisplay�� ������ ������ ���
        while (rangeDisplay == null)
        {
            rangeDisplay = GetComponent<TowerRangeDisplay>();
            yield return null;
        }

        // rangeObject�� ������ ������ ���
        while (rangeDisplay.rangeObject == null)
        {
            yield return null;
        }

        // �ʱ� ���� �ٽ� üũ
        CheckPlacementValidity();
    }

    // ����׿� Gizmo
    void OnDrawGizmos()
    {
        // �ٴ� üũ Ray �ð�ȭ
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