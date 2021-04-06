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
        AssetBundle ab = AssetBundle.LoadFromFile("AssetBundles/abc_btn");
        Debug.LogFormat("2222=-=-=-=-{0}321", ab);
        Sprite btnSprite = ab.LoadAsset<Sprite>("bubble.9");
        Debug.LogFormat("111=-=-=-=-{0}999", btnSprite);
        // Instantiate(btnSprite);

        // Debug.LogFormat("113333331=-=-=-=-{0}9966669", button1);
        // Debug.LogFormat("113333331=-=-=-=-{0}9966669", button1.image);
        // Debug.LogFormat("113333331=-=-=-=-{0}9966669", button1.image.sprite);
        button1.image.sprite = btnSprite;

        luaenv.DoString("CS.UnityEngine.Debug.Log('hello world xlua xlua')");

        var max =  luaenv.Global.GetInPath<LuaMax>("math.min");
        UnityEngine.Debug.Log("max: " + max(32, 12));
        max = null;
        UnityEngine.Debug.Log(max);

        luaenv.DoString("CS.UnityEngine.Debug.Log('hello world xlua xlua 222222222222')");

        // luaenv.Dispose();

        var userId = PlayerPrefs.GetString("UserId","");
        var passWorld = PlayerPrefs.GetString("PassWord","");

        Account.text = userId;
        PassWorld.text = passWorld;

        add();
        luaUse();
    }

    public void luaUse(){
        Debug.Log("开始调用lua脚本---------------");
        luaenv.AddLoader(LuaLoader);
    }

    private byte[] LuaLoader(ref string filename)
    {
        TextAsset text = Resources.Load("Lua/" + "LoginViewLua" + ".lua") as TextAsset;
        return text.bytes;
    }

    public void Update(){
        if(luaenv != null){
            luaenv.Tick();
        }
    }

    public void Destroy(){
        if(luaenv != null){
            luaenv.Dispose();
        }
    }

    public void add()
    {
        var a = 5;
        var b = 1;
        UnityEngine.Debug.Log("111111" + (a + b));
    }

    // [Patch]
    // public void add()
    // {
    //     UnityEngine.Debug.Log("开始进行injectFix热更新-----------------");
    //     var a = 12;
    //     var b = 1;
    //     UnityEngine.Debug.Log("111111" + (a + b));
    // }

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
