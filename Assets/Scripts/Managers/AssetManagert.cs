using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static T LoadAsset<T>(string name) where T : Object 
    {
    	return Resources.Load<T>(name);
    }

    public static GameObject CreatePrefab(string prefabName)
    {
    	var asset = LoadAsset<GameObject>(prefabName);
    	if(asset != null)
    	{
    		return GameObject.Instantiate(asset);
    	}
    	return null;
    }	
}
