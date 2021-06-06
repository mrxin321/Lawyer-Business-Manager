using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageListlView : BaseView
{
	public static Action UpdateView;

	[SerializeField] Transform ItemRoot;
	[SerializeField] Text CaseName;

	private CaseData CaseData;

	private void OnEnable()
	{
		UpdateView += Refresh;
		TempStageChooseItem.TempStageAddSuccess += Refresh;
	}

	private void OnDisable()
	{
		UpdateView -= Refresh;
		TempStageChooseItem.TempStageAddSuccess -= Refresh;
	}

    public override void Refresh()
	{
		var _params = GetParams();
		if(_params.Length >= 1)CaseData = (CaseData)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		CaseName.text = CaseData.Name;

		var dataReader = SqliteManager.Instance.SelectParam("stage","caseid",CaseData.Id.ToString());
		var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des","caseid","casetype");
		foreach(var data in dataList)
    	{
			var castItem = AssetManager.CreatePrefab("StageItem",ItemRoot);

			var item = castItem.GetComponent<StageItem>();
			if(item != null)
			{
				item.SetData(data);
			}
    	}
	}

	public void CaseDelete()
	{
		Action action = delegate{
			Hashtable hashtable = new Hashtable();
			hashtable.Add(0,CaseData.Id);

	        var deleteSql = "delete from task where stageid in(select id from stage where stage.caseid = {0})";
	        SqliteManager.Instance.DeleteRecord("task",string.Format(deleteSql,CaseData.Id.ToString()));
	        
	        SqliteManager.Instance.DeleteRecord("case","id",hashtable,()=>{
	            Utility.SafePostEvent(MyCaseListView.RefreshMyCaseEvent);
				Close();
	        });
			SqliteManager.Instance.DeleteRecord("stage","caseid",hashtable);

			
		};
    	UIManager.Instance.OpenWindow("MessageTipsConfirmView","是否要删除案件？删除后将无法恢复，关联阶段跟任务也将全部删除",action);
	}

	public void StageAdd()
	{
        UIManager.Instance.OpenWindow("StageEditView",CaseData.Id);
	}

	public void TempAdd()
	{
        UIManager.Instance.OpenWindow("TempStageChooseListView",CaseData);
	}
}
