using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCaseItem : MonoBehaviour
{
	[SerializeField] Text CaseName;

	private CaseData CaseData;

    public void SetData(CaseData caseData)
    {
        CaseData = caseData;
        
        if(caseData != null)
        {        
    		CaseName.text = caseData.Name;
    	}
    }

    public void CheckStateList()
    {
        UIManager.Instance.OpenWindow("MyStageListlView",CaseData.Id);
    }
}
