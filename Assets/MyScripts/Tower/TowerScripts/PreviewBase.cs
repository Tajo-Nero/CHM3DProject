using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class PreviewBase : MonoBehaviour
{
    [Header("Preview Settings")]
    [SerializeField] protected Material previewMaterial;
    [SerializeField] protected Color previewColor = Color.white;

    public enum PreviewRangeType
    {
        Circle,
        Rectangle,
        Fan
    }

    [SerializeField] protected PreviewRangeType rangeType = PreviewRangeType.Circle;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float rangeWidth = 4f; // Rectangle용

    protected DecalProjector rangeDecal;

    protected virtual void Start()
    {
        CreatePreviewTexture();
        SetupPreviewDecal();
    }

    protected virtual void CreatePreviewTexture()
    {
        Texture2D texture;

        switch (rangeType)
        {
            case PreviewRangeType.Circle:
                texture = CreateCircleTexture();
                break;
            case PreviewRangeType.Rectangle:
                texture = CreateRectangleTexture();
                break;
            case PreviewRangeType.Fan:
                texture = CreateFanTexture();
                break;
            default:
                texture = CreateCircleTexture();
                break;
        }

        // Material 생성
        //previewMaterial = new Material(Shader.Find("Universal Render Pipeline/Decal"));
        //previewMaterial.SetTexture("_BaseMap", texture);
        //previewMaterial.color = new Color(previewColor.r, previewColor.g, previewColor.b, 0.5f);
        //previewMaterial.renderQueue = 3000;
    }

    protected virtual void SetupPreviewDecal()
    {
        // 범위 표시 전용 GameObject 생성
        GameObject rangeDisplayObject = new GameObject($"{gameObject.name}_PreviewRange");
        rangeDisplayObject.transform.SetParent(transform);

        // 위치와 회전 설정
        rangeDisplayObject.transform.localPosition = Vector3.up * 3f;
        rangeDisplayObject.transform.localRotation = Quaternion.Euler(90, 0, 0);

        // Decal Projector 추가
        rangeDecal = rangeDisplayObject.AddComponent<DecalProjector>();
        rangeDecal.enabled = true; // Preview는 항상 보임
        rangeDecal.material = previewMaterial;

        SetDecalSize();
    }

    protected virtual void SetDecalSize()
    {
        switch (rangeType)
        {
            case PreviewRangeType.Circle:
                rangeDecal.size = new Vector3(detectionRange * 2, detectionRange * 2, 10f);
                break;

            case PreviewRangeType.Rectangle:
                rangeDecal.size = new Vector3(rangeWidth, detectionRange, 10f);
                break;

            case PreviewRangeType.Fan:
                rangeDecal.size = new Vector3(detectionRange * 2, detectionRange * 2, 10f);
                if (rangeDecal != null)
                {
                    rangeDecal.transform.localRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
                }
                break;
        }
    }

    // 텍스처 생성 메서드들 (TowerBase와 동일)
    protected Texture2D CreateCircleTexture()
    {
        int size = 128; // Preview용은 작게
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.4f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = 1f - Mathf.SmoothStep(radius * 0.7f, radius, distance);

                // 점선 효과 (Preview 느낌)
                float dashPattern = Mathf.Sin(distance * 0.5f) * 0.3f + 0.7f;
                alpha *= dashPattern;

                alpha = Mathf.Clamp01(alpha);
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        texture.Apply();
        return texture;
    }

    protected Texture2D CreateRectangleTexture()
    {
        int width = 64;
        int height = 256;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float fadeX = Mathf.Min(x / 5f, (width - x) / 5f);
                float fadeY = Mathf.Min(y / 10f, (height - y) / 10f);
                float alpha = Mathf.Clamp01(Mathf.Min(fadeX, fadeY));

                // 점선 효과
                if ((x + y / 4) % 8 < 4)
                {
                    alpha *= 0.6f;
                }

                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        texture.Apply();
        return texture;
    }

    protected Texture2D CreateFanTexture()
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

        Vector2 center = new Vector2(size * 0.5f, size * 0.8f);
        float maxRadius = size * 0.6f;
        float fanAngle = 60f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 direction = new Vector2(x, y) - center;
                float distance = direction.magnitude;
                float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

                bool inAngleRange = Mathf.Abs(angle) <= fanAngle / 2;

                if (inAngleRange && distance <= maxRadius && distance >= 5f)
                {
                    float distanceFade = 1f - Mathf.SmoothStep(maxRadius * 0.6f, maxRadius, distance);
                    float angleFade = 1f - Mathf.SmoothStep(fanAngle / 2 * 0.6f, fanAngle / 2, Mathf.Abs(angle));

                    // 방사형 점선 효과
                    float radialDash = Mathf.Sin(distance * 0.3f + angle * 0.1f) * 0.4f + 0.6f;

                    float alpha = distanceFade * angleFade * radialDash;
                    alpha = Mathf.Clamp01(alpha);

                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, new Color(1, 1, 1, 0));
                }
            }
        }

        texture.Apply();
        return texture;
    }

    protected virtual void Update()
    {
        // Fan 타입의 경우 회전 업데이트
        if (rangeType == PreviewRangeType.Fan && rangeDecal != null)
        {
            rangeDecal.transform.localRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
        }
    }
}