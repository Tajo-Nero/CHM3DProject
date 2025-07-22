using UnityEngine;

public class PreviewCannon : PreviewBase
{
    protected override void Start()
    {
        detectionRange = 8f;
        rangeType = PreviewRangeType.Circle;

        base.Start();
    }
}