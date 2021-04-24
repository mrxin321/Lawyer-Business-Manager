using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using IFix;

public class LoginView : BaseView
{
	[SerializeField] public InputField Account;
	[SerializeField] public InputField PassWorld;
    [SerializeField] public Button button1;

    [CSharpCallLua]
    public delegate double LuaMax(double a, double b);

    public LuaEnv luaenv = new LuaEnv();

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
    	var password = PassWorld.text;

    	var dataReader = SqliteManager.Instance.SelectParam("user",string.Format("select * from user where account = '{0}' and password = '{1}'",account,password));
    	UnityEngine.Debug.LogFormat("dataReader数据：", dataReader);
        while(dataReader != null && dataReader.Read())
    	{
    		var userId = dataReader.GetInt32(dataReader.GetOrdinal("id"));
    		var permission = dataReader.GetInt32(dataReader.GetOrdinal("permission"));
    		var username = dataReader.GetString(dataReader.GetOrdinal("name"));

            PlayerPrefs.SetString("UserId",account);
            PlayerPrefs.SetString("PassWord",password);

    		PlayerDataManager.Instance.SetUserData(userId,username,permission);
	    	
	    	dataReader.Close();
            SqliteManager.Instance.CloseDB();
	    	Close();
	    	
            UIManager.Instance.OpenWindow("ToDoListView");
            return;
    	}
    	
    	UIManager.Instance.OpenWindow("MessageTipsView","账号或者密码错误");
    }

    public void OnRegisterClick()
    {
        Close();
        UIManager.Instance.OpenWindow("RegisterView");
    }
}
