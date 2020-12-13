using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskEditView : BaseView
{
    [SerializeField] InputField Content;
    [SerializeField] InputField Des;

    private TaskData TaskData;
    private int StageId;
	public override void Refresh()
	{
		var _params = GetParams();

		if(_params.Length >= 1)StageId = (int)_params[0];
		if(_params.Length >= 2)TaskData = (TaskData)_params[1];

		if(TaskData != null)
		{
			Content.text = TaskData.Content;
			Des.text = TaskData.Des;
		}
	}   
	
	public void TaskEdit()
	{
		if(Content.text == "")ViewUtils.MessageTips("任务内容不能空哦");

		Hashtable hashtable = new Hashtable();

		if(TaskData != null && TaskData.Id > 0)
		{
			hashtable.Add(0,TaskData.Id);
			hashtable.Add(1,Content.text);
			hashtable.Add(2,Des.text);
			var calNames1 = new string[]{"id","content","des"};
	        
			SqliteManager.Instance.UpateValue("task",calNames1,hashtable);
			Utility.SafePostEvent(TaskItem.TaskUpdate,TaskData.Id);
			Close();
			return;
		}

		hashtable.Add(0,StageId);
		hashtable.Add(1,Content.text);
		hashtable.Add(2,Des.text);
		var calNames = new string[]{"stageid","content","des"};

		SqliteManager.Instance.InsertValue("task",calNames,hashtable);
		Utility.SafePostEvent(TaskListView.UpdateView);
		Close();
	} 
}
