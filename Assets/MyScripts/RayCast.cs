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
    //void OnCollisionStay(Collision collision)
    //{
        //// �浹�� ������Ʈ ���� ���
        //Debug.Log($"�浹�� ������Ʈ: {collision.collider.name}, �±�: {collision.collider.tag}");

        //// �浹�� ������Ʈ�� �ͷ������� Ȯ���մϴ�.
        //Terrain terrain = collision.collider.GetComponent<Terrain>();
        //if (terrain != null)
        //{
            //Debug.Log("�ͷ��� �浹 ����!");

            //// �ͷ��� �����Ϳ� �����Ͽ� ������ ����ϴ�.
            //TerrainData terrainData = terrain.terrainData;
            //Vector3 collisionPoint = collision.contacts[0].point;

            //// �浹 ������ ���� ��ǥ�� �ͷ��� �������� ���� ��ǥ�� ��ȯ�մϴ�.
            //Vector3 terrainPosition = terrain.transform.position;
            //int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
            //int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

            //Debug.Log($"�浹 ����: {collisionPoint}, ���� ��ǥ: ({xBase}, {yBase})");

            //// ���� �Ĵ� �ݰ� ���� ���̸� �ε巴�� �����մϴ�.
            //int radius = Mathf.FloorToInt(digRadius / terrainData.size.x * terrainData.heightmapResolution);

            //// ���� �� �� ��ǥ�� ����� �� ���� ���� �ֵ��� ����
            //int startX = Mathf.Clamp(xBase - radius, 0, terrainData.heightmapResolution - 1);
            //int endX = Mathf.Clamp(xBase + radius, 0, terrainData.heightmapResolution - 1);
            //int startY = Mathf.Clamp(yBase - radius, 0, terrainData.heightmapResolution - 1);
            //int endY = Mathf.Clamp(yBase + radius, 0, terrainData.heightmapResolution - 1);

            //// ���� �迭 ��������
            //float[,] heights = terrainData.GetHeights(startX, startY, endX - startX + 1, endY - startY + 1);

            //for (int y = 0; y < heights.GetLength(0); y++)
            //{
                //for (int x = 0; x < heights.GetLength(1); x++)
                //{
                    //float distance = Mathf.Sqrt(Mathf.Pow(x + startX - xBase, 2) + Mathf.Pow(y + startY - yBase, 2));
                    //if (distance <= radius)
                    //{
                        //float depthFactor = 1 - (distance / radius);
                        //heights[y, x] = Mathf.Max(0, heights[y, x] - digDepth * depthFactor);
                        //Debug.Log($"(x, y) ��ǥ���� ���̸� ������Ʈ��: ({startX + x}, {startY + y}), �� ���� ��: {heights[y, x]}");
                    //}
                //}
            //}

            //// ���� ���� ������Ʈ�մϴ�.
            //terrainData.SetHeights(startX, startY, heights);
            //Debug.Log($"({xBase}, {yBase}) �ֺ��� ���� �� ������Ʈ��");
        //}
        //else
        //{
            //Debug.Log("�ͷ����� �ƴ� ������Ʈ�� �浹 ������.");
        //}
    //}





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
