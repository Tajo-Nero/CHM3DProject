using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void OnNotify(GameObject obj, string eventMessage);
}

public interface ISubject
{
    void AddObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void NotifyObservers(GameObject obj, string eventMessage);
}

[System.Serializable]
public class TowerCraft
{
    public string _craftName;
    public GameObject _TowerPrefab;
    public GameObject _PreviewPrefab;
}

public class TowerGenerator : MonoBehaviour, ISubject
{
    [SerializeField]
    private TowerCraft[] _CraftTower;
    [SerializeField]
    private Transform _Player;
    [SerializeField]
    private Material[] _Materials;
    [SerializeField]
    public GameObject currentPreview;
    private int currentPreviewIndex = -1;
    private bool canPlaceTower = true;
    public bool canApplyAttackUp = false; // 공격력 업 가능 여부를 판단하는 변수
    [SerializeField] private float maxDistanceFromInitialPosition = 3f;

    private List<IObserver> observers = new List<IObserver>(); // 옵저버 목록

    void Start()
    {
        HandlePreviewCreationInput();
    }

    void Update()
    {
        HandlePreviewCreationInput();

        if (Input.GetMouseButtonDown(0) && currentPreview != null && canPlaceTower)
        {
            PlaceTower(currentPreviewIndex);
        }       

        CheckAndResetPreviewPosition();
    }

    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(GameObject obj, string eventMessage)
    {
        foreach (IObserver observer in observers)
        {
            observer.OnNotify(obj, eventMessage);
        }
    }

    void HandlePreviewCreationInput()
    {
        for (int i = 0; i <= 3; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                CreatePreview(i);
                break;
            }
        }
    }

    void CreatePreview(int index)
    {
        if (index >= 0 && index < _CraftTower.Length)
        {
            if (currentPreview != null)
            {
                Destroy(currentPreview);
            }
            currentPreview = Instantiate(_CraftTower[index]._PreviewPrefab, _Player.position + _Player.forward, _Player.rotation, _Player.transform);

            Renderer renderer = currentPreview.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = _Materials[0];
            }
            else
            {
                Renderer childRenderer = currentPreview.GetComponentInChildren<Renderer>();
                if (childRenderer != null)
                {
                    childRenderer.material = _Materials[0];
                }
            }
            PreviewCollisionDetector previewCollision = currentPreview.AddComponent<PreviewCollisionDetector>();
            previewCollision.Setup(this, _Materials);
            currentPreviewIndex = index;

            // 옵저버에게 알림을 보냅니다.
            NotifyObservers(currentPreview, "PreviewCreated");
        }
    }

    void PlaceTower(int index)
    {
        if (index >= 0 && index < _CraftTower.Length)
        {
            GameObject newTower = Instantiate(_CraftTower[index]._TowerPrefab, currentPreview.transform.position, currentPreview.transform.rotation);
            Destroy(currentPreview);
            currentPreview = null;
            currentPreviewIndex = -1;

            // 옵저버에게 알림을 보냅니다.
            NotifyObservers(newTower, "TowerPlaced");

            // 공격력 업 여부를 적용합니다.
            if (canApplyAttackUp)
            {
                TowerBase towerBase = newTower.GetComponent<TowerBase>();
                if (towerBase != null)
                {
                    towerBase.isAttackUp = true;
                    towerBase.towerAttackPower *= 2;
                    Debug.Log("타워 생성 시 공격력 업 적용됨: " + towerBase.towerAttackPower);
                }
            }
        }
    }

    public void SetCanPlaceTower(bool value)
    {
        canPlaceTower = value;
    }

    GameObject DetectTower()
    {
        Ray ray = new Ray(_Player.position, _Player.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 3.0f))
        {
            if (hitInfo.collider.CompareTag("Towers"))
            {
                return hitInfo.collider.gameObject;
            }
        }
        return null;
    }

    void CheckAndResetPreviewPosition()
    {
        if (currentPreview != null)
        {
            float distance = Vector3.Distance(_Player.transform.position, currentPreview.transform.position);
            if (distance > maxDistanceFromInitialPosition)
            {
                currentPreview.transform.position = _Player.transform.position + _Player.transform.forward;

                Quaternion newRotation = Quaternion.Euler(_Player.rotation.eulerAngles.x, _Player.rotation.eulerAngles.y + currentPreview.transform.rotation.y, _Player.rotation.eulerAngles.z);
                currentPreview.transform.rotation = newRotation;
            }
        }
    }
    public void RemoveTower(GameObject tower)
    {
        if (tower != null)
        {
            Destroy(tower);
        }
    }
}




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



