using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
	[SerializeField] Transform AddStageBtn;

	private CaseData CaseData;
	private List<CaseTypeData> CaseTypeList;
	private string DataTime;
	private List<UserData> MasterList = new List<UserData>();
	private Dictionary<int,StageData> StageList = new Dictionary<int,StageData>();

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
		var TopNum = 1000;
		if(caseIndex < TopNum)
			caseIndex += TopNum;

		var str = caseIndex.ToString();

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

		var dataReader = SqliteManager.Instance.SelectParam("casetypenum","id",DateTime.Now.Year.ToString());
		var dataList = DataBase.GetDataList<CaseTypeNumData>(dataReader,"num"+caseType.ToString());

		//没有当前年份 添加
		if(dataList.Count <= 0)
		{
			Hashtable dhashtable = new Hashtable();
			dhashtable.Add(0,DateTime.Now.Year);
			dhashtable.Add(1,0);
			dhashtable.Add(2,0);
			dhashtable.Add(3,0);
			dhashtable.Add(4,0);
			dhashtable.Add(5,0);
			dhashtable.Add(6,0);
			var calNames = new string[]{"id","num1","num2","num3","num4","num5","num6"};
	        
			SqliteManager.Instance.InsertValue("casetypenum",calNames,dhashtable);
		}

		var index = 1;
		if(dataList.Count > 0)
		{
			index = dataList[0].GetNum(caseType);
			index++;
		}

		return GetCaseNum(index);
	}

	public override void Refresh()
	{
		DataTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

		DataTimeText.text = DataTime;

		var _params = GetParams();
		var caseType = 1;
		var payType = 1;

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

	    	RefreshStageList(CaseData);
		}else
		{
			//新建案件
			NewCaseView();
			SetCaseNum();
		}

	}

	public void NewCaseView()
	{
		var dic = new List<UserData>();
		var data = PlayerDataManager.Instance.GetUserData();
		dic.Add(data);
		Utility.SafePostEvent(CaseEditView.MasterChooseAction,dic);
	}

	public void RefreshStageList(CaseData caseData)
	{
		var dataReader = SqliteManager.Instance.SelectParam("stage","caseid",caseData.Id.ToString());
		var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des");

		var sqlStr = "SELECT * FROM 'stage' where caseid = 0 and casetype = {0}";
		var dataReader2 = SqliteManager.Instance.SelectParam("stage",string.Format(sqlStr,CaseData.CaseType));
		var templateStageList = DataBase.GetDataList<StageData>(dataReader2,"id","name","des","casetype");

		foreach(var stageData in dataList)
    	{
    		foreach(var templateItem in templateStageList)
    		{
    			if(templateItem.Name == stageData.Name)
    			{
    				CaseTempStageAddSuccess(templateItem.Id,true,templateItem.Name,templateItem.Des);
    			}
    		}
    	}
	}	

	public void CaseTypeChangeEvent()
	{
		SetCaseNum();
	}

	public void SetCaseNum()
	{
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
			ViewUtils.MessageTips("案件编号不能空哦");
			return;
		}

		if(CaseName.text == "")
		{
			ViewUtils.MessageTips("案件名字不能空哦");
			return;
		}

		Hashtable hashtable = new Hashtable();


		if(CaseData != null)
		{
			//删除案件管理者
			Hashtable dhashtable = new Hashtable();
			dhashtable.Add(0,CaseData.Id);

			SqliteManager.Instance.DeleteRecord("usercase","caseid",dhashtable,()=>{
				AddCaseMaster(CaseData.Id);
			});

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
				
			DeleteCaseStage(CaseData.Id,()=>{
				AddCaseTemplateStage(StageList,CaseData.Id);
				Close();
			});

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

		SqliteManager.Instance.InsertValue("case",calNames,hashtable,(caseId)=>{
			AddCaseTemplateStage(StageList,caseId);
			//添加负责人
			AddCaseMaster(caseId,()=>{
				Close();	
			});
			//保存index
			var index = GetCaseNameNum(ContractId.text);

			Hashtable dhashtable2 = new Hashtable();
			dhashtable2.Add(0,DateTime.Now.Year.ToString());
			dhashtable2.Add(1,index);
			var calNames2 = new string[]{"id","num"+CaseTypeList[Dropdown.value].Id.ToString()};
	        
			SqliteManager.Instance.UpateValue("casetypenum",calNames2,dhashtable2);
		});
	}

	private int GetCaseNameNum(string name)
	{
		for(int i = 0;i<name.Length;i++)
		{
			if(name[i] == '第')
			{
				for(int j = 1;j<=6;j++)
				{
					if(name[i+j] == '号')
					{
						var indexStr = name.Substring(i+1,j-1);
						var index = 0;
        				int.TryParse(indexStr, out index);

						return index;
					}
				}	
				break;
			}
		}
		return -1;
	}

	private void AddCaseTemplateStage(Dictionary<int,StageData> stageList,int caseId)
	{
		foreach(var item in stageList.Values)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add(0,item.Name);
	        hashtable.Add(1,caseId);
	        hashtable.Add(2,item.Des);

			var calNames = new string[]{"name","caseid","des"};

			SqliteManager.Instance.InsertValue("stage",calNames,hashtable,(insertId)=>{
				var dataReader = SqliteManager.Instance.SelectParam("task","stageid",item.Id.ToString());

				var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","content","des");
				foreach(var taskData in dataList)
		    	{
					//添加任务
					var thashtable = new Hashtable();
					thashtable.Add(0,insertId);
					thashtable.Add(1,taskData.Content);
					thashtable.Add(2,taskData.Des);
					var tcalNames = new string[]{"stageid","content","des"};
					SqliteManager.Instance.InsertValue("task",tcalNames,thashtable);
		    	}
			});

		}
	}

	private void DeleteCaseStage(int caseid,Action callback)
	{
		Hashtable dhashtable = new Hashtable();
		dhashtable.Add(0,caseid);

		SqliteManager.Instance.DeleteRecord("stage","caseid",dhashtable,()=>{
			SqliteManager.Instance.DeleteRecord("task","stageid",dhashtable,()=>{
				Utility.SafePostEvent(callback);
			});
		});
	}

	private void AddCaseMaster(int id,Action callback = null)
	{
		Debug.LogFormat("wtf AddCaseMaster {0}",MasterList.Count);
		var count = 0;
		//添加负责人
		foreach(var item in MasterList)
		{
			var hashtable1 = new Hashtable(); 
			hashtable1.Add(0,id);
			hashtable1.Add(1,item.Id);
			hashtable1.Add(2,item.NickName);
			var calNames1 = new string[]{"caseid","userid","name"};
			SqliteManager.Instance.InsertValue("usercase",calNames1,hashtable1,(iid)=>{
				count ++;
				if(count == MasterList.Count)Utility.SafePostEvent(callback);
			});
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
		var list = StageList.Keys.ToList();
		UIManager.Instance.OpenWindow("TempStageSelectListView",CaseTypeList[Dropdown.value].Id,list);
	}

	private void OnRemoveStageId(int stageId)
	{
		if(StageList.ContainsKey(stageId))
		{
			StageList.Remove(stageId);
		}
	}

	public void CaseTempStageAddSuccess(int stageId,bool _select,string stageName,string des)
	{
		if(_select)
		{
			var data = new StageData();
			data.Id = stageId;
			data.Name = stageName;
			data.Des = des;

			StageList.Add(stageId,data);

	 		var copyItem = AssetManager.CreatePrefab("CaseStageAddItem",TempStageRoot);

	        var item = copyItem.GetComponent<CaseStageAddItem>();
	        if(item != null)
	        {
	        	item.transform.SetAsLastSibling();
	            item.SetData(stageId,stageName);
	        }
	        AddStageBtn.SetAsLastSibling();
	    }else
	    {
	    	OnRemoveStageId(stageId);
	    }
	}
}
