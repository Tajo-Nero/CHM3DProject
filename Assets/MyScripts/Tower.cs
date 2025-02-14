using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Craft
{
    public string _craftName;
    public GameObject _Prefab;
    public GameObject _PreviewPrefab;
}
public class Tower : MonoBehaviour
{
    [SerializeField]
    private Craft[] _craft_Cannon;//�����յ� ����� Ŭ����
    [SerializeField]
    private Transform _Player;//�÷��̾� �տ� Ÿ�� ��ġ�ɰŶ� �÷��̾� ��ǥ �޾ƿ�
    [SerializeField]
    private GameObject _Preview;//�̸������� ������
    [SerializeField]
    private GameObject _TowerPrefab;//��¥ ������ Ÿ�� ������
    [SerializeField]
    bool _IsPreviewActivated = false;      
    
  
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CannonTowerPreview();
            
        }               
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CannonTower();
            Cancel();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }       
    }
    public void CannonTower()//��¥ Ÿ�� ��ġ
    {
        _TowerPrefab = Instantiate(_craft_Cannon[0]._Prefab, _Player.position + _Player.forward, Quaternion.identity);        
    }
    public void CannonTowerPreview()//Ÿ�� ��ġ�� ���ִ��������� Ȯ�ο������� ������ ����
    {     
        _Preview = Instantiate(_craft_Cannon[0]._PreviewPrefab, _Player.position + _Player.forward, Quaternion.identity, _Player.transform);
        
        _IsPreviewActivated = true;        
    }    
    private void Cancel()//������ ������ �����ִ� �Լ�
    {
        if (_IsPreviewActivated == true)
        {
            Destroy(_Preview);
            _Preview = null;
            _IsPreviewActivated= false;
        }
    }   

}
