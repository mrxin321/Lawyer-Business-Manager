using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IFix.Core;
using System.IO;
using System.Diagnostics;

public class StartGame : MonoBehaviour
{
    void Awake()
    {
        AssetNameLoader.Instance.LoadAssetText();
        SqliteManager.Instance.OpenDBFile(SqlConfig.myDB);

		/** injectFix **/
        // var patch = Resources.Load<TextAsset>("Assembly-CSharp.patch");
        // if (patch != null)
        // {
        //     UnityEngine.Debug.Log("loading Assembly-CSharp.patch ...");
        //     var sw = Stopwatch.StartNew();
        //     PatchManager.Load(new MemoryStream(patch.bytes));
        //     UnityEngine.Debug.Log("patch Assembly-CSharp.patch, using " + sw.ElapsedMilliseconds + " ms");
        // }
        // //try to load patch for Assembly-CSharp-firstpass.dll
        // patch = Resources.Load<TextAsset>("Assembly-CSharp-firstpass.patch");
        // if (patch != null)
        // {
        //     UnityEngine.Debug.Log("loading Assembly-CSharp-firstpass ...");
        //     var sw = Stopwatch.StartNew();
        //     PatchManager.Load(new MemoryStream(patch.bytes));
        //     UnityEngine.Debug.Log("patch Assembly-CSharp-firstpass, using " + sw.ElapsedMilliseconds + " ms");
        // }
        /** injectFix **/

        Utility.DoWait(()=>{
        	Destroy(gameObject);

        	//如果本地没有账号 注册
        	// var userId = PlayerPrefs.GetString("UserId","");

			// UnityEngine.Debug.LogFormat("userId：--------------{0}", userId);

        	// if(userId == "")
        	// {	
        	// 	UIManager.Instance.OpenWindow("RegisterView");
        	// 	return;
        	// }

        	// UIManager.Instance.OpenWindow("LoginView");


            // Close();
            UIManager.Instance.OpenWindow("TestLuaView");
        },4,this);
    }
}
