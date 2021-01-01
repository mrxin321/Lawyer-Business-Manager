using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCaseItem : MonoBehaviour
{
	[SerializeField] Text CaseName;
    [SerializeField] Transform StageRoot;
    [SerializeField] Transform OpenTag;

	private CaseData CaseData;

    public void SetData(CaseData caseData)
    {
        CaseData = caseData;
        
        if(caseData != null)
        {        
    		CaseName.text = caseData.Name;
    	}

        var dataReader = SqliteManager.Instance.SelectParam("stage","caseid",caseData.Id.ToString());

        var dataList = DataBase.GetDataList<StageData>(dataReader,"id","name","des","caseid","casetype");
        foreach(var data in dataList)
        {
            var copyItem = AssetManager.CreatePrefab("MyStageItem",StageRoot);
            var myStageItem = copyItem.GetComponent<MyStageItem>();
            if(myStageItem != null)
            {
                data.CaseName = caseData.Name;
                myStageItem.SetData(data);
            }
        }
    }

    public void OpenOrCloseTodoList()
    {
        StageRoot.gameObject.SetActive(!StageRoot.gameObject.activeSelf);
        OpenTag.localRotation = Quaternion.Euler(0.0f,0.0f,StageRoot.gameObject.activeSelf?-90f:90f);
    }

    public void CheckStateList()
    {
        UIManager.Instance.OpenWindow("MyStageListlView",CaseData.Id);
    }
}
