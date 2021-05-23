using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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

	public void OutLoadingClick()
	{
		Action action = delegate{
			UIManager.Instance.CloseAllWindow();
    		// 卸载当前场景
	        Utility.DoWait(()=>{
	            SceneManager.UnloadScene("StartScene");
	        },1f,this);
	        // 加载下一个场景
	        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
		};
    	UIManager.Instance.OpenWindow("MessageTipsConfirmView","是否要重新刷新数据?",action);
	}
	public void OnTotalCaseClick()
	{
		UIManager.Instance.OpenWindow("MyCaseListView",MyCaseShowType.TotalCase);
	}
}
