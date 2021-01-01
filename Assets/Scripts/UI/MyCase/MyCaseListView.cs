using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCaseListView : BaseView
{
    [SerializeField] Transform ItemRoot;

    public override void Refresh()
	{
		Utility.DestroyAllChildren(ItemRoot);

		var myUserId = PlayerDataManager.Instance.GetUserId();
		
		var dataReader = SqliteManager.Instance.SelectParam("case","master",myUserId.ToString());

		var dataList = DataBase.GetDataList<CaseData>(dataReader,"id","name","mask","content","master","contractid");
		foreach(var taskData in dataList)
    	{
			var copyItem = AssetManager.CreatePrefab("MyCaseItem",ItemRoot);

			var item = copyItem.GetComponent<MyCaseItem>();
			if(item != null)
			{
				item.SetData(taskData);
			}

    	}
	}
}
