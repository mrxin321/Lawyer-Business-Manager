using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseView : MonoBehaviour
{
    // Start is called before the first frame update
    private Canvas mainCanvas;
    private static int Layer = 0;
    public ViewType ViewType = ViewType.NormalView;
    public bool RefreshView = true;

    void Start()
    {
    	if(mainCanvas == null)
    	{
    		mainCanvas = gameObject.GetComponent<Canvas>();
    	}
    	Layer += 1;
    	mainCanvas.sortingOrder = Layer;

        var groupPanel = transform.Find("Group");
        if(groupPanel != null)
        {
            var aspectRatioFitter = groupPanel.gameObject.GetComponentForce<AspectRatioFitter>();
            var fitInParentWithMinMax = groupPanel.gameObject.GetComponentForce<FitInParentWithMinMax>();

            if(ViewType == ViewType.NormalView )
            {
                var tweenScaleTemp = groupPanel.gameObject.GetComponent<TweenScale>();
                if(tweenScaleTemp != null)return;

                var tweenScale = groupPanel.gameObject.GetComponentForce<TweenScale>();
                tweenScale.from = new Vector3(0.5f,0.5f,1f);
                tweenScale.to = new Vector3(1f,1f,1f);
                tweenScale.duration = 0.2f;

                Keyframe[] ks = new Keyframe[3];  
                ks[0] = new Keyframe(0, 0);  
                ks[0].outTangent = 1;  
                ks[1] = new Keyframe(0.23f, 0.74f);  
                ks[1].inTangent = 1f;  
                ks[1].outTangent = 1f;  
                ks[2] = new Keyframe(1, 1);  
                ks[2].inTangent = 0;  
                tweenScale.animationCurve = new AnimationCurve(ks);  
            }
        }
    }

    public void Close()
    {
    	// GameObject.Destroy(gameObject);
    	UIManager.Instance.CloseWindow(this);
    }

    private object[] Params;
    public object[] GetParams()
    {
    	return Params;
    }
    public void SetParams(params object[] args)
    {
    	Params = args;
    }

    public virtual void Refresh()
    {}
}
