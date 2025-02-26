using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drill : MonoBehaviour
{
    [SerializeField]
    public float digDistance = 2f;

    public float digRadius = 2f; // ������ �ݰ�

    public float digDepth = 0.2f; // �� �������� ����

    void OnTriggerStay(Collider other)
    {
        // Ʈ���ſ� ���� �ݶ��̴��� TerrainCollider���� Ȯ���մϴ�.
        TerrainCollider terrainCollider = other as TerrainCollider;
        if (terrainCollider != null)
        {
           gameObject.transform.rotation = Quaternion.Euler(0,0,0);
            // Terrain �����͸� �����ɴϴ�.
            Terrain terrain = terrainCollider.GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPosition = terrain.transform.position;

            // Drill�� ��ġ�� �浹 �������� ����մϴ�.
            Vector3 collisionPoint = transform.position;

            // �浹 ������ ���� ��ǥ�� ���� ��ǥ�� ��ȯ�մϴ�.
            int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
            int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

            // �� ������ �ݰ��� �������� ���� ���� �����մϴ�.
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
                            float depthFactor = Mathf.Lerp(1, 0, distance / radius); // �Ÿ� ����Ͽ� ���� ����
                            heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // ���̸� ���Դϴ�.

                            // ����� ���� ���� �����մϴ�.
                            terrainData.SetHeights(xPos, yPos, heights);
                        }
                    }
                }
            }
            // Debug.Log($"Collision Point: {collisionPoint}, Terrain Height Reduced around: ({xBase}, {yBase})");
        }
    }

    //void OnCollisionStay(Collision collision)
    //{
    //    // �浹�� ������Ʈ�� �ͷ������� Ȯ���մϴ�.
    //    Terrain terrain = collision.collider.GetComponent<Terrain>();
    //    if (terrain != null)
    //    {
    //        // �ͷ��� �����Ϳ� �����Ͽ� ������ ����ϴ�.
    //        TerrainData terrainData = terrain.terrainData;
    //        Vector3 collisionPoint = collision.contacts[0].point;
    //
    //        // �浹 ������ ���� ��ǥ�� �ͷ��� �������� ���� ��ǥ�� ��ȯ�մϴ�.
    //        Vector3 terrainPosition = terrain.transform.position;
    //        int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
    //        int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);
    //
    //        // ���� �Ĵ� �ݰ� ���� ���̸� �ε巴�� �����մϴ�.
    //        int radius = Mathf.FloorToInt(digRadius / terrainData.size.x * terrainData.heightmapResolution);
    //        for (int x = -radius; x <= radius; x++)
    //        {
    //            for (int y = -radius; y <= radius; y++)
    //            {
    //                int xPos = xBase + x;
    //                int yPos = yBase + y;
    //
    //                if (xPos >= 0 && xPos < terrainData.heightmapResolution && yPos >= 0 && yPos < terrainData.heightmapResolution)
    //                {
    //                    float distance = Mathf.Sqrt(x * x + y * y);
    //                    if (distance <= radius)
    //                    {
    //                        float[,] heights = terrainData.GetHeights(xPos, yPos, 1, 1);
    //                        float depthFactor = 1 - (distance / radius);
    //                        heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // ���̸� ���Դϴ�.
    //
    //                        // ���̸� ������Ʈ�մϴ�.
    //                        terrainData.SetHeights(xPos, yPos, heights);
    //
    //                    }
    //                }
    //            }
    //        }
    //
    //        //Debug.Log($"Collision Point: {collisionPoint}, Terrain Height Reduced around: ({xBase}, {yBase})");
    //    }
    //    //else
    //    //{
    //    //    Debug.Log("Terrain�� �ƴմϴ�.");
    //    //}
    //}
}
