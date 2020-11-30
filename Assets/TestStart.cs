using System.Collections;
using System.Collections.Generic;
using UnityEngine;          

public class TestStart : MonoBehaviour
{
	[SerializeField] public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        // var prefab = Resources.Load("hh/TestPrefab");
        // var go = GameObject.Instantiate(prefab);

        var loader = new AssetNameLoader();
        loader.LoadAssetText();

        var data = loader.GetAssetMapInfo("TestPrefab");
        Debug.LogFormat("wtf data ================= {0}",data.assetPath);
        var prefab = Resources.Load(data.assetPath);
        
        var go = GameObject.Instantiate(prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
