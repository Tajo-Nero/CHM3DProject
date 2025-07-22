using UnityEngine;

public class TowerSelectionManager : MonoBehaviour
{
    public static TowerSelectionManager Instance;
    private TowerBase currentSelectedTower;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        HandleTowerSelection();
    }

    void HandleTowerSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TowerBase tower = hit.collider.GetComponent<TowerBase>();

                if (tower != null)
                {
                    SelectTower(tower);
                }
                else
                {
                    DeselectCurrentTower();
                }
            }
            else
            {
                DeselectCurrentTower();
            }
        }
    }

    public void SelectTower(TowerBase tower)
    {
        // ���� Ÿ�� ���� ����
        DeselectCurrentTower();

        // �� Ÿ�� ����
        currentSelectedTower = tower;
        tower.ShowRange();

        Debug.Log($"Ÿ�� ����: {tower.name}");
    }

    public void DeselectCurrentTower()
    {
        if (currentSelectedTower != null)
        {
            currentSelectedTower.HideRange();
            currentSelectedTower = null;
        }
    }
}