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
    [SerializeField] protected float rangeWidth = 4f; // Rectangle용
    [SerializeField] protected float fanAngle = 45f; // Fan용

    protected TowerRangeDisplay rangeDisplay;

    // PreviewBase.cs
    protected virtual void Start()
    {
        SetupPreviewRange();
    }


    protected virtual void SetupPreviewRange()
    {
        // 이미 있는지 확인
        rangeDisplay = GetComponent<TowerRangeDisplay>();

        if (rangeDisplay == null)
        {
            rangeDisplay = gameObject.AddComponent<TowerRangeDisplay>();
        }

        // 설정 업데이트
        rangeDisplay.range = detectionRange;

        // 모양 설정
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

        // Preview는 항상 범위 표시
        rangeDisplay.ShowRange(true);
    }

    protected virtual void Update()
    {
        // 필요시 업데이트 로직
    }
}