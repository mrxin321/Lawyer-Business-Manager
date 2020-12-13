using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTaskListView : BaseView
{
	[SerializeField] Transform ItemRoot;

	private int StageId;

    public override void Refresh()
	{
		var _params = GetParams();
		if(_params.Length >= 1)StageId = (int)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectParam("task","stageid",StageId.ToString());

		var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","des","content","stageid","state");
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
