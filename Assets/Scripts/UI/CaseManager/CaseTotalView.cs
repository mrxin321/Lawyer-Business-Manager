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

    	while(dataReader != null && dataReader.Read())
    	{
    		var caseId = dataReader.GetInt32(dataReader.GetOrdinal("id"));
    		var casename = dataReader.GetString(dataReader.GetOrdinal("name"));
    		
			var castItem = AssetManager.CreatePrefab("CaseItem",ItemRoot);

			var item = castItem.GetComponent<CaseItem>();
			if(item != null)
			{
				item.SetData(caseId);
			}

    	}
    	dataReader.Close();
	}
}
