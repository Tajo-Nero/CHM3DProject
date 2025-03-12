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
    public bool canApplyAttackUp = false; // 타워 공격력 업그레이드 적용 여부
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

        HandlePreviewCancellation(); // 프리뷰 타워 취소 처리
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
    //1,2,3,4 타워 입력
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
    //입력된 타워 프리뷰 보여주고 설치가능여부 판단함
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
    //프리뷰가 초록색이면 설치가능함, 그중 타워파워업에 위치하면 공격력 2배
    void PlaceTower(int index)
    {
        if (index >= 0 && index < _CraftTower.Length)
        {
            GameObject newTower = Instantiate(_CraftTower[index]._TowerPrefab, currentPreview.transform.position, currentPreview.transform.rotation);          

            Destroy(currentPreview);
            currentPreview = null;
            currentPreviewIndex = -1;

            NotifyObservers(newTower, "TowerPlaced");//참이면 설치함

            TowerBase towerBase = newTower.GetComponent<TowerBase>();
            if (towerBase != null)
            {
                int towerCost = towerBase.installationCost;
                if (GameManager.Instance.CanAffordInstallation(towerCost))
                {
                    GameManager.Instance.DeductCost(towerCost);
                    Debug.Log("타워가 설치되었습니다. 남은 설치 비용: " + GameManager.Instance.installationCost);

                    GameManager.Instance.UpdateUI(); // UI 업데이트 호출

                    if (canApplyAttackUp)
                    {
                        towerBase.isAttackUp = true;
                        towerBase.towerAttackPower *= 2;
                        Debug.Log("타워의 공격력이 증가했습니다: " + towerBase.towerAttackPower);
                    }
                }
                else
                {
                    Debug.Log("자원이 부족하여 타워를 설치할 수 없습니다.");
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
    //마그넷에 붙었다 떼졋을때 위치할 프리뷰 플레이어 앞으로 설정,원래자리로 위치하게함
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
    //타워 제거
    public void RemoveTower(GameObject tower)
    {
        if (tower != null)
        {
            Destroy(tower);
        }
    }
    //프리뷰가 캔슬 할 키 마우스 버튼 과 Esc 제외하면 제거되게 함
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
                Debug.Log("프리뷰가 취소되었습니다.");
            }
        }
    }
}
