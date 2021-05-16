using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class RegisterView : BaseView
{
    [SerializeField] public InputField Account;
    [SerializeField] public InputField Name;
	[SerializeField] public InputField PassWord;

    public void OnLoginClick()
    {
    	var account = Account.text;
    	var name = Name.text;
    	var password = PassWord.text;

        var tmpAccount = Regex.Replace(account, @"\s", "");
        var tmpName = Regex.Replace(name, @"\s", "");
        var tmpPassword = Regex.Replace(password, @"\s", "");

        Debug.LogFormat("Wtf tmpAccounttmpAccounttmpAccount {0} {1} {2}",tmpAccount,tmpName,tmpPassword);

        if(account != tmpAccount || name != tmpName || password != tmpPassword)
        {
            ViewUtils.MessageTips("不能带有空格哦");
            return;
        }

		if(Account.text == "" || Name.text == "" ||PassWord.text == "")
        {
            ViewUtils.MessageTips("账号不能为空哦");
            return;
        }
		
		var hashtable = new Hashtable();
    	hashtable.Add(0,account);
		hashtable.Add(1,name);
		hashtable.Add(2,password);
		var calNames = new string[]{"account","name","password"};

        var sql = "SELECT * FROM '{0}' where {1} = '{2}' or {3} = '{4}'";
        var dataReader = SqliteManager.Instance.SelectParam("user",string.Format(sql,"user","account",Account.text,"name",Name.text));
        var list = DataBase.GetDataList<UserData>(dataReader,"id","account");
        if(list.Count > 0)
        {
            ViewUtils.MessageTips("账号或者名字重复了!!!");
            return;
        }

        UIManager.Instance.OpenWindow("MessageMaskView",true);

		SqliteManager.Instance.InsertValue("user",calNames,hashtable,(insertId)=>{
            UIManager.Instance.CloseWindow("MessageMaskView");

            PlayerPrefs.SetString("UserId",account);
            PlayerPrefs.SetString("PassWord",password);
            Close();
            UIManager.Instance.OpenWindow("LoginView");
        });
    	
    }

    public void Back()
    {
        Close();
        UIManager.Instance.OpenWindow("LoginView");
    }
}
