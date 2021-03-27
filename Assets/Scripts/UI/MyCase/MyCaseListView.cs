using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCaseListView : BaseView
{
    [SerializeField] Transform ItemRoot;

    public override void Refresh()
	{
		Utility.DestroyAllChildren(ItemRoot);

		var dataList = CaseDataReader.GetMyDataList();
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
