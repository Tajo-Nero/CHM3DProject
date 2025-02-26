using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[System.Serializable]
public class TowerCraft
{
    public string _craftName;
    public GameObject _TowerPrefab;
    public GameObject _PreviewPrefab;
}

public class TowerGenerator : MonoBehaviour
{
    [SerializeField]
    private TowerCraft[] _CraftTower; // �����յ� ����� Ŭ����
    [SerializeField]
    private Transform _Player; // �÷��̾� �տ� Ÿ�� ��ġ�� �Ŷ� �÷��̾� ��ǥ �޾ƿ�
    [SerializeField]
    private Material[] _Materials; // ��ġ ������ ���� �ʷϻ� ���׸���� ��ġ �Ұ����� ���� ������ ���׸����� �迭�� ����
    [SerializeField]
    private GameObject currentPreview;
    private int currentPreviewIndex = -1;
    private bool canPlaceTower = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreatePreview(0); // 1�� Ű�� ������ ĳ�� ������ ����
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreatePreview(1); // 2�� Ű�� ������ ������ ������ ����
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CreatePreview(2); // 3�� Ű�� ������ ���� ������ ����
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CreatePreview(3);
        }

        if (Input.GetMouseButtonDown(0) && currentPreview != null && canPlaceTower)
        {
            {
                PlaceTower(currentPreviewIndex); // ���콺�� Ŭ���ϸ� ���� Ÿ�� ����
            }
        }

        void CreatePreview(int index)
        {
            if (index >= 0 && index < _CraftTower.Length)
            {
                if (currentPreview != null)
                {
                    Destroy(currentPreview); // ������ �����䰡 ���� ��� ����
                }
                currentPreview = Instantiate(_CraftTower[index]._PreviewPrefab, _Player.position + _Player.forward, _Player.rotation, _Player.transform);
                Renderer renderer = currentPreview.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = _Materials[0]; // �ʱ� ���׸����� �ʷϻ����� ����
                }
                else
                {
                    // �ڽ� ������Ʈ���� Renderer�� ã�� ����
                    Renderer childRenderer = currentPreview.GetComponentInChildren<Renderer>();
                    if (childRenderer != null)
                    {
                        childRenderer.material = _Materials[0]; // �ʱ� ���׸����� �ʷϻ����� ����
                    }
                }
                currentPreview.AddComponent<PreviewCollisionDetector>().Setup(this, _Materials);
                currentPreviewIndex = index; // ���� �������� �ε��� ����
            }
        }

        void PlaceTower(int index)
        {
            if (index >= 0 && index < _CraftTower.Length)
            {
                Instantiate(_CraftTower[index]._TowerPrefab, _Player.position + _Player.forward, _Player.rotation);
                Destroy(currentPreview); // Ÿ���� ��ġ�� �� ������ ����
                currentPreview = null; // ���� ������ ���� �ʱ�ȭ
                currentPreviewIndex = -1; // ���� ������ �ε��� �ʱ�ȭ
            }

        }
    }
    public void SetCanPlaceTower(bool value)
    {
        canPlaceTower = value;

    }
}
public class PreviewCollisionDetector : MonoBehaviour
{
    private TowerGenerator towerManager;
    private Material[] materials;
    private bool isCollidingWithTower = false;
    private bool isCollidingWithTerrain = false;

    public void Setup(TowerGenerator manager, Material[] mats)
    {
        towerManager = manager;
        materials = mats;
        CheckPlacementValidity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Towers"))
        {
            Debug.Log("�浹 ����: " + other.name);
            isCollidingWithTower = true;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("Floor"))
        {
            Debug.Log("�ͷ��� ����: " + other.name);
            isCollidingWithTerrain = true;
            CheckPlacementValidity();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Towers"))
        {
            Debug.Log("�浹 ����: " + other.name);
            isCollidingWithTower = false;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("Floor"))
        {
            Debug.Log("�ͷ��� ����: " + other.name);
            isCollidingWithTerrain = false;
            CheckPlacementValidity();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            isCollidingWithTerrain = true;
        }
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
        if (isCollidingWithTerrain && !isCollidingWithTower)
        {
            SetMaterial(materials[0]); // �ʷϻ� ���׸���� ����
            towerManager.SetCanPlaceTower(true);
        }
        else
        {
            SetMaterial(materials[1]); // ������ ���׸���� ����
            towerManager.SetCanPlaceTower(false);
        }
    }
}