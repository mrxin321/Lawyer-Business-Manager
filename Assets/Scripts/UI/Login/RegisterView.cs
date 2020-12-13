using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterView : MonoBehaviour
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

		SqliteManager.Instance.InsertValue("user",calNames,hashtable);
    		
        PlayerPrefs.SetString("UserId",account);
        PlayerPrefs.SetString("PassWord",password);

    	UIManager.Instance.OpenWindow("LoginView");
    }
}
