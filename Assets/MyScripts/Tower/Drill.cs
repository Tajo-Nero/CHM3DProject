//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class Drill : MonoBehaviour
//{
//    [SerializeField]
//    public float digDistance = 2f;
//    public float digRadius = 2f; // 땅굴의 반경
//    public float digDepth = 0.2f; // 파 내려가는 깊이

//    void OnTriggerStay(Collider other)
//    {
//        // 트리거에 들어온 콜라이더가 TerrainCollider인지 확인합니다.
//        TerrainCollider terrainCollider = other as TerrainCollider;
//        if (terrainCollider != null)
//        {
//            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
//            // Terrain 데이터를 가져옵니다.
//            Terrain terrain = terrainCollider.GetComponent<Terrain>();
//            TerrainData terrainData = terrain.terrainData;
//            Vector3 terrainPosition = terrain.transform.position;

//            // Drill의 위치를 충돌 지점으로 사용합니다.
//            Vector3 collisionPoint = transform.position;

//            // 충돌 지점의 월드 좌표를 지형 좌표로 변환합니다.
//            //collisionPoint 은 충돌된 터레인좌표 - 터레인의 원래좌표 / 부딪힌 터레인의 총 x 길이 * 부딪힌 터레인 해상도
//            int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
//            //y 가 터레인 y 값을 넣으면 높이 위아래가 됨 z 가 2D에서 높이가됨
//            int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

//            // 파 내려갈 반경을 기준으로 높이 맵을 수정합니다.
//            // digRadius= 2  파낼 반경
//            //radius 에 digRadius/부딪힌터레인의 가로=x 로 나누면 digRadius를 지형 데이터의 비율로 맞춰줌
//            int radius = Mathf.FloorToInt(digRadius / terrainData.size.x * terrainData.heightmapResolution);

//            //radius 가 digRadius로 설정한값 즉 현재 -2부터 2 까지 
//            for (int x = -radius; x <= radius; x++)
//            {
//                //y= 터레인사이즈.z 임 즉 위아래 -2 ~2 까지 반복
//                for (int y = -radius; y <= radius; y++)
//                {
//                    int xPos = xBase + x;
//                    int yPos = yBase + y;

//                    if (xPos >= 0 && xPos < terrainData.heightmapResolution && yPos >= 0 && yPos < terrainData.heightmapResolution)
//                    {
//                        //위에 xBase와 YBase에 닿은 좌표 기준점으로 반지름 x= -2  y=-2 유클리드 거리를 계산하는 방법이라는대 정확히는 모르겟다
//                        //그래서 계산해보면 2.828 인 값이 나옴 콜라이더의 닿은곳과 Mathf.Sqrt(x,y) 사이의 거리를 계산해줌 닿은곳에서 반지름인듯함
//                        float distance = Mathf.Sqrt(x * x + y * y);
//                        if (distance <= radius)
//                        {
//                            //xPos, yPos 는 닿은 좌표, 1,1 = 1x1 크기안의 높이데이터를 반환 높이 6으로 설정하면 heights에 6을 반환함
//                            float[,] heights = terrainData.GetHeights(xPos, yPos, 1, 1);

//                            //Mathf.Lerp(1, 0, distance / radius) distance과 radius를 비교해서 0,1 을 depthFactor 에 담아준다
//                            float depthFactor = Mathf.Lerp(1, 0, distance / radius); // 거리 비례하여 깊이 설정

//                            //digDepth = 파내리는 정도 0.2 씩 줄어들어라
//                            //heights[0, 0] 충돌한 터레인의 높이 6이 반환된상태고 높이 6의 터레인을 digDepth -0.2 *
//                            //digDepth 에 0,1이 담기니까 0보다 작아지지 않도록 최소값 설정
//                            heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // 높이를 줄입니다.

//                            // 변경된 높이 값을 설정합니다.
//                            terrainData.SetHeights(xPos, yPos, heights);
//                        }
//                    }
//                }
//            }
//            // Debug.Log($"Collision Point: {collisionPoint}, Terrain Height Reduced around: ({xBase}, {yBase})");
//        }
//    }

//}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Drill : MonoBehaviour
{
    [SerializeField]
    public float digDepth = 0.2f; // 파낼 깊이 설정

    void OnTriggerStay(Collider other)
    {
        // 트리거에 들어오는 오브젝트가 TerrainCollider인지 확인합니다.
        TerrainCollider terrainCollider = other as TerrainCollider;
        if (terrainCollider != null)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            // Terrain 데이터를 가져옵니다.
            Terrain terrain = terrainCollider.GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPosition = terrain.transform.position;

            // Drill의 위치를 충돌 지점으로 설정합니다.
            Vector3 collisionPoint = transform.position;

            // 충돌 지점의 월드 좌표를 지형 데이터의 높이맵 좌표로 변환합니다.
            int xPos = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
            int yPos = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

            if (xPos >= 0 && xPos < terrainData.heightmapResolution && yPos >= 0 && yPos < terrainData.heightmapResolution)
            {
                float[,] heights = terrainData.GetHeights(xPos, yPos, 1, 1);
                heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth); // 높이를 줄입니다.

                // 변경된 높이 값을 설정합니다.
                terrainData.SetHeights(xPos, yPos, heights);
            }
        }
    }
}

