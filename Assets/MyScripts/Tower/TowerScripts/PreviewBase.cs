using System.Collections;
using UnityEngine;

public abstract class PreviewBase : MonoBehaviour
{
    public enum PreviewRangeType
    {
        Circle,
        Rectangle,
        Fan
    }

    [SerializeField] protected PreviewRangeType rangeType = PreviewRangeType.Circle;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float rangeWidth = 4f; // Rectangle��
    [SerializeField] protected float fanAngle = 45f; // Fan��

    protected TowerRangeDisplay rangeDisplay;

    // PreviewBase.cs
    protected virtual void Start()
    {
        SetupPreviewRange();
    }


    protected virtual void SetupPreviewRange()
    {
        // �̹� �ִ��� Ȯ��
        rangeDisplay = GetComponent<TowerRangeDisplay>();

        if (rangeDisplay == null)
        {
            rangeDisplay = gameObject.AddComponent<TowerRangeDisplay>();
        }

        // ���� ������Ʈ
        rangeDisplay.range = detectionRange;

        // ��� ����
        switch (rangeType)
        {
            case PreviewRangeType.Circle:
                rangeDisplay.shape = TowerRangeDisplay.RangeShape.Circle;
                break;
            case PreviewRangeType.Rectangle:
                rangeDisplay.shape = TowerRangeDisplay.RangeShape.Rectangle;
                rangeDisplay.rectangleWidth = rangeWidth;
                break;
            case PreviewRangeType.Fan:
                rangeDisplay.shape = TowerRangeDisplay.RangeShape.Fan;
                rangeDisplay.fanAngle = fanAngle;
                break;
        }

        // Preview�� �׻� ���� ǥ��
        rangeDisplay.ShowRange(true);
    }

    protected virtual void Update()
    {
        // �ʿ�� ������Ʈ ����
    }
}