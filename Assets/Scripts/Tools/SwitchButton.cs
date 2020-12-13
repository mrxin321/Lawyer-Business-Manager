using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class SwitchButton : Button
{
	public string UIName;
	public GameObject ViewObj;
	
	protected override void OnEnable()
	{
		onClick.AddListener(OpenWindow);
	}

	protected override void OnDisable()
	{
		onClick.RemoveListener(OpenWindow);
	}

    private void OpenWindow()
    {
    	Debug.LogFormat("Wtf OpenWindowOpenWindow {0}",UIName);
    	if(!string.IsNullOrEmpty(UIName))
    	{
    		UIManager.Instance.OpenWindow(UIName);
    	}
    }
}
