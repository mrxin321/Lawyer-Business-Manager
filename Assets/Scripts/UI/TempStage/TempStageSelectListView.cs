using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempStageSelectListView : BaseView
{
	[SerializeField] Transform ItemRoot;

	private int CaseType;

    public override void Refresh()
	{
		var _params = GetParams();

		var map = new Dictionary<int,bool>();
		if(_params.Length >= 1)CaseType = (int)_params[0];
		if(_params.Length >= 2)
		{
			var stageList = (List<int>)_params[1];
			foreach(var item in stageList)
			{
				map.Add(item,true);
			}
		}

		Utility.DestroyAllChildren(ItemRoot);
		
		var sqlStr = "SELECT * FROM 'stage' where caseid = 0 and casetype = {0}";
		var dataReader = SqliteManager.Instance.SelectParam("stage",string.Format(sqlStr,CaseType));
		var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des","casetype");
		foreach(var stage in dataList)
		{
			var copyItem = AssetManager.CreatePrefab("TempStageSelectItem",ItemRoot);
			var item = copyItem.GetComponent<TempStageSelectItem>();
			if(item != null)
			{
				item.SetData(stage,map.ContainsKey(stage.Id));
			}
		}
	}
}
