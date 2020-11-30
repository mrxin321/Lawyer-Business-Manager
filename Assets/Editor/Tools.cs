using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Collections;
using System.IO;


namespace Assets.CustomAssets.Scripts.Foundation.Editor
{
    public class Tools
    {
        [MenuItem("Tools/创建界面")]
        public static void CreateUI()
        {
        	var assetName = "TestPrefab";
        	var assetPatch = "Assets/Resources/{0}.prefab";

        	var uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format(assetPatch,assetName));
        	if(uiPrefab != null)
        	{
        		var copyGo = GameObject.Instantiate(uiPrefab);
        		var index = 1;
        		var projectPath = Application.dataPath.Replace("Assets","");
        		while(true)
        		{
        			if(!File.Exists(projectPath + string.Format(assetPatch,assetName + index.ToString())))
        			{
        				copyGo.GetComponentInChildren<Text>().text = "我是界面"+assetName+index.ToString();
        				PrefabUtility.CreatePrefab(string.Format(assetPatch,assetName + index.ToString()), copyGo);
        				Debug.LogFormat(assetName + index.ToString());
        				Debug.LogFormat("wtf 界面创建成功=================================");
        				break;
        			}
        			index ++;
        		}

        		// GameObject.DestroyImmediate(copyGo);
        	}

        	AssetDatabase.SaveAssets();//保存修改
 
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/生成映射资源文件")]
        public static void CreateAssetName2Path()
        {
            var tool = new AssetNameBuildList();
            tool.GetBuildList(true);
        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
	    static void OnScriptReload()
	    {
	    	Debug.Log("脚本编译完毕");
            CreateAssetName2Path();
	    }
    }
 
}