using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollisionDetector : MonoBehaviour, IObserver
{
    private TowerGenerator towerManager;
    private Material[] materials;
    private bool isCollidingWithTower = false;
    private bool isCollidingWithTerrain = false;
    private bool isCollidingWithPowerUp = false;

    public void Setup(TowerGenerator manager, Material[] mats)
    {
        towerManager = manager;
        materials = mats;
        towerManager.AddObserver(this); // 옵저버로 등록
        CheckPlacementValidity();
    }

    void OnDestroy()
    {
        if (towerManager != null)
        {
            towerManager.RemoveObserver(this); // 옵저버에서 제거
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Towers"))
        {
            isCollidingWithTower = true;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("Floor"))
        {
            isCollidingWithTerrain = true;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("TowerPowUp"))
        {
            isCollidingWithPowerUp = true;
            CheckPlacementValidity();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Towers"))
        {
            isCollidingWithTower = false;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("Floor"))
        {
            isCollidingWithTerrain = false;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("TowerPowUp"))
        {
            isCollidingWithPowerUp = false;
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
        if (isCollidingWithTerrain && !isCollidingWithTower || isCollidingWithPowerUp)
        {
            SetMaterial(materials[0]);
            towerManager.SetCanPlaceTower(true);

            // 공격력 업 가능 여부를 설정합니다.
            towerManager.canApplyAttackUp = isCollidingWithPowerUp;
        }
        else
        {
            SetMaterial(materials[1]);
            towerManager.SetCanPlaceTower(false);
        }
    }
}
