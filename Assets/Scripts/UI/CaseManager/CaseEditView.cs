using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseEditView : BaseView	
{

	[SerializeField] InputField CaseName;
	[SerializeField] InputField ContractId;
	[SerializeField] InputField Des;
	[SerializeField] Dropdown DropDown;

	private int CaseId;

	public override void Refresh()
	{
		var _params = GetParams();
		if(_params.Length > 0)
		{
			var caseId = (int)_params[0];
			CaseId = caseId;
			var dataReader = SqliteManager.Instance.SelectParam("case",string.Format("select * from 'case' where id = {0}",caseId));
			while(dataReader != null && dataReader.Read())
			{
				var casename = dataReader.GetString(dataReader.GetOrdinal("name"));
				CaseName.text = casename;

				var contractId = dataReader.GetInt32(dataReader.GetOrdinal("contractid"));
				ContractId.text = contractId.ToString();

				var des = dataReader.GetString(dataReader.GetOrdinal("des"));
				Des.text = des.ToString();
			}
			dataReader.Close();
			return;			
		}
	}

	public void OnSaveClick()
	{
		if(CaseName.text == "")ViewUtils.MessageTips("案子名字不能空哦");

		Hashtable hashtable = new Hashtable();

		if(CaseId > 0)
		{
			hashtable.Add(0,CaseId);
			hashtable.Add(1,CaseName.text);
	        hashtable.Add(2,Convert.ToInt32(ContractId.text));
	        hashtable.Add(3,Des.text);
			var calNames1 = new string[]{"id","name","contractid","des"};
	        
			var reader = SqliteManager.Instance.UpateValue("case",calNames1,hashtable);
			reader.Close();
			Close();
			return;
		}

		hashtable.Add(0,CaseName.text);
        hashtable.Add(1,Convert.ToInt32(ContractId.text));
        hashtable.Add(2,Des.text);
        hashtable.Add(3,PlayerDataManager.Instance.GetUserId());
		var calNames = new string[]{"name","contractid","des","master"};

		SqliteManager.Instance.InsertValue("case",calNames,hashtable);

		Close();
	}
}
