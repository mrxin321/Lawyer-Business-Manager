using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    // Start is called before the first frame update
    private Canvas mainCanvas;
    private static int Layer = 0;
    void Start()
    {
    	if(mainCanvas == null)
    	{
    		mainCanvas = gameObject.GetComponent<Canvas>();
    	}
    	Layer += 1;
    	mainCanvas.sortingOrder = Layer;
    }

    public void onClose()
    {
    	GameObject.Destroy(gameObject);
    }
}
