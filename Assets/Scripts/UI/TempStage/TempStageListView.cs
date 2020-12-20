using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempStageListView : BaseView
{
	[SerializeField] Transform ItemRoot;

	private void OnEnable()
	{
		StageListlView.UpdateView += Refresh;
	}

	private void OnDisable()
	{
		StageListlView.UpdateView -= Refresh;
	}

    public override void Refresh()
	{
		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectParam("stage","caseid","0");

		var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des");
		foreach(var data in dataList)
    	{
			var copyItem = AssetManager.CreatePrefab("StageItem",ItemRoot);
			var item = copyItem.GetComponent<StageItem>();
			if(item != null)
			{
				item.SetData(data.Id,0);
			}
    	}
	}

	public void StageAdd()
	{
        UIManager.Instance.OpenWindow("StageEditView",0);
	}
	
}
