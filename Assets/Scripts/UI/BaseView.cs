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
