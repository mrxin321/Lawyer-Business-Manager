using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskListView : BaseView
{
	public static Action UpdateView;

    [SerializeField] Transform ItemRoot;

    private int StageId;

    private void OnEnable()
	{
		UpdateView += Refresh;
	}

	private void OnDisable()
	{
		UpdateView -= Refresh;
	}

    public override void Refresh()
	{
		var _params = GetParams();

		if(_params.Length >= 1)StageId = (int)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectParam("task","stageid",StageId.ToString());

		var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","content","des","todocount");
		foreach(var taskData in dataList)
    	{
			var copyItem = AssetManager.CreatePrefab("TaskItem",ItemRoot);

			var item = copyItem.GetComponent<TaskItem>();
			if(item != null)
			{
				item.SetData(taskData);
			}

    	}
	}

	public void TaskAdd()
	{
        UIManager.Instance.OpenWindow("TaskEditView",StageId);
	}
}
