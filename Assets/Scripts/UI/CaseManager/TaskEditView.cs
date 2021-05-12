using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskEditView : BaseView
{
    [SerializeField] InputField Content;
    [SerializeField] InputField Des;
    [SerializeField] Transform Root;
    [SerializeField] Transform AddBtn;

    private TaskData TaskData;
    private int StageId;
    private List<GameObject> ToDoList = new List<GameObject>();

	public override void Refresh()
	{
		var _params = GetParams();

		if(_params.Length >= 1)StageId = (int)_params[0];
		if(_params.Length >= 2)TaskData = (TaskData)_params[1];

		if(TaskData != null)
		{
			Content.text = TaskData.Content;
			// Des.text = TaskData.Des;
			if(TaskData.TodoCount > 0)
			{
				for(int i =0;i < TaskData.TodoCount;i++)
				{
					TodoAdd();
				}
			}
		}
	}   
	
	public void TaskEdit()
	{
		if(Content.text == "")
		{
			ViewUtils.MessageTips("任务内容不能空哦");
			return;
		}

		Hashtable hashtable = new Hashtable();

		if(TaskData != null && TaskData.Id > 0)
		{
			hashtable.Add(0,TaskData.Id);
			hashtable.Add(1,Content.text);
			hashtable.Add(2,Des.text);
			hashtable.Add(3,GetToDoCount());
			var calNames1 = new string[]{"id","content","des","todocount"};
	        
			SqliteManager.Instance.UpateValue("task",calNames1,hashtable,()=>{
				Utility.SafePostEvent(TaskItem.TaskUpdate,TaskData.Id);
				Close();
				return;
			});
			
		}

		hashtable.Add(0,StageId);
		hashtable.Add(1,Content.text);
		hashtable.Add(2,Des.text);
		hashtable.Add(3,GetToDoCount());

		var calNames = new string[]{"stageid","content","des","todocount"};

		SqliteManager.Instance.InsertValue("task",calNames,hashtable,(insertId)=>{
			if(insertId > 0)
			{
				Utility.SafePostEvent(TaskListView.UpdateView);
				Close();
			}
		},true);
		
	} 

	private int GetToDoCount()
	{
		var count = 0;
		foreach(var item in ToDoList)
		{
			if(item != null)
			{
				count ++;
			}
		}
		return count;
	}
	public void TodoAdd()
	{
		if(GetToDoCount() >= 3)
		{
			ViewUtils.MessageTips("现在只支持3个待做哦!");
			return;
		}
		var todoItem = Utility.AddChildGameObjectAsLastSibling(Des.gameObject,Root);
		ToDoList.Add(todoItem);
		todoItem.SetActive(true);
		AddBtn.SetAsLastSibling();
	}
}
