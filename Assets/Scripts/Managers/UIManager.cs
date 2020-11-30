using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[SerializeField] public Transform UIRoot;

	private List<GameObject> UIList = new List<GameObject>();

	public GameObject OpenWindow(string name)
	{
		var loader = new AssetNameLoader();
        loader.LoadAssetText();

        var data = loader.GetAssetMapInfo(name);
		var window = AssetManager.CreatePrefab(data.assetPath);
		if(window != null)
		{
			if(UIRoot != null)
			{
				window.transform.SetParent(UIRoot);
			}
			UIList.Add(window);
		}
		return window;
	}
}
