using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseTotalView : BaseView
{
	[SerializeField] Transform ItemRoot;

	public static Action UpdateCaseView;

	private void OnEnable()
	{
		UpdateCaseView += Refresh;
	}

	private void OnDisable()
	{
		UpdateCaseView -= Refresh;
	}

	public override void Refresh()
	{
		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectAllParam("case");

		var dataList = DataBase.GetDataList<CaseData>(dataReader,"id","name","mask","content","master","contractid","casetype","customer","plaintiff","defendant","other","institution","money","paytype","paydes","createtime");
    	foreach(var data in dataList)
    	{
			var castItem = AssetManager.CreatePrefab("CaseItem",ItemRoot);
			var item = castItem.GetComponent<CaseItem>();
			if(item != null)
			{
				item.SetData(data);
			}

    	}
    	dataReader.Close();
	}
}
