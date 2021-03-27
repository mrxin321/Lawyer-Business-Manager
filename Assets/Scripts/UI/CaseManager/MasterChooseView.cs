using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterChooseView : BaseView
{
    [SerializeField] Transform Root;

    private Dictionary<int,bool> CurChoose = new Dictionary<int,bool>();
    private List<UserData> UserList;
	public override void Refresh()
	{
		var _params = GetParams();

		if(_params.Length >= 1)
		{
			var curChoose = (List<UserData>)_params[0];
			foreach(var item in curChoose)
			{
				CurChoose.Add(item.Id,true);
			}
		}

		var dataReader = SqliteManager.Instance.SelectAllParam("user");
		var dataList = DataBase.GetDataList<UserData>(dataReader,"id","name");
		UserList = dataList;
		foreach(var data in dataList)
    	{
			var castItem = AssetManager.CreatePrefab("MasterChooseItem",Root);

			var item = castItem.GetComponent<MasterChooseItem>();
			if(item != null)
			{
				item.SetData(data.Id,data.NickName,CurChoose.ContainsKey(data.Id),Choose);
			}
    	}
	}   

	public void Choose(int chooseId,bool choose)
	{
		if(choose)
		{
			if(!CurChoose.ContainsKey(chooseId))CurChoose.Add(chooseId,true);
		}else
		{
			if(CurChoose.ContainsKey(chooseId))CurChoose.Remove(chooseId);
		}
	}
	
	public void Close()
	{
		base.Close();
		var dic = new List<UserData>();
		foreach(var item in UserList)
		{
			if(CurChoose.ContainsKey(item.Id))
			{
				dic.Add(item);
			}
		}
		Utility.SafePostEvent(CaseEditView.MasterChooseAction,dic);
	}
}
