using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageItem : MonoBehaviour
{
    [SerializeField] Text StageName;

	private int StageId;
	private int CaseId;
    private StageData StageData;

    public void SetData(StageData stageData)
    { 
    	StageData = stageData;

		StageName.text = stageData.Name;
    }

    public void StateEdit()
    {
        UIManager.Instance.OpenWindow("StageEditView",0,StageData);
    }

    public void StateDelete()
    {
    	Hashtable hashtable = new Hashtable();
		hashtable.Add(0,StageData.Id);
        SqliteManager.Instance.DeleteRecord("stage","id",hashtable);
		SqliteManager.Instance.DeleteRecord("task","stageid",hashtable);

		Utility.SafePostEvent(StageListlView.UpdateView);
    }

    public void TaskList()
    {
        UIManager.Instance.OpenWindow("TaskListView",StageData);
    }
}
