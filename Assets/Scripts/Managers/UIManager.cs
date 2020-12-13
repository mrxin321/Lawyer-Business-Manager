using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[SerializeField] public Transform UIRoot;

	private List<BaseView> UIList = new List<BaseView>();

	public BaseView OpenWindow(string name,params object[] args)
	{
		var window = AssetManager.CreatePrefab(name);
		if(window != null)
		{
			if(UIRoot != null)
			{
				window.transform.SetParent(UIRoot);
			}

			var baseView = window.GetComponentForce<BaseView>();
			baseView.SetParams(args);
			baseView.Refresh();

			if(baseView.ViewType == ViewType.NormalView)
			{
				Debug.LogFormat("wtf y藏界面1 {0}",UIList.Count);
				if(UIList.Count > 0)
				{
					var lastWindow = UIList[UIList.Count - 1];
				Debug.LogFormat("wtf y藏界面2 {0}",lastWindow);

					if(lastWindow !=null && lastWindow.gameObject != null)
					{
						lastWindow.gameObject.SetActive(false);
					}
				}
			}

			UIList.Add(baseView);

			return baseView;
		}
		Debug.LogFormat("wtf 没有此界面{0}",name);
		return null;
	}

	public void CloseWindow(BaseView baseview)
	{
		if(baseview != null)
		{
			if(UIList.Count > 0 && UIList[UIList.Count - 1] == baseview && baseview.ViewType == ViewType.NormalView)
			{
				GameObject.Destroy(baseview.gameObject);
				UIList.RemoveAt(UIList.Count - 1);

				if(UIList.Count > 0)
				{
					var nextView = UIList[UIList.Count - 1];
					if(nextView != null)
					{
						nextView.gameObject.SetActive(true);
						nextView.Refresh();				
					}
				}
				return;
			}

			GameObject.Destroy(baseview.gameObject);
			UIList.RemoveAt(UIList.Count - 1);
		}
	}
}
