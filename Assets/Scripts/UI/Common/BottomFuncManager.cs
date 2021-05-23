using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomFuncManager: MonoBehaviour
{
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
