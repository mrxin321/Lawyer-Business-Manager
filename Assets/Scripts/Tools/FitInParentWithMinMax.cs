using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
public class FitInParentWithMinMax : MonoBehaviour
{
    #region Fields

    public float MinWidthHighRate = 0.48f;

    public float MaxWidthHighRate = 17.5f/9.0f;

    #endregion

    #region Methods

    protected void Start()
    {
        var parentRectTransform = transform.parent.GetComponent<RectTransform>();
        var rate = parentRectTransform.rect.width/parentRectTransform.rect.height;
        rate = Mathf.Clamp(rate, MinWidthHighRate, MaxWidthHighRate);
        var aspectRatioFitter = GetComponent<AspectRatioFitter>();
        aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        aspectRatioFitter.aspectRatio = rate;
    }

    #endregion
}
