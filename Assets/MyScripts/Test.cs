//using Unity.VisualScripting;
//using UnityEngine;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
//using UnityEngine.UIElements;

//public class Test : MonoBehaviour
//{
//    public LineRenderer lineRenderer;
//    public int segments = 50;
//    public float attackRange = 5.0f;

//    public int conesegments = 20;
//    public float angle = 30.0f;
//    public float distance = 10.0f;
//    //라인렌더러
//    void Start()
//    {
//        //DrawAttackRange();

//    }

//    private void Update()
//    {
//        DrawCone();
//    }
//    void DrawAttackRange()
//    {
//        lineRenderer.positionCount = segments + 1;
//        lineRenderer.useWorldSpace = false;

//        float x;
//        float y;
//        float angle = 20f;

//        for (int i = 0; i < segments + 1; i++)
//        {
//            x = Mathf.Sin(Mathf.Deg2Rad * angle) * attackRange;
//            y = Mathf.Cos(Mathf.Deg2Rad * angle) * attackRange;

//            lineRenderer.SetPosition(i, new Vector3(x, 0, y));
//            angle += (360f / segments);
//        }
//    }

//    void DrawCone()
//    {
//        lineRenderer.positionCount = segments + 2;
//        Vector3 origin = transform.position;
//        float angleStep = angle / segments;
//        float currentAngle = -angle / 2.0f;

//        // 밑면 그리기
//        for (int i = 0; i <= segments; i++)
//        {
//            float rad = Mathf.Deg2Rad * currentAngle;
//            float x = Mathf.Sin(rad) * distance;
//            float z = Mathf.Cos(rad) * distance;
//            Vector3 vertex = origin + Quaternion.Euler(0, currentAngle, 0) * (transform.forward * distance);

//            lineRenderer.SetPosition(i, vertex);
//            currentAngle += angleStep;
//        }

//        // 원뿔의 꼭짓점
//        lineRenderer.SetPosition(segments + 1, origin);
//    }

//}
//원뿔형
//using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
//public class FanMeshGenerator : MonoBehaviour
//{
//    public int segments = 20;
//    public float angle = 30.0f; // 부채꼴의 각도
//    public float radius = 10.0f; // 부채꼴의 반지름

//    private void Start()
//    {
//        GenerateFan();
//    }

//    private void GenerateFan()
//    {
//        MeshFilter meshFilter = GetComponent<MeshFilter>();
//        Mesh mesh = new Mesh();
//        mesh.name = "Fan";

//        Vector3[] vertices = new Vector3[segments + 2]; 
//        int[] triangles = new int[segments * 3];

//        vertices[0] = Vector3.zero; // 중심점

//        float angleStep = angle / segments;
//        float currentAngle = -angle / 2.0f;

//        for (int i = 0; i <= segments; i++)
//        {
//            float rad = Mathf.Deg2Rad * currentAngle;
//            float x = Mathf.Sin(rad) * radius;
//            float z = Mathf.Cos(rad) * radius;
//            vertices[i + 1] = new Vector3(x, 0, z);
//            currentAngle += angleStep;
//        }

//        for (int i = 0; i < segments; i++)
//        {
//            triangles[i * 3] = 0;
//            triangles[i * 3 + 1] = i + 1;
//            triangles[i * 3 + 2] = i + 2;
//        }

//        mesh.vertices = vertices;
//        mesh.triangles = triangles;
//        mesh.RecalculateNormals();

//        meshFilter.mesh = mesh;
//    }

//원형
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CircleMeshGenerator : MonoBehaviour
{
    public int segments = 36;
    public float radius = 5f;

    private void Start()
    {
        GenerateCircle();
    }

    private void Update()
    {
        GenerateCircle();
    }

    private void GenerateCircle()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = "Circle";

        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        // 중심점
        vertices[0] = Vector3.zero;

        float angleStep = 360f / segments;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i] = new Vector3(x, 0, z);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i == segments - 1 ? 1 : i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}




