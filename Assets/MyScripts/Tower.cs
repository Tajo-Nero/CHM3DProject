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
//private Craft[] _craft_Cannon;//�����յ� ����� Ŭ����
//[SerializeField]
//private Transform _Player;//�÷��̾� �տ� Ÿ�� ��ġ�ɰŶ� �÷��̾� ��ǥ �޾ƿ�

//[SerializeField]
//private GameObject[] _Preview;//�̸������� ������
//[SerializeField]
//bool _IsPreviewActivated = false;//������ ��ġ �Ǵ�

//[SerializeField]
//private GameObject[] _TowerPrefab;//��¥ ������ Ÿ�� ������
//[SerializeField]
//bool _IsTowerActivated = false;//��¥ Ÿ�� ��ġ ��ġ �Ǵ�


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
//public void CannonTower()//��¥ Ÿ�� ��ġ
//{
//_TowerPrefab[0] = Instantiate(_craft_Cannon[0]._Prefab, _Player.position + _Player.forward, Quaternion.identity);
//_IsTowerActivated = false;
//_IsPreviewActivated = true;
//Cancel();
//}
//public void CannonTowerPreview()//Ÿ�� ��ġ�� ���ִ��������� Ȯ�ο������� ������ ����
//{
//_Preview[0] = Instantiate(_craft_Cannon[0]._PreviewPrefab, _Player.position + _Player.forward, Quaternion.identity, _Player.transform);
//_IsPreviewActivated = true;
//_IsTowerActivated = true;
//}    
//private void Cancel()//������ ������ �����ִ� �Լ�
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
    public Craft[] _craft_Cannon; // �����յ� ����� Ŭ����
    [SerializeField]
    public Transform _Player; // �÷��̾� �տ� Ÿ�� ��ġ�� �Ŷ� �÷��̾� ��ǥ �޾ƿ�

    [SerializeField]
    public GameObject[] _Preview; // �̸������� ������
    [SerializeField]
    public bool _IsPreviewActivated = false; // ������ ��ġ �Ǵ�

    [SerializeField]
    public GameObject[] _TowerPrefab; // ��¥ ������ Ÿ�� ������
    [SerializeField]
    public bool _IsTowerActivated = false; // ��¥ Ÿ�� ��ġ �Ǵ�

    [SerializeField]
    public Material[] _Material; // �ڽİ�ü�� ���׸��� ����

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
