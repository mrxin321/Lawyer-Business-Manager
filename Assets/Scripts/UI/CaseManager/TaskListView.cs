using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskListView : BaseView
{
	public static Action UpdateView;

    [SerializeField] Transform ItemRoot;
	[SerializeField] Text StageName;

    private StageData StageData;

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

		if(_params.Length >= 1)StageData = (StageData)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		StageName.text = StageData.Name;

		var dataReader = SqliteManager.Instance.SelectParam("task","stageid",StageData.Id.ToString());

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

	public void StageDelete()
	{
		Action action = delegate{
			Hashtable hashtable = new Hashtable();
			hashtable.Add(0,StageData.Id);
	        SqliteManager.Instance.DeleteRecord("stage","id",hashtable,()=>{
	           Utility.SafePostEvent(StageListlView.UpdateView);
	        });

	        Close();
			SqliteManager.Instance.DeleteRecord("task","stageid",hashtable);
		};
    	UIManager.Instance.OpenWindow("MessageTipsConfirmView","是否要删除阶段？删除后将无法恢复，关联任务也将全部删除",action);
	}

	public void TaskAdd()
	{
        UIManager.Instance.OpenWindow("TaskEditView",StageData.Id);
	}
}
