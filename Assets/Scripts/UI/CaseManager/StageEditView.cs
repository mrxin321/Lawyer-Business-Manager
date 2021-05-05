using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEditView : BaseView
{	
	[SerializeField] InputField StageName;
	[SerializeField] InputField Des;
	[SerializeField] Dropdown Dropdown;
	[SerializeField] Text BtnText;

	private int CaseId;
    private StageData StageData;

	private List<CaseTypeData> CaseTypeList;

	public override void Refresh()
	{
		var caseType = PlayerPrefs.GetInt("InserStageCaseType",1);
		var _params = GetParams();
		if(_params.Length >= 1)CaseId = (int)_params[0];
		if(_params.Length >= 2)StageData = (StageData)_params[1];

		if(StageData != null)
		{
			BtnText.text = "修改阶段";
			StageName.text = StageData.Name;
			Des.text = StageData.Des;
			caseType = StageData.CaseType;
		}

		var dataReader_ = SqliteManager.Instance.SelectAllParam("casetype");

		var dataList = DataBase.GetDataList<CaseTypeData>(dataReader_,"id","name");
		CaseTypeList = dataList;
		Dropdown.options.Clear();

	   	for (int i = 0; i < dataList.Count; i++)
	    {
	       	var temoData = new Dropdown.OptionData();
	       	temoData.text = dataList[i].Name;
	        Dropdown.options.Add(temoData);

	        if(caseType == dataList[i].Id)Dropdown.value = i;
	    }
	}   
	
	public void StageEdit()
	{
		if(StageName.text == "")
		{
			ViewUtils.MessageTips("阶段名字不能空哦");
			return;
		}

		Hashtable hashtable = new Hashtable();

		if(StageData != null)
		{
			hashtable.Add(0,StageData.Id);
			hashtable.Add(1,StageName.text);
	        hashtable.Add(2,StageData.CaseId);
	        hashtable.Add(3,Des.text);
	        hashtable.Add(4,CaseTypeList[Dropdown.value].Id);
			var calNames1 = new string[]{"id","name","caseid","des","casetype"};
	        
			SqliteManager.Instance.UpateValue("stage",calNames1,hashtable);
			Utility.SafePostEvent(StageListlView.UpdateView);
			Close();
			return;
		}

		hashtable.Add(0,StageName.text);
        hashtable.Add(1,CaseId);
        hashtable.Add(2,Des.text);
	    hashtable.Add(3,CaseTypeList[Dropdown.value].Id);
		var calNames = new string[]{"name","caseid","des","casetype"};
		
		SqliteManager.Instance.InsertValue("stage",calNames,hashtable,(insertId)=>{

			if(insertId > 0)
			{
				Utility.SafePostEvent(StageListlView.UpdateView);
				Close();

				PlayerPrefs.SetInt("InserStageCaseType",CaseTypeList[Dropdown.value].Id);
			}
			
		});
		
	} 

}
