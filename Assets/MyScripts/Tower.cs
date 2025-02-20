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
//    private TowerCraft[] _CraftTower;//프리팹들 담아줄 클래스
//    [SerializeField]
//    private Transform _Player;//플레이어 앞에 타워 설치될거라 플레이어 좌표 받아옴
//
//    [SerializeField]
//    private GameObject[] _Preview;//미리보기할 프리팹
//    [SerializeField]
//    bool _IsPreviewActivated = false;//프리뷰 설치 판단
//
//    [SerializeField]
//    private GameObject[] _Tower;//진짜 생성될 타워 프리팹
//    [SerializeField]
//    bool _IsTowerActivated = false;//진짜 타워 설치 설치 판단
//    [SerializeField]
//    public Material[] _Material; // 배열안의 객체의 메테리얼 설정 초록일때 설치 가능 , 레드 일때 설치 불가능
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
//    public void CannonTower(int i)//진짜 타워 설치
//    {
//        if (i >= 0 && i < _Tower.Length)
//        {
//            _Tower[i] = Instantiate(_CraftTower[i]._TowerPrefab, _Player.position + _Player.forward, Quaternion.identity, _Player.transform);
//        }
//        _IsPreviewActivated = true;
//            Cancel(i);
//     } 
//    public void CannonTowerPreview(int i)//타워 설치할 수있는지없는지 확인용프리뷰 프리팹 생성
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
//    private void Cancel(int i)//프리뷰 프리팹 지워주는 함수
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
//public TowerCraft[] _craft_Cannon; // 프리팹들 담아줄 클래스 3가지 캐논,레이저,로켓 
//[SerializeField]
//public Transform _Player; // 플레이어 앞에 타워 설치될 거라 플레이어 좌표 받아옴

//[SerializeField]
//public GameObject[] _Preview; // 미리보기할 프리팹
//[SerializeField]
//public bool _IsPreviewActivated = true; // 프리뷰 설치 판단

//[SerializeField]
//public GameObject[] _TowerPrefab; // 진짜 생성될 타워 프리팹
//[SerializeField]
//public bool _IsTowerActivated = true; // 진짜 타워 설치 판단

//[SerializeField]
//public Material[] _Material; // 배열안의 객체의 메테리얼 설정 초록일때 설치 가능 , 레드 일때 설치 불가능

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
    private TowerCraft[] _CraftTower; // 프리팹들 담아줄 클래스
    [SerializeField]
    private Transform _Player; // 플레이어 앞에 타워 설치될 거라 플레이어 좌표 받아옴
    [SerializeField]
    private Material[] _Materials; // 설치 가능할 때의 초록색 메테리얼과 설치 불가능할 때의 빨간색 메테리얼을 배열로 받음
    [SerializeField]
    private GameObject currentPreview;
    private int currentPreviewIndex = -1;
    private bool canPlaceTower = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreatePreview(0); // 1번 키를 누르면 캐논 프리뷰 생성
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreatePreview(1); // 2번 키를 누르면 레이저 프리뷰 생성
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CreatePreview(2); // 3번 키를 누르면 로켓 프리뷰 생성
        }

        if (Input.GetMouseButtonDown(0) && currentPreview != null && canPlaceTower)
        {
            {
                PlaceTower(currentPreviewIndex); // 마우스를 클릭하면 실제 타워 생성
            }
        }

        void CreatePreview(int index)
        {
            if (index >= 0 && index < _CraftTower.Length)
            {
                if (currentPreview != null)
                {
                    Destroy(currentPreview); // 기존의 프리뷰가 있을 경우 제거
                }
                currentPreview = Instantiate(_CraftTower[index]._PreviewPrefab, _Player.position + _Player.forward, _Player.rotation, _Player.transform);
                Renderer renderer = currentPreview.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = _Materials[0]; // 초기 메테리얼을 초록색으로 설정
                }
                else
                {
                    // 자식 오브젝트에서 Renderer를 찾아 설정
                    Renderer childRenderer = currentPreview.GetComponentInChildren<Renderer>();
                    if (childRenderer != null)
                    {
                        childRenderer.material = _Materials[0]; // 초기 메테리얼을 초록색으로 설정
                    }
                }
                currentPreview.AddComponent<PreviewCollisionDetector>().Setup(this, _Materials);
                currentPreviewIndex = index; // 현재 프리뷰의 인덱스 저장
            }
        }

        void PlaceTower(int index)
        {
            if (index >= 0 && index < _CraftTower.Length)
            {
                Instantiate(_CraftTower[index]._TowerPrefab, _Player.position + _Player.forward, _Player.rotation);
                Destroy(currentPreview); // 타워를 설치한 후 프리뷰 제거
                currentPreview = null; // 현재 프리뷰 변수 초기화
                currentPreviewIndex = -1; // 현재 프리뷰 인덱스 초기화
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
            Debug.Log("충돌 감지: " + other.name);
            isCollidingWithTower = true;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("Floor"))
        {
            Debug.Log("터레인 감지: " + other.name);
            isCollidingWithTerrain = true;
            CheckPlacementValidity();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Towers"))
        {
            Debug.Log("충돌 종료: " + other.name);
            isCollidingWithTower = false;
            CheckPlacementValidity();
        }
        else if (other.CompareTag("Floor"))
        {
            Debug.Log("터레인 종료: " + other.name);
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
            SetMaterial(materials[0]); // 초록색 메테리얼로 설정
            towerManager.SetCanPlaceTower(true);
        }
        else
        {
            SetMaterial(materials[1]); // 빨간색 메테리얼로 설정
            towerManager.SetCanPlaceTower(false);
        }
    }
}