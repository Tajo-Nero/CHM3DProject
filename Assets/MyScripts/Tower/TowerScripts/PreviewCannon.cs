using UnityEngine;

public class PreviewCannon : PreviewBase
{
    protected override void Start()
    {
        previewColor = Color.red;
        detectionRange = 8f;
        rangeType = PreviewRangeType.Circle;

        base.Start();
    }
}