using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyStageItem : MonoBehaviour
{
    public static Action<int> StageDataUpdate;

    [SerializeField] Text StageName;
    [SerializeField] Text StageProgress;
	[SerializeField] Transform DeleteBtn;

	private StageData StageData;

    private void MyCaseEditEvent()
    {
        DeleteBtn.gameObject.SetActive(!DeleteBtn.gameObject.activeSelf);
        StageProgress.gameObject.SetActive(!DeleteBtn.gameObject.activeSelf);
    }
    private void Awake()
    {
        StageDataUpdate += OnStageDataUpdate;
        MyCaseListView.MyCaseEditEvent +=    MyCaseEditEvent;
    }

    private void OnDestroy()
    {
        StageDataUpdate -= OnStageDataUpdate;
        MyCaseListView.MyCaseEditEvent -=    MyCaseEditEvent;
    }

    private void OnStageDataUpdate(int stageId)
    {
        if(StageData != null && StageData.Id == stageId)
        {
            SetData(StageData);
        }
    }

    public void OnDeleteClick()
    {

    }

    public void SetData(StageData stageData)
    {
        StageData = stageData;
        
        if(StageData != null)
        {        
    		StageName.text = stageData.Name;

            var taskList = TaskReader.GetTaskListByStageId(StageData.Id);
            var count = 0;
            foreach(var item in taskList)
            {
                if(item.State == 2)count ++;
            }
            StageProgress.text = string.Format("完成进度：{0}/{1}",count,taskList.Count);
    	}
    }

    public void CheckTaskList()
    {
        UIManager.Instance.OpenWindow("MyTaskListView",StageData);
    }
}
