using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomFuncManager: MonoBehaviour
{
    [SerializeField] List<Transform> BottomBg;

    public void SetIndex(int index)
    {
    	SetBg();

    	BottomBg[index].gameObject.SetActive(true);
    }
    public void SetBg()
    {
    	foreach(var item in BottomBg)
    	{
	    	item.gameObject.SetActive(false);
    	}
    }
	public void ToListClick()
	{
		UIManager.Instance.CloseAllWindow();
    	UIManager.Instance.OpenWindow("ToDoListView");
	}    

	public void MyCaseClick()
	{
		UIManager.Instance.CloseAllWindow();
    	UIManager.Instance.OpenWindow("MyCaseListView");
	}    
	public void CaseManagerClick()
	{
		UIManager.Instance.CloseAllWindow();
    	UIManager.Instance.OpenWindow("CaseTotalView");
	}    
	public void UserClick()
	{
		UIManager.Instance.CloseAllWindow();
    	UIManager.Instance.OpenWindow("UserView");
	}    

}
