using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Craft
{
    public string _craftName;
    public GameObject _Prefab;
    public GameObject _PreviewPrefab;
}
//public class Tower : MonoBehaviour
//{
//[SerializeField]
//private Craft[] _craft_Cannon;//프리팹들 담아줄 클래스
//[SerializeField]
//private Transform _Player;//플레이어 앞에 타워 설치될거라 플레이어 좌표 받아옴

//[SerializeField]
//private GameObject[] _Preview;//미리보기할 프리팹
//[SerializeField]
//bool _IsPreviewActivated = false;//프리뷰 설치 판단

//[SerializeField]
//private GameObject[] _TowerPrefab;//진짜 생성될 타워 프리팹
//[SerializeField]
//bool _IsTowerActivated = false;//진짜 타워 설치 설치 판단


//List<Collider> _Colliders;

//void Update()
//{
//if (Input.GetKeyDown(KeyCode.Alpha1))
//{
//CannonTowerPreview();            
//}               
//if (Input.GetKeyDown(KeyCode.Mouse0))
//{
//if (_IsTowerActivated == true)
//{                
//CannonTower();               
//}           
//}
//if (Input.GetKeyDown(KeyCode.Escape))
//{
//Cancel();
//}       
//}
//public void CannonTower()//진짜 타워 설치
//{
//_TowerPrefab[0] = Instantiate(_craft_Cannon[0]._Prefab, _Player.position + _Player.forward, Quaternion.identity);
//_IsTowerActivated = false;
//_IsPreviewActivated = true;
//Cancel();
//}
//public void CannonTowerPreview()//타워 설치할 수있는지없는지 확인용프리뷰 프리팹 생성
//{
//_Preview[0] = Instantiate(_craft_Cannon[0]._PreviewPrefab, _Player.position + _Player.forward, Quaternion.identity, _Player.transform);
//_IsPreviewActivated = true;
//_IsTowerActivated = true;
//}    
//private void Cancel()//프리뷰 프리팹 지워주는 함수
//{
//if (_IsPreviewActivated == true)
//{
//Destroy(_Preview[0]);
////_Preview = null;
//_IsPreviewActivated= false;
//}
//}

//}
public class Tower : MonoBehaviour
{
    [SerializeField]
    public Craft[] _craft_Cannon; // 프리팹들 담아줄 클래스
    [SerializeField]
    public Transform _Player; // 플레이어 앞에 타워 설치될 거라 플레이어 좌표 받아옴

    [SerializeField]
    public GameObject[] _Preview; // 미리보기할 프리팹
    [SerializeField]
    public bool _IsPreviewActivated = false; // 프리뷰 설치 판단

    [SerializeField]
    public GameObject[] _TowerPrefab; // 진짜 생성될 타워 프리팹
    [SerializeField]
    public bool _IsTowerActivated = false; // 진짜 타워 설치 판단

    [SerializeField]
    public Material[] _Material; // 자식객체의 메테리얼 설정

    public Transform _Tower;
    public List<Collider> _Colliders;

    private ITowerState currentState;

    private void Start()
    {
        TransitionToState(new TowerIdleState());
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void TransitionToState(ITowerState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Towers")
        {
            TransitionToState(new TowerRedState());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Towers")
        {
            TransitionToState(new TowerGreenState());
        }
    }
}
