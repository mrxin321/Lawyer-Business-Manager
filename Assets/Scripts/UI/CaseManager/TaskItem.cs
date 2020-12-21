using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskItem : MonoBehaviour
{
	public static Action<int> TaskUpdate;

    [SerializeField] Text TaskName;

	private TaskData TaskData;

	private void OnEnable()
	{
		TaskUpdate += Refresh;
	}

	private void OnDisable()
	{
		TaskUpdate -= Refresh;
	}

	private void Refresh(int taskId)
	{
		var dataReader = SqliteManager.Instance.SelectParam("task","id",TaskData.Id.ToString());

    	SetData(TaskData.GetTypeData<TaskData>(dataReader,"id","content","des","todocount"));
	}

    public void SetData(TaskData taskData)
    {
    	TaskData = taskData;

    	TaskName.text = TaskData.Content;
    }

    public void TaskEdit()
    {
        UIManager.Instance.OpenWindow("TaskEditView",0,TaskData);
    }

    public void TaskDelete()
    {
    	Hashtable hashtable = new Hashtable();
		hashtable.Add(0,TaskData.Id);
		SqliteManager.Instance.DeleteRecord("task","id",hashtable);

		Destroy(gameObject);
    }
}
