using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageListlView : BaseView
{
	public static Action UpdateView;

	[SerializeField] Transform ItemRoot;

	private int CaseId;

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
		if(_params.Length >= 1)CaseId = (int)_params[0];

		Utility.DestroyAllChildren(ItemRoot);
		
		var dataReader = SqliteManager.Instance.SelectParam("stage","caseid",CaseId.ToString());

    	while(dataReader != null && dataReader.Read())
    	{
    		var stageId = dataReader.GetInt32(dataReader.GetOrdinal("id"));
    		var caseId = dataReader.GetInt32(dataReader.GetOrdinal("caseid"));
    		
			var castItem = AssetManager.CreatePrefab("StageItem",ItemRoot);

			var item = castItem.GetComponent<StageItem>();
			if(item != null)
			{
				item.SetData(stageId,caseId);
			}
    	}
        dataReader.Close();
	}

	public void StageAdd()
	{
        UIManager.Instance.OpenWindow("StageEditView",CaseId);
	}

	public void TempAdd()
	{
        UIManager.Instance.OpenWindow("TempStageChooseListView",CaseId);
	}
}
