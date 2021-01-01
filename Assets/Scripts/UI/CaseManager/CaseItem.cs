using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseItem : MonoBehaviour
{
	[SerializeField] Text CaseName;

	private CaseData CaseData;

    public void SetData(CaseData caseData)
    {
    	CaseData = caseData;
    	CaseName.text = caseData.Name;
    }

    public void CaseEdit()
    {
    	UIManager.Instance.OpenWindow("CaseEditView",CaseData);
    }

    public void StateEdit()
    {
        UIManager.Instance.OpenWindow("StageListlView",CaseData);
    }
    public void CaseDelete()
    {
    	Hashtable hashtable = new Hashtable();
		hashtable.Add(0,CaseData.Id);

        var deleteSql = "delete from task where stageid in(select id from stage where stage.caseid = {0})";
        SqliteManager.Instance.DeleteRecord("task",string.Format(deleteSql,CaseData.Id.ToString()));
        
        SqliteManager.Instance.DeleteRecord("case","id",hashtable);
		SqliteManager.Instance.DeleteRecord("stage","caseid",hashtable);
        

		Utility.SafePostEvent(CaseTotalView.UpdateCaseView);
    }
}
