using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCaseItem : MonoBehaviour
{
	[SerializeField] Text CaseName;
    [SerializeField] Transform StageRoot;
    [SerializeField] Transform OpenTag;
    [SerializeField] Transform DeleteBtn;

	private CaseData CaseData;

    void OnEnable()
    {
        MyCaseListView.MyCaseEditEvent +=    MyCaseEditEvent;
    }

    void OnDisable()
    {
        MyCaseListView.MyCaseEditEvent -=    MyCaseEditEvent;
    }

    private void MyCaseEditEvent()
    {
        DeleteBtn.gameObject.SetActive(!DeleteBtn.gameObject.activeSelf);
    }

    public void DeleteBtnClick()
    {}

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
            Debug.LogFormat("wtf datao tat ata tat {0}",JsonUtility.ToJson(data));
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

    public void CaseCheckClick()
    {
        UIManager.Instance.OpenWindow("CaseCheckView",CaseData);
    }
}
