using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempStageChooseItem : MonoBehaviour
{
	public static Action TempStageAddSuccess;

    [SerializeField] Text StageName;

    private StageData StageData;	
    private int CaseId;

    public void SetData(StageData stageData,int caseId)
    {
    	if(stageData == null)
    	{
    		CaseId = caseId;
    		StageName.text = "其他";
    		return;
    	}
    	StageData = stageData;
    	StageName.text = stageData.Name;
    }

    public void StateChoose()
    {
    	if(StageData == null)
    	{
    		Utility.SafePostEvent(TempStageAddSuccess);
    		UIManager.Instance.OpenWindow("StageEditView",CaseId);
    		return;
    	}
    	//添加阶段
    	Hashtable hashtable = new Hashtable();
		hashtable.Add(0,StageData.Name);
        hashtable.Add(1,StageData.CaseId);
        hashtable.Add(2,StageData.Des);
		var calNames = new string[]{"name","caseid","des"};

		SqliteManager.Instance.InsertValue("stage",calNames,hashtable,(stageId)=>{
			if(stageId > 0)
			{
				var dataReader = SqliteManager.Instance.SelectParam("task","stageid",StageData.Id.ToString());

				var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","content","des");
				foreach(var taskData in dataList)
		    	{
					//添加任务
					var thashtable = new Hashtable();
					thashtable.Add(0,stageId);
					thashtable.Add(1,taskData.Content);
					thashtable.Add(2,taskData.Des);
					var tcalNames = new string[]{"stageid","content","des"};
					SqliteManager.Instance.InsertValue("task",tcalNames,thashtable);
		    	}
			}

			ViewUtils.MessageTips("添加模板成功!!!");
			Utility.SafePostEvent(TempStageAddSuccess);
		});
    }
}
