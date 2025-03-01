using UnityEngine;

public class TowerFactory
{
    private GameObject[] towerPrefabs;

    public TowerFactory(GameObject[] towerPrefabs)
    {
        this.towerPrefabs = towerPrefabs;
    }

    public TowerBase CreateTower(string towerType)
    {
        GameObject towerObject = null;

        foreach (var prefab in towerPrefabs)
        {
            if (prefab.name == towerType)
            {
                towerObject = GameObject.Instantiate(prefab);
                break;
            }
        }

        if (towerObject != null)
        {
            return towerObject.GetComponent<TowerBase>();
        }

        return null;
    }
}
