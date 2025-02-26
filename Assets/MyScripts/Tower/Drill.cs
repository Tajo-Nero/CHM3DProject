using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drill : MonoBehaviour
{
    [SerializeField]
    public float digDistance = 2f;

    public float digRadius = 2f; // 땅굴의 반경

    public float digDepth = 0.2f; // 파 내려가는 깊이

    void OnTriggerStay(Collider other)
    {
        // 트리거에 들어온 콜라이더가 TerrainCollider인지 확인합니다.
        TerrainCollider terrainCollider = other as TerrainCollider;
        if (terrainCollider != null)
        {
           gameObject.transform.rotation = Quaternion.Euler(0,0,0);
            // Terrain 데이터를 가져옵니다.
            Terrain terrain = terrainCollider.GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPosition = terrain.transform.position;

            // Drill의 위치를 충돌 지점으로 사용합니다.
            Vector3 collisionPoint = transform.position;

            // 충돌 지점의 월드 좌표를 지형 좌표로 변환합니다.
            int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
            int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

            // 파 내려갈 반경을 기준으로 높이 맵을 수정합니다.
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
                            float depthFactor = Mathf.Lerp(1, 0, distance / radius); // 거리 비례하여 깊이 설정
                            heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // 높이를 줄입니다.

                            // 변경된 높이 값을 설정합니다.
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
    //    // 충돌된 오브젝트가 터레인인지 확인합니다.
    //    Terrain terrain = collision.collider.GetComponent<Terrain>();
    //    if (terrain != null)
    //    {
    //        // 터레인 데이터에 접근하여 정보를 얻습니다.
    //        TerrainData terrainData = terrain.terrainData;
    //        Vector3 collisionPoint = collision.contacts[0].point;
    //
    //        // 충돌 지점의 월드 좌표를 터레인 데이터의 로컬 좌표로 변환합니다.
    //        Vector3 terrainPosition = terrain.transform.position;
    //        int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
    //        int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);
    //
    //        // 땅을 파는 반경 내의 높이를 부드럽게 조절합니다.
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
    //                        heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // 높이를 줄입니다.
    //
    //                        // 높이를 업데이트합니다.
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
    //    //    Debug.Log("Terrain이 아닙니다.");
    //    //}
    //}
}
