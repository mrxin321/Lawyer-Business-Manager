using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempStageListView : BaseView
{
	[SerializeField] Transform ItemRoot;
	[SerializeField] Dropdown Dropdown;

	private List<CaseTypeData> CaseTypeList;

	private void OnEnable()
	{
		StageListlView.UpdateView += Refresh;
	}

	private void OnDisable()
	{
		StageListlView.UpdateView -= Refresh;
	}

    public override void Refresh()
	{
    	RefreshDropdown();
    	RefreshChooseStage();
	}

	public void OnValueChange(int value)
	{
		RefreshChooseStage();
	}	

	private void RefreshChooseStage()
	{
		Utility.DestroyAllChildren(ItemRoot);
		
		var caseType = CaseTypeList[Dropdown.value].Id;
		var sql  = "SELECT * FROM '{0}' where {1} = {2} and {3} = {4}";
		var dataReader = SqliteManager.Instance.SelectParam("stage",string.Format(sql,"stage","caseid","0","casetype",caseType.ToString()));

		var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des","casetype");
		foreach(var data in dataList)
    	{
			var copyItem = AssetManager.CreatePrefab("StageItem",ItemRoot);
			var item = copyItem.GetComponent<StageItem>();
			if(item != null)
			{
				item.SetData(data);
			}
    	}
	}

	private void RefreshDropdown()
	{
		var dataReader_ = SqliteManager.Instance.SelectAllParam("casetype");
		var dataList = DataBase.GetDataList<CaseTypeData>(dataReader_,"id","name");
		Dropdown.options.Clear();
		CaseTypeList = dataList;
	   	for (int i = 0; i < dataList.Count; i++)
	    {
	       	var temoData = new Dropdown.OptionData();
	       	temoData.text = dataList[i].Name;
	        Dropdown.options.Add(temoData);
	    }
	}

	public void StageAdd()
	{
        UIManager.Instance.OpenWindow("StageEditView",0);
	}
	
}
