using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempStageSelectItem : MonoBehaviour
{
	public static Action<int,bool,string,string> CaseTempStageAddSuccess;

    [SerializeField] Text StageName;
    [SerializeField] SelectItem SelectItem;

    private StageData StageData;

    public void SetData(StageData stageData,bool IsSelect)
    {
    	StageData = stageData;
    	StageName.text = stageData.Name;
        SelectItem.SetData(OnSelectItem,IsSelect);
    }

    public void OnSelectItem(bool _select)
    {
        Utility.SafePostEvent(CaseTempStageAddSuccess,StageData.Id,_select,StageData.Name,StageData.Des);
    }
}
