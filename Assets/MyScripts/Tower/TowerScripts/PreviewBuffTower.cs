using UnityEngine;

public class PreviewBuffTower : PreviewBase
{
    protected override void Start()
    {
        previewColor = Color.green;
        detectionRange = 10f;
        rangeType = PreviewRangeType.Circle;

        base.Start();
    }
}