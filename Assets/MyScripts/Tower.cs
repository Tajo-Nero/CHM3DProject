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
    private Craft[] _craft_Cannon;//프리팹들 담아줄 클래스
    [SerializeField]
    private Transform _Player;//플레이어 앞에 타워 설치될거라 플레이어 좌표 받아옴
    [SerializeField]
    private GameObject _Preview;//미리보기할 프리팹
    [SerializeField]
    private GameObject _TowerPrefab;//진짜 생성될 타워 프리팹
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
    public void CannonTower()//진짜 타워 설치
    {
        _TowerPrefab = Instantiate(_craft_Cannon[0]._Prefab, _Player.position + _Player.forward, Quaternion.identity);        
    }
    public void CannonTowerPreview()//타워 설치할 수있는지없는지 확인용프리뷰 프리팹 생성
    {     
        _Preview = Instantiate(_craft_Cannon[0]._PreviewPrefab, _Player.position + _Player.forward, Quaternion.identity, _Player.transform);
        
        _IsPreviewActivated = true;        
    }    
    private void Cancel()//프리뷰 프리팹 지워주는 함수
    {
        if (_IsPreviewActivated == true)
        {
            Destroy(_Preview);
            _Preview = null;
            _IsPreviewActivated= false;
        }
    }   

}
