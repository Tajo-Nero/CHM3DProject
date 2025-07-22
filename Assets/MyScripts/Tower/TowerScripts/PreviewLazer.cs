using UnityEngine;

public class PreviewLaser : PreviewBase
{
    protected override void Start()
    {
        detectionRange = 20f;
        rangeWidth = 4f;
        rangeType = PreviewRangeType.Rectangle;

        base.Start();
    }
}