using System.Collections.Generic;
using UnityEngine;

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
    public GameObject currentPreview;
    private int currentPreviewIndex = -1;
    private bool canPlaceTower = true;
    public bool canApplyAttackUp = false; // Ÿ�� ���ݷ� ���׷��̵� ���� ����
    [SerializeField] private float maxDistanceFromInitialPosition = 3f;

    private List<IObserver> observers = new List<IObserver>();

    void Start()
    {
        HandlePreviewCreationInput();
        AddObserver(GameManager.Instance);
    }

    void Update()
    {
        HandlePreviewCreationInput();

        if (Input.GetMouseButtonDown(0) && currentPreview != null && canPlaceTower)
        {
            PlaceTower(currentPreviewIndex);
        }

        CheckAndResetPreviewPosition();

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject tower = DetectTower();
            if (tower != null)
            {
                TowerBase towerBase = tower.GetComponent<TowerBase>();
                if (towerBase != null)
                {
                    towerBase.TowerPowUp();
                }
            }
        }

        HandlePreviewCancellation(); // ������ Ÿ�� ��� ó��
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

            NotifyObservers(currentPreview, "PreviewCreated");
        }
    }

    void PlaceTower(int index)
    {
        if (index >= 0 && index < _CraftTower.Length)
        {
            GameObject newTower = Instantiate(_CraftTower[index]._TowerPrefab, currentPreview.transform.position, currentPreview.transform.rotation);

            if (newTower == null)
            {
                Debug.LogError("Rocket �����ڿ��� ������ Ÿ���� ã�� �� �����ϴ�!");
                return;
            }

            Destroy(currentPreview);
            currentPreview = null;
            currentPreviewIndex = -1;

            NotifyObservers(newTower, "TowerPlaced");

            TowerBase towerBase = newTower.GetComponent<TowerBase>();
            if (towerBase != null)
            {
                int towerCost = towerBase.installationCost;
                if (GameManager.Instance.CanAffordInstallation(towerCost))
                {
                    GameManager.Instance.DeductCost(towerCost);
                    Debug.Log("Ÿ���� ��ġ�Ǿ����ϴ�. ���� ��ġ ���: " + GameManager.Instance.installationCost);

                    GameManager.Instance.UpdateUI(); // UI ������Ʈ ȣ��

                    if (canApplyAttackUp)
                    {
                        towerBase.isAttackUp = true;
                        towerBase.towerAttackPower *= 2;
                        Debug.Log("Ÿ���� ���ݷ��� �����߽��ϴ�: " + towerBase.towerAttackPower);
                    }
                }
                else
                {
                    Debug.Log("�ڿ��� �����Ͽ� Ÿ���� ��ġ�� �� �����ϴ�.");
                    Destroy(newTower);
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

    void HandlePreviewCancellation()
    {
        if (currentPreview != null)
        {
            bool cancelKeyPressed = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape);

            if (cancelKeyPressed)
            {
                Destroy(currentPreview);
                currentPreview = null;
                currentPreviewIndex = -1;
                Debug.Log("�����䰡 ��ҵǾ����ϴ�.");
            }
        }
    }
}
