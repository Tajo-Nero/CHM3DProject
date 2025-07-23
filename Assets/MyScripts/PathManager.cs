using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    // ���� ��� ����
    private List<Vector3> mainPath = new List<Vector3>();

    // ��� �ð�ȭ ����
    public bool visualizePath = true;
    public float pathWidth = 0.2f;
    public Color pathColor = Color.blue;

    // ���� ������ ����
    private LineRenderer pathRenderer;

    void Awake()
    {
        // ���� ������ �ʱ�ȭ
        pathRenderer = GetComponent<LineRenderer>();
        if (pathRenderer == null && visualizePath)
        {
            pathRenderer = gameObject.AddComponent<LineRenderer>();
            pathRenderer.startWidth = pathWidth;
            pathRenderer.endWidth = pathWidth;
            pathRenderer.material = new Material(Shader.Find("Sprites/Default"));
            pathRenderer.startColor = pathColor;
            pathRenderer.endColor = pathColor;
        }
    }

    // ��� ���� �Լ� (�÷��̾� ī ��忡�� ȣ��)
    public void SetMainPath(List<Vector3> newPath)
    {
        if (newPath == null || newPath.Count < 2)
        {
            Debug.LogWarning("��ȿ���� ���� ����Դϴ�!");
            return;
        }

        mainPath = new List<Vector3>(newPath);
        Debug.Log($"�� ��� ����: {mainPath.Count}�� ����");

        // ��� �ð�ȭ ������Ʈ
        UpdatePathVisualization();
    }

    // ��� �ð�ȭ ������Ʈ
    private void UpdatePathVisualization()
    {
        if (!visualizePath || pathRenderer == null || mainPath.Count < 2)
            return;

        pathRenderer.positionCount = mainPath.Count;
        for (int i = 0; i < mainPath.Count; i++)
        {
            pathRenderer.SetPosition(i, mainPath[i]);
        }
    }
    // PathManager.cs�� �߰�
    public bool HasValidPath()
    {
        // ��ΰ� ��ȿ���� Ȯ���ϴ� ����
        return (mainPath != null && mainPath.Count >= 2);
    }

    // ��� �������� �Լ� (WaveManager���� ȣ��)
    public List<Vector3> GetMainPath()
    {
        // ��ΰ� ��������� �⺻ ��� ����
        if (mainPath == null || mainPath.Count < 2)
        {
            return GenerateDefaultPath();
        }

        return new List<Vector3>(mainPath);
    }

    // �⺻ ��� ����
    private List<Vector3> GenerateDefaultPath()
    {
        List<Vector3> defaultPath = new List<Vector3>();

        // ���� ����Ʈ�� ã�� ���������� ���
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");
        if (spawnPoints.Length > 0)
        {
            defaultPath.Add(spawnPoints[0].transform.position);
        }
        else
        {
            defaultPath.Add(new Vector3(-10, 0, -10)); // �⺻ ������
        }

        // �ؼ����� ã�� �������� ���
        Nexus nexus = FindObjectOfType<Nexus>();
        if (nexus != null)
        {
            defaultPath.Add(nexus.transform.position);
        }
        else
        {
            defaultPath.Add(new Vector3(10, 0, 10)); // �⺻ ������
        }

        Debug.LogWarning("�⺻ ��� ������. �÷��̾� ī ���� ��θ� �����ϼ���!");
        return defaultPath;
    }

    // ��� �����
    public void ClearPath()
    {
        mainPath.Clear();

        if (pathRenderer != null)
        {
            pathRenderer.positionCount = 0;
        }
    }
}