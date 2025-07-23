using UnityEngine;

public class PreviewBuffTower : PreviewBase
{
    protected override void Start()
    {
        detectionRange = 10f;
        rangeType = PreviewRangeType.Circle;

        base.Start();
    }
}