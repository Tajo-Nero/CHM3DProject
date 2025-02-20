using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    [SerializeField]
    public float digDistance = 1.0f;

    public float digRadius = 1.0f; // 땅을 팔 반경

    public float digDepth = 0.1f;//땅 얼만큼 파일지          

    void OnCollisionStay(Collision collision)
    {
        // 충돌된 오브젝트가 터레인인지 확인합니다.
        Terrain terrain = collision.collider.GetComponent<Terrain>();
        if (terrain != null)
        {
            // 터레인 데이터에 접근하여 정보를 얻습니다.
            TerrainData terrainData = terrain.terrainData;
            Vector3 collisionPoint = collision.contacts[0].point;
    
            // 충돌 지점의 월드 좌표를 터레인 데이터의 로컬 좌표로 변환합니다.
            Vector3 terrainPosition = terrain.transform.position;
            int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
            int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);
    
            // 땅을 파는 반경 내의 높이를 부드럽게 조절합니다.
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
                            heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // 높이를 줄입니다.
    
                            // 높이를 업데이트합니다.
                            terrainData.SetHeights(xPos, yPos, heights);
    
                        }
                    }
                }
            }
    
            //Debug.Log($"Collision Point: {collisionPoint}, Terrain Height Reduced around: ({xBase}, {yBase})");
        }
        //else
        //{
        //    Debug.Log("Terrain이 아닙니다.");
        //}
    }
    //void OnCollisionStay(Collision collision)
    //{
        //// 충돌된 오브젝트 정보 출력
        //Debug.Log($"충돌한 오브젝트: {collision.collider.name}, 태그: {collision.collider.tag}");

        //// 충돌된 오브젝트가 터레인인지 확인합니다.
        //Terrain terrain = collision.collider.GetComponent<Terrain>();
        //if (terrain != null)
        //{
            //Debug.Log("터레인 충돌 감지!");

            //// 터레인 데이터에 접근하여 정보를 얻습니다.
            //TerrainData terrainData = terrain.terrainData;
            //Vector3 collisionPoint = collision.contacts[0].point;

            //// 충돌 지점의 월드 좌표를 터레인 데이터의 로컬 좌표로 변환합니다.
            //Vector3 terrainPosition = terrain.transform.position;
            //int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
            //int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

            //Debug.Log($"충돌 지점: {collisionPoint}, 로컬 좌표: ({xBase}, {yBase})");

            //// 땅을 파는 반경 내의 높이를 부드럽게 조절합니다.
            //int radius = Mathf.FloorToInt(digRadius / terrainData.size.x * terrainData.heightmapResolution);

            //// 시작 및 끝 좌표를 계산할 때 범위 내에 있도록 조정
            //int startX = Mathf.Clamp(xBase - radius, 0, terrainData.heightmapResolution - 1);
            //int endX = Mathf.Clamp(xBase + radius, 0, terrainData.heightmapResolution - 1);
            //int startY = Mathf.Clamp(yBase - radius, 0, terrainData.heightmapResolution - 1);
            //int endY = Mathf.Clamp(yBase + radius, 0, terrainData.heightmapResolution - 1);

            //// 높이 배열 가져오기
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
                        //Debug.Log($"(x, y) 좌표에서 높이를 업데이트함: ({startX + x}, {startY + y}), 새 높이 값: {heights[y, x]}");
                    //}
                //}
            //}

            //// 높이 값을 업데이트합니다.
            //terrainData.SetHeights(startX, startY, heights);
            //Debug.Log($"({xBase}, {yBase}) 주변의 높이 값 업데이트됨");
        //}
        //else
        //{
            //Debug.Log("터레인이 아닌 오브젝트와 충돌 감지됨.");
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
            // 히트된 오브젝트가 터레인인지 확인합니다.
            Terrain terrain = hit.collider.GetComponent<Terrain>();
            if (terrain != null)
            {
                // 터레인 데이터에 접근하여 정보를 얻습니다.
                TerrainData terrainData = terrain.terrainData;
                Vector3 hitPoint = hit.point;

                // 히트된 지점의 월드 좌표를 터레인 데이터의 로컬 좌표로 변환합니다.
                Vector3 terrainPosition = terrain.transform.position;
                int x = Mathf.FloorToInt((hitPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
                int z = Mathf.FloorToInt((hitPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

                // 높이를 줄이기 위해 현재 높이를 가져옵니다.
                float[,] heights = terrainData.GetHeights(x, z, 1, 1);
                heights[0, 0] = Mathf.Max(0, heights[0, 0] - 0.1f); // 높이를 0.1만큼 줄입니다.

                // 높이를 업데이트합니다.
                terrainData.SetHeights(x, z, heights);

               //Debug.Log($"Hit Point: {hitPoint}, Terrain Height Reduced at: ({x}, {z})");
            }
            //else
            //{
            //    Debug.Log("Terrain이 아닙니다.");
            //}
        }
    }



}
