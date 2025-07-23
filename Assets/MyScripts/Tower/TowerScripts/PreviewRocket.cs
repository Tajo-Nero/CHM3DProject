using UnityEngine;

public class PreviewRocket : PreviewBase
{
    protected override void Start()
    {
        detectionRange = 10f;
        fanAngle = 45f;
        rangeType = PreviewRangeType.Fan;

        base.Start();
    }
}