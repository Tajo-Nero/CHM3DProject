using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ITowerState
{
    void EnterState(Tower tower);
    void UpdateState(Tower tower);
    void ExitState(Tower tower);
}

public class TowerPreviewState : ITowerState //Ÿ�� ������ ����
{
    public void EnterState(Tower tower)
    {
        tower._Preview[0] = Object.Instantiate(tower._craft_Cannon[0]._PreviewPrefab, tower._Player.position + tower._Player.forward, Quaternion.identity, tower._Player.transform);
        tower._IsPreviewActivated = true;
    }

    public void UpdateState(Tower tower)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) // Ŭ���ϸ� ��¥ Ÿ�� ����
        {
            tower.TransitionToState(new TowerInstallState());

        }
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC ������ ���
        {
            tower.TransitionToState(new TowerCancelState());
        }
    }

    public void ExitState(Tower tower) { }
}

public class TowerInstallState : ITowerState // ��¥ Ÿ�� ����
{
    public void EnterState(Tower tower)
    {
        tower._TowerPrefab[0] = Object.Instantiate(tower._craft_Cannon[0]._Prefab, tower._Player.position + tower._Player.forward, Quaternion.identity);
        tower._IsTowerActivated = false;
        tower._IsPreviewActivated = true;
        tower.TransitionToState(new TowerIdleState());
    }

    public void UpdateState(Tower tower) { }
    public void ExitState(Tower tower) { }
}

public class TowerCancelState : ITowerState //������ Ÿ�� �����
{
    public void EnterState(Tower tower)
    {
        if (tower._IsPreviewActivated == true)
        {
            Object.Destroy(tower._Preview[0]);
            tower._IsPreviewActivated = false;
        }
        tower.TransitionToState(new TowerIdleState());
    }

    public void UpdateState(Tower tower) { }
    public void ExitState(Tower tower) { }
}

public class TowerIdleState : ITowerState //1�� ������ ������ ����
{
    public void EnterState(Tower tower) { }
    public void UpdateState(Tower tower)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            tower.TransitionToState(new TowerPreviewState());
            EnterState(tower);
        }
    }
    public void ExitState(Tower tower) { }
}

public class TowerRedState : ITowerState // Ÿ�� ������ ����� ����
{
    public void EnterState(Tower tower)
    {
        tower._Tower = tower.transform.GetComponentInChildren<Transform>();
        foreach (Transform t in tower._Tower)
        {
            t.gameObject.GetComponent<Renderer>().material = tower._Material[1];
        }
    }

    public void UpdateState(Tower tower) { }
    public void ExitState(Tower tower) { }
}

public class TowerGreenState : ITowerState// Ÿ�� ������ �׸� ���� ����
{
    public void EnterState(Tower tower)
    {
        tower._Tower = tower.transform.GetComponentInChildren<Transform>();
        foreach (Transform t in tower._Tower)
        {
            t.gameObject.GetComponent<Renderer>().material = tower._Material[0];
        }

    }

    public void UpdateState(Tower tower) { }
    public void ExitState(Tower tower) { }
}