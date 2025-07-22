using UnityEngine;

public class TowerRangeDisplay : MonoBehaviour
{
    public enum RangeShape
    {
        Circle,
        Rectangle,
        Fan
    }

    [Header("Range Settings")]
    public RangeShape shape = RangeShape.Circle;
    public float range = 10f;
    public float fanAngle = 45f;  // ��ä�� ����
    public float rectangleWidth = 4f;  // ���簢�� ��
    public Material rangeMaterial;

    public GameObject rangeObject { get; private set; }
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        CreateRangeDisplay();
    }

    void CreateRangeDisplay()
    {
        rangeObject = new GameObject("RangeDisplay");
        rangeObject.transform.SetParent(transform);
        rangeObject.transform.localPosition = Vector3.up * 0.1f;

        meshFilter = rangeObject.AddComponent<MeshFilter>();
        meshRenderer = rangeObject.AddComponent<MeshRenderer>();

        // Material üũ ���� - ���߿� ������ ���� ����
        if (rangeMaterial != null)
        {
            meshRenderer.material = rangeMaterial;
        }
        // else �κ� ���� (���� ��� ����)

        UpdateRangeMesh();
    }

    public void UpdateRangeMesh()
    {
        if (meshFilter == null) return;

        Mesh mesh = null;

        switch (shape)
        {
            case RangeShape.Circle:
                mesh = CreateCircleMesh(range);
                break;
            case RangeShape.Rectangle:
                mesh = CreateRectangleMesh(range, rectangleWidth);
                break;
            case RangeShape.Fan:
                mesh = CreateFanMesh(range, fanAngle);
                break;
        }

        meshFilter.mesh = mesh;

    }

    // ���� �޽�
    Mesh CreateCircleMesh(float radius)
    {
        Mesh mesh = new Mesh();
        int segments = 50;

        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        Vector2[] uvs = new Vector2[segments + 1];

        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * 2 * Mathf.PI;
            vertices[i + 1] = new Vector3(
                Mathf.Sin(angle) * radius,
                0,
                Mathf.Cos(angle) * radius
            );

            uvs[i + 1] = new Vector2(
                Mathf.Sin(angle) * 0.5f + 0.5f,
                Mathf.Cos(angle) * 0.5f + 0.5f
            );

            if (i < segments - 1)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // ������ �ﰢ��
        triangles[(segments - 1) * 3] = 0;
        triangles[(segments - 1) * 3 + 1] = segments;
        triangles[(segments - 1) * 3 + 2] = 1;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }

    // ���簢�� �޽� (������ Ÿ����)
    Mesh CreateRectangleMesh(float length, float width)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];
        Vector2[] uvs = new Vector2[4];

        // Ÿ�� �������� ������� ���簢��
        vertices[0] = new Vector3(-width / 2, 0, 0);
        vertices[1] = new Vector3(width / 2, 0, 0);
        vertices[2] = new Vector3(-width / 2, 0, length);
        vertices[3] = new Vector3(width / 2, 0, length);

        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(1, 0);
        uvs[2] = new Vector2(0, 1);
        uvs[3] = new Vector2(1, 1);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }

    // ��ä�� �޽� (���� Ÿ����)
    Mesh CreateFanMesh(float radius, float angle)
    {
        Mesh mesh = new Mesh();
        int segments = 30;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        Vector2[] uvs = new Vector2[segments + 2];

        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + (halfAngle * 2 * i / segments);
            vertices[i + 1] = new Vector3(
                Mathf.Sin(currentAngle) * radius,
                0,
                Mathf.Cos(currentAngle) * radius
            );

            uvs[i + 1] = new Vector2(
                Mathf.Sin(currentAngle) * 0.5f + 0.5f,
                Mathf.Cos(currentAngle) * 0.5f + 0.5f
            );

            if (i < segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }

    // ���� ǥ��/�����
    public void ShowRange(bool show)
    {
        if (rangeObject != null)
            rangeObject.SetActive(show);
    }

    // ��Ÿ�ӿ� ��� ����
    public void ChangeShape(RangeShape newShape)
    {
        shape = newShape;
        UpdateRangeMesh();
    }

    

    // ȸ�� ������Ʈ (��ä�ð� ���簢����)
    void Update()
    {
        if (rangeObject != null && (shape == RangeShape.Fan || shape == RangeShape.Rectangle))
        {
            rangeObject.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }

    void OnDestroy()
    {
        if (rangeObject != null)
            Destroy(rangeObject);
    }
}