using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MyCaseListView : BaseView
{
    [SerializeField] Transform ItemRoot;
    [SerializeField] Transform NewCaseBtn;
    [SerializeField] Transform BackBtn;
    [SerializeField] Text TileText;
	[SerializeField] BottomFuncManager BottomFuncManager;

    public static Action MyCaseEditEvent;
    public static Action RefreshMyCaseEvent;
    private MyCaseShowType MyCaseShowType;

    private void Awake()
    {
        RefreshMyCaseEvent += OnRefreshMyCaseEvent;
    }

    private void OnDestroy()
    {
        RefreshMyCaseEvent -= OnRefreshMyCaseEvent;
    }

    public override void Refresh()
	{
		BottomFuncManager.SetIndex(1);
		Utility.DestroyAllChildren(ItemRoot);

		var _param = GetParams();
		var caseList = new List<CaseData>();

		if(_param.Length > 0)MyCaseShowType = MyCaseShowType.TotalCase;

		NewCaseBtn.gameObject.SetActive(MyCaseShowType != MyCaseShowType.TotalCase);
		BackBtn.gameObject.SetActive(MyCaseShowType == MyCaseShowType.TotalCase);

		if(MyCaseShowType == MyCaseShowType.TotalCase)
		{
			caseList = CaseDataReader.GetAllDataList();
			TileText.text = "全部案件";
		}
		else caseList = CaseDataReader.GetMyDataList();
		
		foreach(var taskData in caseList)
    	{
			var copyItem = AssetManager.CreatePrefab("MyCaseItem",ItemRoot);

			var item = copyItem.GetComponent<MyCaseItem>();
			if(item != null)
			{
				item.SetData(taskData);
			}

    	}
	}

	private void OnRefreshMyCaseEvent()
	{
		Refresh();
	}

	public void OnEditorClick()
	{
		Utility.SafePostEvent(MyCaseEditEvent);
	}
}
