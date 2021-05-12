using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TodoItem : MonoBehaviour
{
    [SerializeField] Text CaseName;
    [SerializeField] Text TaskContent;
	[SerializeField] Dropdown Dropdown;

	private TaskData TaskData;

    public void SetData(TaskData taskData)
    {
        TaskData = taskData;
        
        if(taskData != null)
        { 
            TaskContent.text = taskData.Content;
            CaseName.text = string.Format("{0}案件{1}阶段",taskData.CaseName,taskData.StageName);
            Dropdown.value = taskData.State;
    	}
    }

    public void SelectChange()
    {
        var hashtable = new Hashtable();

        hashtable.Add(0,TaskData.Id);
        hashtable.Add(1,Dropdown.value);
        var calNames1 = new string[]{"id","state"};
        
        SqliteManager.Instance.UpateValue("task",calNames1,hashtable,()=>{
            if(Dropdown.value == 2)
            {
                Utility.SafePostEvent(ToDoListView.UpdateView);
            }
        });

        
    }
}
