using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    private static Dictionary<string,Object> AssetCache = new Dictionary<string,Object>();

    public static T LoadAsset<T>(string name) where T : Object 
    {
    	return Resources.Load<T>(name);
    }
    public static GameObject CreatePrefab(string prefabName,Transform parent = null)
    {
        var data = AssetNameLoader.Instance.GetAssetMapInfo(prefabName);
        if(data != null)
        {
            if(!AssetCache.ContainsKey(data.assetPath))
            {
                var asset = LoadAsset<GameObject>(data.assetPath);
                AssetCache.Add(data.assetPath,asset);
            }
            if(AssetCache.ContainsKey(data.assetPath) && AssetCache[data.assetPath] != null)
            {
                var prefab =  GameObject.Instantiate((GameObject)AssetCache[data.assetPath]);
                if(parent != null)
                {
                    prefab.transform.SetParent(parent.transform);
                }
                return prefab;
            }

            
        }
    	
    	return null;
    }	

    public static T AddChildAsLastSibling<T>(T prefab, Transform parent)where T : Component
    {
        var child = GameObject.Instantiate(prefab);
        child.transform.SetParent(parent,false);
        return child;
    }

       
}
