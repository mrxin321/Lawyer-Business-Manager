using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTaskListView : BaseView
{
	[SerializeField] Transform ItemRoot;

	private int StageId;

    public override void Refresh()
	{
		var _params = GetParams();
		if(_params.Length >= 1)StageId = (int)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectParam("task","stageid",StageId.ToString());

		var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","des","content");
		foreach(var data in dataList)
    	{
			var copyItem = AssetManager.CreatePrefab("TaskItem",ItemRoot);

			var item = copyItem.GetComponent<TaskItem>();
			if(item != null)
			{
				item.SetData(data);
			}

    	}
	}

	public void TaskAdd()
	{
        UIManager.Instance.OpenWindow("TaskEditView",StageId);
	}
}
