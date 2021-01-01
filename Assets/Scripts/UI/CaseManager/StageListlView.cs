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

	public void StageAdd()
	{
        UIManager.Instance.OpenWindow("StageEditView",CaseData.Id);
	}

	public void TempAdd()
	{
        UIManager.Instance.OpenWindow("TempStageChooseListView",CaseData);
	}
}
