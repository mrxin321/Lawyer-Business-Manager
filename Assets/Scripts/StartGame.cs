using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    void Awake()
    {
        AssetNameLoader.Instance.LoadAssetText();
        SqliteManager.Instance.OpenDBFile(SqlConfig.myDB);

        Utility.DoWait(()=>{
        	Destroy(gameObject);

        	//如果本地没有账号 注册
        	var userId = PlayerPrefs.GetString("UserId","");
        	if(userId == "")
        	{	
        		UIManager.Instance.OpenWindow("RegisterView");
        		return;
        	}

        	UIManager.Instance.OpenWindow("LoginView");
        },4,this);
    }
}
