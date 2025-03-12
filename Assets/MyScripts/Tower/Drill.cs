//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class Drill : MonoBehaviour
//{
//    [SerializeField]
//    public float digDistance = 2f;
//    public float digRadius = 2f; // ������ �ݰ�
//    public float digDepth = 0.2f; // �� �������� ����

//    void OnTriggerStay(Collider other)
//    {
//        // Ʈ���ſ� ���� �ݶ��̴��� TerrainCollider���� Ȯ���մϴ�.
//        TerrainCollider terrainCollider = other as TerrainCollider;
//        if (terrainCollider != null)
//        {
//            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
//            // Terrain �����͸� �����ɴϴ�.
//            Terrain terrain = terrainCollider.GetComponent<Terrain>();
//            TerrainData terrainData = terrain.terrainData;
//            Vector3 terrainPosition = terrain.transform.position;

//            // Drill�� ��ġ�� �浹 �������� ����մϴ�.
//            Vector3 collisionPoint = transform.position;

//            // �浹 ������ ���� ��ǥ�� ���� ��ǥ�� ��ȯ�մϴ�.
//            //collisionPoint �� �浹�� �ͷ�����ǥ - �ͷ����� ������ǥ / �ε��� �ͷ����� �� x ���� * �ε��� �ͷ��� �ػ�
//            int xBase = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
//            //y �� �ͷ��� y ���� ������ ���� ���Ʒ��� �� z �� 2D���� ���̰���
//            int yBase = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

//            // �� ������ �ݰ��� �������� ���� ���� �����մϴ�.
//            // digRadius= 2  �ĳ� �ݰ�
//            //radius �� digRadius/�ε����ͷ����� ����=x �� ������ digRadius�� ���� �������� ������ ������
//            int radius = Mathf.FloorToInt(digRadius / terrainData.size.x * terrainData.heightmapResolution);

//            //radius �� digRadius�� �����Ѱ� �� ���� -2���� 2 ���� 
//            for (int x = -radius; x <= radius; x++)
//            {
//                //y= �ͷ��λ�����.z �� �� ���Ʒ� -2 ~2 ���� �ݺ�
//                for (int y = -radius; y <= radius; y++)
//                {
//                    int xPos = xBase + x;
//                    int yPos = yBase + y;

//                    if (xPos >= 0 && xPos < terrainData.heightmapResolution && yPos >= 0 && yPos < terrainData.heightmapResolution)
//                    {
//                        //���� xBase�� YBase�� ���� ��ǥ ���������� ������ x= -2  y=-2 ��Ŭ���� �Ÿ��� ����ϴ� ����̶�´� ��Ȯ���� �𸣰ٴ�
//                        //�׷��� ����غ��� 2.828 �� ���� ���� �ݶ��̴��� �������� Mathf.Sqrt(x,y) ������ �Ÿ��� ������� ���������� �������ε���
//                        float distance = Mathf.Sqrt(x * x + y * y);
//                        if (distance <= radius)
//                        {
//                            //xPos, yPos �� ���� ��ǥ, 1,1 = 1x1 ũ����� ���̵����͸� ��ȯ ���� 6���� �����ϸ� heights�� 6�� ��ȯ��
//                            float[,] heights = terrainData.GetHeights(xPos, yPos, 1, 1);

//                            //Mathf.Lerp(1, 0, distance / radius) distance�� radius�� ���ؼ� 0,1 �� depthFactor �� ����ش�
//                            float depthFactor = Mathf.Lerp(1, 0, distance / radius); // �Ÿ� ����Ͽ� ���� ����

//                            //digDepth = �ĳ����� ���� 0.2 �� �پ����
//                            //heights[0, 0] �浹�� �ͷ����� ���� 6�� ��ȯ�Ȼ��°� ���� 6�� �ͷ����� digDepth -0.2 *
//                            //digDepth �� 0,1�� ���ϱ� 0���� �۾����� �ʵ��� �ּҰ� ����
//                            heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // ���̸� ���Դϴ�.

//                            // ����� ���� ���� �����մϴ�.
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
    public float digDepth = 0.2f; // �ĳ� ���� ����

    void OnTriggerStay(Collider other)
    {
        // Ʈ���ſ� ������ ������Ʈ�� TerrainCollider���� Ȯ���մϴ�.
        TerrainCollider terrainCollider = other as TerrainCollider;
        if (terrainCollider != null)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            // Terrain �����͸� �����ɴϴ�.
            Terrain terrain = terrainCollider.GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPosition = terrain.transform.position;

            // Drill�� ��ġ�� �浹 �������� �����մϴ�.
            Vector3 collisionPoint = transform.position;

            // �浹 ������ ���� ��ǥ�� ���� �������� ���̸� ��ǥ�� ��ȯ�մϴ�.
            int xPos = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
            int yPos = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

            if (xPos >= 0 && xPos < terrainData.heightmapResolution && yPos >= 0 && yPos < terrainData.heightmapResolution)
            {
                float[,] heights = terrainData.GetHeights(xPos, yPos, 1, 1);
                heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth); // ���̸� ���Դϴ�.

                // ����� ���� ���� �����մϴ�.
                terrainData.SetHeights(xPos, yPos, heights);
            }
        }
    }
}

