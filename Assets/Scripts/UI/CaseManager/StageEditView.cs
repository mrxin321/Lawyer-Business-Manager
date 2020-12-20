using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEditView : BaseView
{	
	[SerializeField] InputField StageName;
	[SerializeField] InputField Des;
    private int CaseId;
	private int StageId;

	public override void Refresh()
	{
		var _params = GetParams();
		if(_params.Length >= 1)CaseId = (int)_params[0];
		if(_params.Length >= 2)StageId = (int)_params[1];

		if(StageId > 0)
		{
			var dataReader = SqliteManager.Instance.SelectParam("stage",string.Format("select * from 'stage' where id = {0}",StageId));
			while(dataReader != null && dataReader.Read())
			{
				var stagename = dataReader.GetString(dataReader.GetOrdinal("name"));
				StageName.text = stagename;

				var des = dataReader.GetString(dataReader.GetOrdinal("des"));
				Des.text = des.ToString();
			}
			dataReader.Close();
		}
	}   
	
	public void StageEdit()
	{
		if(StageName.text == "")ViewUtils.MessageTips("阶段名字不能空哦");

		Hashtable hashtable = new Hashtable();

		if(StageId > 0)
		{
			hashtable.Add(0,StageId);
			hashtable.Add(1,StageName.text);
	        hashtable.Add(2,CaseId);
	        hashtable.Add(3,Des.text);
			var calNames1 = new string[]{"id","name","caseid","des"};
	        
			SqliteManager.Instance.UpateValue("stage",calNames1,hashtable);
			Utility.SafePostEvent(StageListlView.UpdateView);
			Close();
			return;
		}

		hashtable.Add(0,StageName.text);
        hashtable.Add(1,CaseId);
        hashtable.Add(2,Des.text);
		var calNames = new string[]{"name","caseid","des"};

		SqliteManager.Instance.InsertValue("stage",calNames,hashtable);
		Utility.SafePostEvent(StageListlView.UpdateView);
		Close();

	} 

}
