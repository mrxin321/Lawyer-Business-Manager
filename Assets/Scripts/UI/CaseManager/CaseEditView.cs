using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseEditView : BaseView	
{
	[SerializeField] InputField CaseName;
	[SerializeField] InputField ContractId;
	[SerializeField] InputField Mask;
	[SerializeField] Dropdown Dropdown;
	[SerializeField] Dropdown MasterDropdown;
    [SerializeField] Text BtnName;
    [SerializeField] Text DataTimeText;
	[SerializeField] InputField Customer;
	[SerializeField] InputField Plaintiff;
	[SerializeField] InputField Defendant;
	[SerializeField] InputField Other;
	[SerializeField] InputField Institution;
	[SerializeField] InputField Money;
	[SerializeField] Dropdown Paytype;
	[SerializeField] InputField Paydes;

	private CaseData CaseData;
	private List<CaseTypeData> CaseTypeList;
	private string DataTime;

	public override void Refresh()
	{
		DataTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

		DataTimeText.text = DataTime;

		var _params = GetParams();
		var caseType = 1;
		var payType = 1;
		if(_params.Length > 0)
		{
			CaseData = (CaseData)_params[0];

			CaseName.text = CaseData.Name;
			ContractId.text = CaseData.ContractId;
			Mask.text = CaseData.Mask.ToString();
		    caseType = CaseData.CaseType;
		    payType = CaseData.Paytype;
		    Customer.text      = CaseData.Customer;
			Plaintiff.text = CaseData.Plaintiff;
			Defendant.text = CaseData.Defendant;
			Other.text = CaseData.Other;
			Institution.text = CaseData.Institution;
			Money.text = CaseData.Money;
			Paydes.text = CaseData.Paydes;

		    BtnName.text = "修改案子";
		}

		var dataReader_ = SqliteManager.Instance.SelectAllParam("casetype");

		var dataList = DataBase.GetDataList<CaseTypeData>(dataReader_,"id","name");
		Dropdown.options.Clear();
		CaseTypeList = dataList;
	   	for (int i = 0; i < dataList.Count; i++)
	    {
	       	var temoData = new Dropdown.OptionData();
	       	temoData.text = dataList[i].Name;
	        Dropdown.options.Add(temoData);

	        if(caseType == dataList[i].Id)Dropdown.value = i;
	    }

	    var paytypeReader = SqliteManager.Instance.SelectAllParam("paytype");

		var payTypeList = DataBase.GetDataList<PayTypeData>(paytypeReader,"id","name");

	    ViewUtils.SetPayTypeDropdown(Paytype,payTypeList,payType - 1);

	    Utility.DoNextFrame(()=>{
		    MasterDropdown.captionText.text = PlayerDataManager.Instance.GetUserName();
	    });
	}

	public void OnValueChange(int value)
	{
		Utility.DoNextFrame(()=>{
		    MasterDropdown.captionText.text = PlayerDataManager.Instance.GetUserName();
	    });
	}

	public void OnSaveClick()
	{
		if(ContractId.text == "")
		{
			ViewUtils.MessageTips("项目编号不能空哦");
			return;
		}

		if(CaseName.text == "")
		{
			ViewUtils.MessageTips("项目名字不能空哦");
			return;
		}

		Hashtable hashtable = new Hashtable();

		if(CaseData != null)
		{
			hashtable.Add(0,CaseData.Id);
			hashtable.Add(1,CaseName.text);
	        hashtable.Add(2,ContractId.text);
	        hashtable.Add(3,Mask.text);
	        hashtable.Add(4,CaseTypeList[Dropdown.value].Id);
	        hashtable.Add(5,Customer.text);
	        hashtable.Add(6,Plaintiff.text);
	        hashtable.Add(7,Defendant.text);
	        hashtable.Add(8,Other.text);
	        hashtable.Add(9,Institution.text);
	        hashtable.Add(10,Money.text);
	        hashtable.Add(11,Paytype.value + 1);
	        hashtable.Add(12,Paydes.text);

			var calNames1 = new string[]{"id","name","contractid","mask","casetype","customer","plaintiff","defendant","other","institution","money","paytype","paydes"};
	        
			var reader = SqliteManager.Instance.UpateValue("case",calNames1,hashtable);
			reader.Close();
			Close();
			return;
		}

		hashtable.Add(0,CaseName.text);
        hashtable.Add(1,ContractId.text);
        hashtable.Add(2,Mask.text);
        hashtable.Add(3,PlayerDataManager.Instance.GetUserId());
        hashtable.Add(4,CaseTypeList[Dropdown.value].Id);
        hashtable.Add(5,Customer.text);
        hashtable.Add(6,Plaintiff.text);
        hashtable.Add(7,Defendant.text);
        hashtable.Add(8,Other.text);
        hashtable.Add(9,Institution.text);
        hashtable.Add(10,Money.text);
        hashtable.Add(11,Paytype.value + 1);
        hashtable.Add(12,Paydes.text);
        hashtable.Add(13,DataTime);
        
		var calNames = new string[]{"name","contractid","mask","master","casetype","customer","plaintiff","defendant","other","institution","money","paytype","paydes","createtime"};

		SqliteManager.Instance.InsertValue("case",calNames,hashtable);

		Close();
	}
}
