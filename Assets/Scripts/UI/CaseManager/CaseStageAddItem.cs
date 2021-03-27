using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseStageAddItem : MonoBehaviour
{
    public static Action<int> RemoveStageId;

	[SerializeField] Text StageName;

    private void OnEnable()
    {
        TempStageSelectItem.CaseTempStageAddSuccess += CaseTempStageAddSuccess;
    }

    private void OnDisable()
    {
        TempStageSelectItem.CaseTempStageAddSuccess -= CaseTempStageAddSuccess;
    }

    private void CaseTempStageAddSuccess(int id,bool _select,string name)
    {
        if(!_select && StageId == id)
        {
            GameObject.Destroy(gameObject);
        }
    }
    
    private int StageId;
    public void SetData(int stageId,string stageName)
    {
        StageId = stageId;
    	StageName.text = stageName;
    }

    public void RemoveStage()
    {
        GameObject.Destroy(gameObject);
        Utility.SafePostEvent(RemoveStageId,StageId);
    }

}
