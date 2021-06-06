using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempStageChooseListView : BaseView
{
	[SerializeField] Transform ItemRoot;

	private void OnEnable()
	{
		TempStageChooseItem.TempStageAddSuccess += Close;
	}

	private void OnDisable()
	{
		TempStageChooseItem.TempStageAddSuccess -= Close;
	}

	private CaseData CaseData;

    public override void Refresh()
	{
		var _params = GetParams();

		if(_params.Length >= 1)CaseData = (CaseData)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		var sqlStr = "SELECT * FROM 'stage' where caseid = 0 and casetype = {0}";
		var dataReader = SqliteManager.Instance.SelectParam("stage",string.Format(sqlStr,CaseData.CaseType));
		var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des","casetype");
		foreach(var stage in dataList)
		{
			var copyItem = AssetManager.CreatePrefab("TempStageChooseItem",ItemRoot);
			var item = copyItem.GetComponent<TempStageChooseItem>();
			if(item != null)
			{
				stage.CaseId = CaseData.Id;
				item.SetData(stage,-1);
			}
		}

		//添加其他
		var otherItem = AssetManager.CreatePrefab("TempStageChooseItem",ItemRoot);
		var otherCell = otherItem.GetComponent<TempStageChooseItem>();
		if(otherCell != null)
		{
			otherCell.SetData(null,CaseData.Id);
		}
	}
}
