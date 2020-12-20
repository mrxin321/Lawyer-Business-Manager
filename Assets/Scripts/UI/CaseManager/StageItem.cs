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

    public void SetData(int stageId,int caseId)
    {
    	StageId = stageId;
    	CaseId = caseId;

        ViewUtils.Print("wtf data tatata  {0}",stageId);

		var dataReader = SqliteManager.Instance.SelectParam("stage",string.Format("select * from 'stage' where id = {0}",stageId));
		while(dataReader != null && dataReader.Read())
    	{
    		var stagename = dataReader.GetString(dataReader.GetOrdinal("name"));
    		StageName.text = stagename;
    	}
    	dataReader.Close();
    }

    public void StateEdit()
    {
        UIManager.Instance.OpenWindow("StageEditView",CaseId,StageId);
    }

    public void StateDelete()
    {
    	Hashtable hashtable = new Hashtable();
		hashtable.Add(0,StageId);
        SqliteManager.Instance.DeleteRecord("stage","id",hashtable);
		SqliteManager.Instance.DeleteRecord("task","stageid",hashtable);

		Utility.SafePostEvent(StageListlView.UpdateView);
    }

    public void TaskList()
    {
        UIManager.Instance.OpenWindow("TaskListView",StageId);
    }
}
