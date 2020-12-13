using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyStageItem : MonoBehaviour
{
	[SerializeField] Text StageName;

	private StageData StageData;

    public void SetData(StageData stageData)
    {
        StageData = stageData;
        
        if(StageData != null)
        {        
    		StageName.text = stageData.Name;
    	}
    }

    public void CheckTaskList()
    {
        UIManager.Instance.OpenWindow("MyTaskListView",StageData.Id);
    }
}
