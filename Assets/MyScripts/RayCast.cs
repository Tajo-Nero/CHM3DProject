using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    [SerializeField]
    public float digDistance = 1.0f;

    public float digRadius = 1.0f; // ���� �� �ݰ�

    public float digDepth = 0.1f;//�� ��ŭ ������          

    void OnCollisionStay(Collision collision)
    {
        // �浹�� ������Ʈ�� �ͷ������� Ȯ���մϴ�.
        Terrain terrain = collision.collider.GetComponent<Terrain>();
        if (terrain != null)
        {
            // �ͷ��� �����Ϳ� �����Ͽ� ������ ����ϴ�.
            TerrainData terrainData = terrain.terrainData;
            Vector3 collisionPoint = collision.contacts[0].point;
    
            // �浹 ������ ���� ��ǥ�� �ͷ��� �������� ���� ��ǥ�� ��ȯ�մϴ�.
            Vector3 terrainPosition = terrain.transform.position;
            int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
            int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);
    
            // ���� �Ĵ� �ݰ� ���� ���̸� �ε巴�� �����մϴ�.
            int radius = Mathf.FloorToInt(digRadius / terrainData.size.x * terrainData.heightmapResolution);
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int xPos = xBase + x;
                    int yPos = yBase + y;
    
                    if (xPos >= 0 && xPos < terrainData.heightmapResolution && yPos >= 0 && yPos < terrainData.heightmapResolution)
                    {
                        float distance = Mathf.Sqrt(x * x + y * y);
                        if (distance <= radius)
                        {
                            float[,] heights = terrainData.GetHeights(xPos, yPos, 1, 1);
                            float depthFactor = 1 - (distance / radius);
                            heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // ���̸� ���Դϴ�.
    
                            // ���̸� ������Ʈ�մϴ�.
                            terrainData.SetHeights(xPos, yPos, heights);

                        }
                    }
                }
            }
    
            //Debug.Log($"Collision Point: {collisionPoint}, Terrain Height Reduced around: ({xBase}, {yBase})");
        }
        //else
        //{
        //    Debug.Log("Terrain�� �ƴմϴ�.");
        //}
    }
    private void Update()
    {
        //RayTerrain();
    }
    void RayTerrain()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * digDistance;
        Ray ray = new Ray(transform.position, forward);
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit,2f))
        {
            // ��Ʈ�� ������Ʈ�� �ͷ������� Ȯ���մϴ�.
            Terrain terrain = hit.collider.GetComponent<Terrain>();
            if (terrain != null)
            {
                // �ͷ��� �����Ϳ� �����Ͽ� ������ ����ϴ�.
                TerrainData terrainData = terrain.terrainData;
                Vector3 hitPoint = hit.point;

                // ��Ʈ�� ������ ���� ��ǥ�� �ͷ��� �������� ���� ��ǥ�� ��ȯ�մϴ�.
                Vector3 terrainPosition = terrain.transform.position;
                int x = Mathf.FloorToInt((hitPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
                int z = Mathf.FloorToInt((hitPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

                // ���̸� ���̱� ���� ���� ���̸� �����ɴϴ�.
                float[,] heights = terrainData.GetHeights(x, z, 1, 1);
                heights[0, 0] = Mathf.Max(0, heights[0, 0] - 0.1f); // ���̸� 0.1��ŭ ���Դϴ�.

                // ���̸� ������Ʈ�մϴ�.
                terrainData.SetHeights(x, z, heights);

               //Debug.Log($"Hit Point: {hitPoint}, Terrain Height Reduced at: ({x}, {z})");
            }
            //else
            //{
            //    Debug.Log("Terrain�� �ƴմϴ�.");
            //}
        }
    }



}
