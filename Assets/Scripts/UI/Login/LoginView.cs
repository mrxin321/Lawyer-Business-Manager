using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : BaseView
{
	[SerializeField] public InputField Account;
	[SerializeField] public InputField PassWorld;

    public override void Refresh()
    {
        var userId = PlayerPrefs.GetString("UserId","");
        var passWorld = PlayerPrefs.GetString("PassWord","");

        Account.text = userId;
        PassWorld.text = passWorld;
    }

    public void OnLoginClick()
    {
    	var account = Account.text;
    	var passworld = PassWorld.text;

    	var dataReader = SqliteManager.Instance.SelectParam("user",string.Format("select * from user where account = '{0}' and password = '{1}'",account,passworld));
    	while(dataReader != null && dataReader.Read())
    	{
    		var userId = dataReader.GetInt32(dataReader.GetOrdinal("id"));
    		var permission = dataReader.GetInt32(dataReader.GetOrdinal("permission"));
    		var username = dataReader.GetString(dataReader.GetOrdinal("name"));

    		PlayerDataManager.Instance.SetUserData(userId,username,permission);
	    	
	    	dataReader.Close();
	    	Close();
	    	
            UIManager.Instance.OpenWindow("ToDoListView");
            return;
    	}
    	
    	UIManager.Instance.OpenWindow("MessageTipsView","账号或者密码错误");
    }
}
