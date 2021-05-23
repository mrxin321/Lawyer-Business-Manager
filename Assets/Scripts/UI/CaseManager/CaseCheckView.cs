using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseCheckView : BaseView
{
    [SerializeField] Text ContractId;
    [SerializeField] Text CaseName;
    [SerializeField] Text CaseType;
    [SerializeField] Text Master;
    [SerializeField] Text Customer;
    [SerializeField] Text Mask;
    [SerializeField] Text Plaintiff;
    [SerializeField] Text Defendant;
    [SerializeField] Text Other;
    [SerializeField] Text Institution;
    [SerializeField] Text Paytype;
    [SerializeField] Text Paydes;
    [SerializeField] Text Createtime;

    private CaseData CaseData;

    public override void Refresh()
	{
		var _params = GetParams();

		if(_params.Length > 0)
		{
			CaseData = (CaseData)_params[0];

			var caseDataReader = SqliteManager.Instance.SelectParam("Case","id",CaseData.Id.ToString());
		 	var caseList = DataBase.GetDataList<CaseData>(caseDataReader,"id","name","mask","content","master","contractid","casetype","customer","plaintiff","defendant","other","institution","money","paytype","paydes","createtime");
		 	if(caseList.Count <= 0)return;

		 	CaseData = caseList[0];

			CaseName.text = CaseData.Name;
			ContractId.text = CaseData.ContractId;
			Mask.text = CaseData.Mask.ToString();

		    var dataReader_ = SqliteManager.Instance.SelectAllParam("casetype");
			var caseTypeList = DataBase.GetDataList<CaseTypeData>(dataReader_,"id","name");
			foreach(var item in caseTypeList)
			{
				if(item.Id == CaseData.CaseType)CaseType.text = item.Name;
			}
		    	
		    var dataReader = SqliteManager.Instance.SelectParam("usercase","caseid",CaseData.Id.ToString());
		 	var masterList = DataBase.GetDataList<UserCaseData>(dataReader,"userid","name");
			var str = "";
			for(int i = 0;i<masterList.Count;i++)
			{
				str += masterList[i].UserName;
				if(i < i-1)str+="、";
			}
			if(str == "")str = "暂无负责人";

			Master.text = str;

		    Customer.text      = CaseData.Customer;
			Plaintiff.text = CaseData.Plaintiff;
			Defendant.text = CaseData.Defendant;
			Other.text = CaseData.Other;
			Institution.text = CaseData.Institution;

			var paytypeReader = SqliteManager.Instance.SelectAllParam("paytype");
			var payTypeList = DataBase.GetDataList<PayTypeData>(paytypeReader,"id","name");
			foreach(var item in payTypeList)
			{
				if(item.Id == CaseData.Paytype)Paytype.text = string.Format("{0}:{1}元",item.Name,CaseData.Money == ""?"0":CaseData.Money);
			}

			Paydes.text = CaseData.Paydes;
			Createtime.text = CaseData.Createtime;
		}
	}

	public void OnDeleteClick()
	{
		Action action = delegate{
			Hashtable hashtable = new Hashtable();
			hashtable.Add(0,CaseData.Id);

	        var deleteSql = "delete from task where stageid in(select id from stage where stage.caseid = {0})";
	        SqliteManager.Instance.DeleteRecord("task",string.Format(deleteSql,CaseData.Id.ToString()));
	        
	        SqliteManager.Instance.DeleteRecord("case","id",hashtable,()=>{
	            Utility.SafePostEvent(MyCaseListView.RefreshMyCaseEvent);
				Close();
	        });
			SqliteManager.Instance.DeleteRecord("stage","caseid",hashtable);

			
		};
    	UIManager.Instance.OpenWindow("MessageTipsConfirmView","是否要删除案件？删除后将无法恢复，关联阶段跟任务也将全部删除",action);
	}
}
