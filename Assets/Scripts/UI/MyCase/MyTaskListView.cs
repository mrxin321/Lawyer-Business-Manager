using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyTaskListView : BaseView
{
	[SerializeField] Transform ItemRoot;
	[SerializeField] Text CaseName;
	[SerializeField] Text StageName;

	private StageData StageData;

    public override void Refresh()
	{
		var _params = GetParams();
		if(_params.Length >= 1)StageData = (StageData)_params[0];

		CaseName.text = StageData.CaseName;
		StageName.text = StageData.Name;

		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectParam("task","stageid",StageData.Id.ToString());

		var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","des","content","stageid","state","todocount","todo1","todo2","todo3");
		foreach(var data in dataList)
    	{
			var copyItem = AssetManager.CreatePrefab("MyTaskItem",ItemRoot);

			var item = copyItem.GetComponent<MyTaskItem>();
			if(item != null)
			{
				item.SetData(data);
			}
    	}
	}
}
