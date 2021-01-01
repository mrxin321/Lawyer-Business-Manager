using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserView : BaseView
{
	[SerializeField] Text WelcomeText;

    public override void Refresh()
	{
		WelcomeText.text = string.Format("欢迎你,{0}",PlayerDataManager.Instance.GetUserName());
	}
	
	public void OpenMyCaseView()
	{
    	UIManager.Instance.OpenWindow("MyCaseListView");
	}

	public void OpenTempView()
	{
    	UIManager.Instance.OpenWindow("TempStageListView");
	}

	public void OutLoginClick()
	{
		Action action = delegate{
			UIManager.Instance.CloseAllWindow();
    		UIManager.Instance.OpenWindow("LoginView");
		};
    	UIManager.Instance.OpenWindow("MessageTipsConfirmView","是否要退出登录?",action);
	}
}
