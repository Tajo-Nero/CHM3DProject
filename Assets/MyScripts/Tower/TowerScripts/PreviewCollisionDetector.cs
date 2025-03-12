using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollisionDetector : MonoBehaviour, IObserver
{
    private TowerGenerator towerManager;
    private Material[] materials;
    private bool isCollidingWithTower = false; //타워설치 가능여부
    private bool isCollidingWithTerrain = false; //타워프리뷰 설치 가능여부
    private bool isCollidingWithPowerUp = false; //타워 파워업 가능 여부

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

    //머테리얼색으로 타워 건설 가능 불가능여부 판단
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
            towerManager.SetCanPlaceTower(true);//초록색이면 트루 반환

            // 공격력 업 가능 여부를 설정합니다.
            towerManager.canApplyAttackUp = isCollidingWithPowerUp;
        }
        else
        {
            SetMaterial(materials[1]);
            towerManager.SetCanPlaceTower(false);//빨강색이면 설치 불가
        }
    }
}
