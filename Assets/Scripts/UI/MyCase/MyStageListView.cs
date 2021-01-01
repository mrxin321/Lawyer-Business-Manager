using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStageListView : BaseView
{
	[SerializeField] Transform ItemRoot;

	private int CaseId;

    public override void Refresh()
	{
		var _params = GetParams();
		if(_params.Length >= 1)CaseId = (int)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectParam("stage","caseid",CaseId.ToString());

		var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des","caseid","casetype");
		foreach(var data in dataList)
    	{
			var copyItem = AssetManager.CreatePrefab("MyStageItem",ItemRoot);

			var item = copyItem.GetComponent<MyStageItem>();
			if(item != null)
			{
				item.SetData(data);
			}

    	}
	}
	
}
