using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

		if(Account.text == "")ViewUtils.MessageTips("账号不能为空哦");
		if(Name.text == "")ViewUtils.MessageTips("昵称不能为空哦");
		if(PassWord.text == "")ViewUtils.MessageTips("密码不能为空哦");

		var hashtable = new Hashtable();
    	hashtable.Add(0,account);
		hashtable.Add(1,name);
		hashtable.Add(2,password);
		var calNames = new string[]{"account","name","password"};

        var dataReader = SqliteManager.Instance.SelectParam("user","account",string.Format("'{0}'",Account.text));
        var list = DataBase.GetDataList<UserData>(dataReader,"id","account");

        Debug.LogFormat("list:----------------{0}", list);
        for(int i = 0; i < list.Count; i++){
            Debug.LogFormat("i==================: {0}", i);
            Debug.LogFormat("list.Name: {0}", list[i].Name);
            Debug.LogFormat("list.Id: {0}", list[i].Id);
            Debug.LogFormat("list.NickName: {0}", list[i].NickName);
            Debug.LogFormat("list.Password: {0}", list[i].Password);
        }
        
        if(list.Count > 0)
        {
            ViewUtils.MessageTips("账号重复了!!!");
            return;
        }

		SqliteManager.Instance.InsertValue("user",calNames,hashtable);
    		
        PlayerPrefs.SetString("UserId",account);
        PlayerPrefs.SetString("PassWord",password);
        Close();
    	UIManager.Instance.OpenWindow("LoginView");
    }
}
