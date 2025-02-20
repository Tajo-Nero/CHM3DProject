using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//[System.Serializable]
//public class TowerCraft
//{
//    public string _craftName;
//    public GameObject _TowerPrefab;
//    public GameObject _PreviewPrefab;
//}
//public class Tower : MonoBehaviour
//{
//    [SerializeField]
//    private TowerCraft[] _CraftTower;//�����յ� ����� Ŭ����
//    [SerializeField]
//    private Transform _Player;//�÷��̾� �տ� Ÿ�� ��ġ�ɰŶ� �÷��̾� ��ǥ �޾ƿ�
//
//    [SerializeField]
//    private GameObject[] _Preview;//�̸������� ������
//    [SerializeField]
//    bool _IsPreviewActivated = false;//������ ��ġ �Ǵ�
//
//    [SerializeField]
//    private GameObject[] _Tower;//��¥ ������ Ÿ�� ������
//    [SerializeField]
//    bool _IsTowerActivated = false;//��¥ Ÿ�� ��ġ ��ġ �Ǵ�
//    [SerializeField]
//    public Material[] _Material; // �迭���� ��ü�� ���׸��� ���� �ʷ��϶� ��ġ ���� , ���� �϶� ��ġ �Ұ���
//
//
//    List<Collider> _Colliders;
//
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            CannonTowerPreview(0);           
//        }
//        if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            CannonTowerPreview(1);
//        }
//        if (Input.GetKeyDown(KeyCode.Alpha3))
//        { 
//            CannonTowerPreview(2);
//        }
//        if (Input.GetMouseButtonDown(1))
//        {
//            CannonTower(0);
//        }
//
//        //if (Input.GetKeyDown(KeyCode.Escape))
//        //{
//        //    Cancel();
//        //}
//    }
//    public void CannonTower(int i)//��¥ Ÿ�� ��ġ
//    {
//        if (i >= 0 && i < _Tower.Length)
//        {
//            _Tower[i] = Instantiate(_CraftTower[i]._TowerPrefab, _Player.position + _Player.forward, Quaternion.identity, _Player.transform);
//        }
//        _IsPreviewActivated = true;
//            Cancel(i);
//     } 
//    public void CannonTowerPreview(int i)//Ÿ�� ��ġ�� ���ִ��������� Ȯ�ο������� ������ ����
//    {
//        if (i >= 0 && i < _Preview.Length)
//        {
//            _Preview[i] = Instantiate(_CraftTower[i]._PreviewPrefab, _Player.position + _Player.forward, Quaternion.identity, _Player.transform);
//            _IsPreviewActivated = true;
//            _IsTowerActivated = true;
//        }
//        
//        
//    }
//    private void Cancel(int i)//������ ������ �����ִ� �Լ�
//    {
//        if (_IsPreviewActivated == true)
//        {
//            Destroy(_Preview[i]);
//            //_Preview = null;
//            _IsPreviewActivated = false;
//        }
//    }
//
//}
//public class Tower : MonoBehaviour
//{
//[SerializeField]
//public TowerCraft[] _craft_Cannon; // �����յ� ����� Ŭ���� 3���� ĳ��,������,���� 
//[SerializeField]
//public Transform _Player; // �÷��̾� �տ� Ÿ�� ��ġ�� �Ŷ� �÷��̾� ��ǥ �޾ƿ�

//[SerializeField]
//public GameObject[] _Preview; // �̸������� ������
//[SerializeField]
//public bool _IsPreviewActivated = true; // ������ ��ġ �Ǵ�

//[SerializeField]
//public GameObject[] _TowerPrefab; // ��¥ ������ Ÿ�� ������
//[SerializeField]
//public bool _IsTowerActivated = true; // ��¥ Ÿ�� ��ġ �Ǵ�

//[SerializeField]
//public Material[] _Material; // �迭���� ��ü�� ���׸��� ���� �ʷ��϶� ��ġ ���� , ���� �϶� ��ġ �Ұ���

//public Transform _Tower;
//public List<Collider> _Colliders;

//private ITowerState currentState;

//private void Start()
//{
//TransitionToState(new TowerIdleState());
//}

//private void Update()
//{
//currentState.UpdateState(this);
//}

//public void TransitionToState(ITowerState newState)
//{
//if (currentState != null)
//{
//currentState.ExitState(this);
//}
//currentState = newState;
//currentState.EnterState(this);

//}

//private void OnTriggerEnter(Collider other)
//{
//if (other.gameObject.tag == "Towers")
//{
//TransitionToState(new TowerRedState());
//}
//}

//private void OnTriggerExit(Collider other)
//{
//_IsTowerActivated = true;
//_IsPreviewActivated = true;
//if (other.gameObject.tag == "Towers")
//{
//TransitionToState(new TowerGreenState());
//}
//}
//}
[System.Serializable]
public class TowerCraft
{
    public string _craftName;
    public GameObject _TowerPrefab;
    public GameObject _PreviewPrefab;
}

public class Tower : MonoBehaviour
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
    private Tower towerManager;
    private Material[] materials;
    private bool isCollidingWithTower = false;
    private bool isCollidingWithTerrain = false;

    public void Setup(Tower manager, Material[] mats)
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