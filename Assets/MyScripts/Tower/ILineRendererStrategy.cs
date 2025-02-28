using UnityEngine;

public interface ILineRendererStrategy
{
    void Setup(LineRenderer lineRenderer);
    void GeneratePattern(LineRenderer lineRenderer, Vector3 origin, Transform launchPoint, int segments, float parameter, float length);
}

public class CircleRendererStrategy : ILineRendererStrategy
{
    public void Setup(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.loop = true;
    }

    public void GeneratePattern(LineRenderer lineRenderer, Vector3 origin, Transform launchPoint, int segments, float radius, float length)
    {
        lineRenderer.positionCount = segments + 1;
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, origin + new Vector3(x, 0, z));
        }
    }
}

public class FanRendererStrategy : ILineRendererStrategy
{
    public void Setup(LineRenderer lineRenderer)
    {
        // Setup �ڵ�
    }

    public void GeneratePattern(LineRenderer lineRenderer, Vector3 origin, Transform launchPoint, int segments, float coneAngle, float length)
    {
        lineRenderer.positionCount = segments + 2;
        float angleStep = coneAngle / (segments - 1);
        lineRenderer.SetPosition(0, origin);

        for (int i = 0; i <= segments; i++)
        {
            float angle = (angleStep * i - coneAngle / 2) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * length;
            float z = Mathf.Sin(angle) * length;
            Vector3 position = origin + launchPoint.forward * x + launchPoint.right * z;
            lineRenderer.SetPosition(i + 1, position);
        }

        lineRenderer.SetPosition(segments + 1, origin);
    }
}

public class LaserRendererStrategy : ILineRendererStrategy
{
    public void Setup(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    public void GeneratePattern(LineRenderer lineRenderer, Vector3 origin, Transform launchPoint, int segments, float detectionRange, float length)
    {
        Vector3 boxCenter = launchPoint.position + launchPoint.forward * (detectionRange / 2);
        Vector3 boxSize = new Vector3(2f, 2f, detectionRange);

        Vector3[] vertices = new Vector3[8];

        vertices[0] = launchPoint.position + new Vector3(0, 1, 0) + launchPoint.right * (boxSize.x / 2) + launchPoint.up * (boxSize.y / 2);
        vertices[1] = launchPoint.position + new Vector3(0, 1, 0) + launchPoint.right * (boxSize.x / 2) - launchPoint.up * (boxSize.y / 2);
        vertices[2] = launchPoint.position + new Vector3(0, 1, 0) - launchPoint.right * (boxSize.x / 2) - launchPoint.up * (boxSize.y / 2);
        vertices[3] = launchPoint.position + new Vector3(0, 1, 0) - launchPoint.right * (boxSize.x / 2) + launchPoint.up * (boxSize.y / 2);

        vertices[4] = boxCenter + new Vector3(0, 1, 0) + launchPoint.right * (boxSize.x / 2) + launchPoint.up * (boxSize.y / 2);
        vertices[5] = boxCenter + new Vector3(0, 1, 0) + launchPoint.right * (boxSize.x / 2) - launchPoint.up * (boxSize.y / 2);
        vertices[6] = boxCenter + new Vector3(0, 1, 0) - launchPoint.right * (boxSize.x / 2) - launchPoint.up * (boxSize.y / 2);
        vertices[7] = boxCenter + new Vector3(0, 1, 0) - launchPoint.right * (boxSize.x / 2) + launchPoint.up * (boxSize.y / 2);

        lineRenderer.positionCount = 24;

        lineRenderer.SetPosition(0, vertices[0]);
        lineRenderer.SetPosition(1, vertices[1]);
        lineRenderer.SetPosition(2, vertices[1]);
        lineRenderer.SetPosition(3, vertices[2]);
        lineRenderer.SetPosition(4, vertices[2]);
        lineRenderer.SetPosition(5, vertices[3]);
        lineRenderer.SetPosition(6, vertices[3]);
        lineRenderer.SetPosition(7, vertices[0]);

        lineRenderer.SetPosition(8, vertices[4]);
        lineRenderer.SetPosition(9, vertices[5]);
        lineRenderer.SetPosition(10, vertices[5]);
        lineRenderer.SetPosition(11, vertices[6]);
        lineRenderer.SetPosition(12, vertices[6]);
        lineRenderer.SetPosition(13, vertices[7]);
        lineRenderer.SetPosition(14, vertices[7]);
        lineRenderer.SetPosition(15, vertices[4]);

        lineRenderer.SetPosition(16, vertices[0]);
        lineRenderer.SetPosition(17, vertices[4]);
        lineRenderer.SetPosition(18, vertices[1]);
        lineRenderer.SetPosition(19, vertices[5]);
        lineRenderer.SetPosition(20, vertices[2]);
        lineRenderer.SetPosition(21, vertices[6]);
        lineRenderer.SetPosition(22, vertices[3]);
        lineRenderer.SetPosition(23, vertices[7]);
    }
}

