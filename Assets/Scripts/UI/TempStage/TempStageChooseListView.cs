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

	private int CaseId;

    public override void Refresh()
	{
		var _params = GetParams();

		if(_params.Length >= 1)CaseId = (int)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectParam("stage","caseid","0");

		var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des");
		foreach(var stage in dataList)
		{
			var copyItem = AssetManager.CreatePrefab("TempStageChooseItem",ItemRoot);
			var item = copyItem.GetComponent<TempStageChooseItem>();
			if(item != null)
			{
				stage.CaseId = CaseId;
				item.SetData(stage);
			}
		}
	}
}
