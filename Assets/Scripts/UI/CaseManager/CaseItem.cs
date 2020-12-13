using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseItem : MonoBehaviour
{
	[SerializeField] Text CaseName;

	private int CaseId;

    public void SetData(int caseId)
    {
    	CaseId = caseId;
		var dataReader = SqliteManager.Instance.SelectParam("case",string.Format("select * from 'case' where id = {0}",caseId));
		while(dataReader != null && dataReader.Read())
    	{
    		var casename = dataReader.GetString(dataReader.GetOrdinal("name"));
    		CaseName.text = casename;
    	}
        dataReader.Close();
    }

    public void CaseEdit()
    {
    	UIManager.Instance.OpenWindow("CaseEditView",CaseId);
    }

    public void StateEdit()
    {
        UIManager.Instance.OpenWindow("StageListlView",CaseId);
    }
    public void CaseDelete()
    {
    	Hashtable hashtable = new Hashtable();
		hashtable.Add(0,CaseId);
		SqliteManager.Instance.DeleteRecord("case","id",hashtable);

		Utility.SafePostEvent(CaseTotalView.UpdateCaseView);

		ViewUtils.Print("wtf safddasfasfsafasdf ");
    }
}
