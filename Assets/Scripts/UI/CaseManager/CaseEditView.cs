using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseEditView : BaseView	
{
	public static Action<List<UserData>> MasterChooseAction;

	[SerializeField] InputField CaseName;
	[SerializeField] InputField ContractId;
	[SerializeField] InputField Mask;
	[SerializeField] Dropdown Dropdown;
    [SerializeField] Text BtnName;
    [SerializeField] Text DataTimeText;
    [SerializeField] Text MasterText;
	[SerializeField] InputField Customer;
	[SerializeField] InputField Plaintiff;
	[SerializeField] InputField Defendant;
	[SerializeField] InputField Other;
	[SerializeField] InputField Institution;
	[SerializeField] InputField Money;
	[SerializeField] Dropdown Paytype;
	[SerializeField] InputField Paydes;
	[SerializeField] Transform TempStageRoot;

	private CaseData CaseData;
	private List<CaseTypeData> CaseTypeList;
	private string DataTime;
	private List<UserData> MasterList = new List<UserData>();
	private List<int> TempStageList = new List<int>();

	private void OnEnable()
	{
		MasterChooseAction += OnMasterChooseAction;
		TempStageSelectItem.CaseTempStageAddSuccess += CaseTempStageAddSuccess;
		CaseStageAddItem.RemoveStageId += OnRemoveStageId;
	}

	private void OnDisable()
	{
		MasterChooseAction -= OnMasterChooseAction;
		TempStageSelectItem.CaseTempStageAddSuccess -= CaseTempStageAddSuccess;
		CaseStageAddItem.RemoveStageId -= OnRemoveStageId;
	}

	public string GetCaseNum(int caseIndex)
	{
		var TopNum = 10000;
		TopNum += caseIndex;

		var str = TopNum.ToString();

		return str.Substring(1,str.Length-1);
	}	

	public string GetDayString()
	{
		return DateTime.Now.Year.ToString();
	}

	public string GetCaseTypeString()
	{
		return CaseTypeList[Dropdown.value].Name[0] + "";
	}

	public string GetCaseTypeIndex()
	{
		var caseType = CaseTypeList[Dropdown.value].Id;
		var index = PlayerPrefs.GetInt("CaseIndex"+caseType.ToString(),1);

		return GetCaseNum(index);
	}

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
			DataTimeText.text = CaseData.Createtime;
		    BtnName.text = "修改案件";

			var dataReader = SqliteManager.Instance.SelectParam("usercase","caseid",CaseData.Id.ToString());
		 	var masterList = DataBase.GetDataList<UserCaseData>(dataReader,"userid","name");
		 	MasterList.Clear();
		 	foreach(var item in masterList)
		 	{
		 		var data = new UserData();
		 		data.Id = item.UserId;
		 		data.NickName = item.UserName;
		 		MasterList.Add(data);
		 	}
		 	OnMasterChooseAction(MasterList);
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

	    SetCaseNum(_params.Length <= 0);

	    var paytypeReader = SqliteManager.Instance.SelectAllParam("paytype");

		var payTypeList = DataBase.GetDataList<PayTypeData>(paytypeReader,"id","name");

	    ViewUtils.SetPayTypeDropdown(Paytype,payTypeList,payType - 1);

	}

	public void SetCaseNum(bool setName)
	{
		if(!setName)return;
		//新创建案件 需要自动填充名字
		var dayStr = GetDayString();
		var castTypeStr = GetCaseTypeString();
		var indexStr = GetCaseTypeIndex();
		ContractId.text = string.Format("({0})粤法丞汇俊莞{1}字第{2}号",dayStr,castTypeStr,indexStr);
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
			//删除案件管理者
			Hashtable dhashtable = new Hashtable();
			dhashtable.Add(0,CaseData.Id);

			SqliteManager.Instance.DeleteRecord("usercase","caseid",dhashtable);

			AddCaseMaster(CaseData.Id);

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
	        
			SqliteManager.Instance.UpateValue("case",calNames1,hashtable);
			
			Close();
			return;
		}

		hashtable.Add(0,CaseName.text);
        hashtable.Add(1,ContractId.text);
        hashtable.Add(2,Mask.text);
        hashtable.Add(3,0);
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

		var caseId = SqliteManager.Instance.InsertValue("case",calNames,hashtable);

		//添加负责人
		AddCaseMaster(caseId);

		var index = PlayerPrefs.GetInt("CaseIndex"+CaseTypeList[Dropdown.value].Id.ToString(),1);
		index++;
		PlayerPrefs.SetInt("CaseIndex"+CaseTypeList[Dropdown.value].Id.ToString(),index);

		Close();	
	}

	private void AddCaseMaster(int id)
	{
		//添加负责人
		foreach(var item in MasterList)
		{
			var hashtable1 = new Hashtable(); 
			hashtable1.Add(0,id);
			hashtable1.Add(1,item.Id);
			hashtable1.Add(2,item.NickName);
			var calNames1 = new string[]{"caseid","userid","name"};
			SqliteManager.Instance.InsertValue("usercase",calNames1,hashtable1);
		}
	}

	public void MasterChoose()
	{
		UIManager.Instance.OpenWindow("MasterChooseView",MasterList);
	}

	public void StageChoose()
	{
		UIManager.Instance.OpenWindow("TempStageChoose",MasterList);
	}

	public void OnMasterChooseAction(List<UserData> masterList)
	{
		MasterList = masterList;
		var str = "";
		var length = masterList.Count;
		var i = 0;
		foreach(var item in masterList)
		{
			str += item.NickName;
			if(i < length-1)str+="、";
			i++;
		}
		MasterText.text = str;

		if(MasterText.text == "")
		{
			MasterText.text = "请选择负责人";
		}
	}

	public void OpenStageListView()
	{
		UIManager.Instance.OpenWindow("TempStageSelectListView",CaseTypeList[Dropdown.value].Id,TempStageList);
	}

	private void OnRemoveStageId(int stageId)
	{
		foreach(var item in TempStageList)
		{
			if(item == stageId)
			{
				TempStageList.Remove(stageId);
				return;
			}
		}
	}

	public void CaseTempStageAddSuccess(int stageId,bool _select,string stageName)
	{
		if(_select)
		{
			TempStageList.Add(stageId);
	 		var copyItem = AssetManager.CreatePrefab("CaseStageAddItem",TempStageRoot);

	        var item = copyItem.GetComponent<CaseStageAddItem>();
	        if(item != null)
	        {
	        	item.transform.SetAsFirstSibling();
	            item.SetData(stageId,stageName);
	        }
	    }else
	    {
	    	OnRemoveStageId(stageId);
	    }
	}
}
