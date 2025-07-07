using UnityEngine;

public class PreviewRocket : PreviewBase
{
    protected override void Start()
    {
        previewColor = Color.grey;
        detectionRange = 10f;
        rangeType = PreviewRangeType.Fan;

        base.Start();
    }
}