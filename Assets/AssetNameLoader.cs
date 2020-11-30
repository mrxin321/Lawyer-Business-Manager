using System.Collections.Generic;
using UnityEngine;

public class AssetInfo
{
    public string assetName;
    public string assetPath;
    public string bundleName;
    public string type;
}

public class AssetNameLoader
{
    private Dictionary<string, AssetInfo> _assetTextCache = new Dictionary<string, AssetInfo>();
    private Dictionary<string, List< AssetInfo>> _bundleListCache = new Dictionary<string, List< AssetInfo>>();

    public AssetInfo GetAssetMapInfo(string assetName)
    {
        assetName = assetName.ToLower();
        AssetInfo mapInfo;
        _assetTextCache.TryGetValue(assetName, out mapInfo);
        return mapInfo;
    }
    
    public List<AssetInfo> GetBundleMapInfo(string bundleName)
    {
        List<AssetInfo> listInfo = new List<AssetInfo>();
        _bundleListCache.TryGetValue(bundleName, out listInfo);
        return listInfo;
    }

    public void LoadAssetText()
    {
        TextAsset asset = Resources.Load<TextAsset>(BuildConfig.AssetTxtLoadName);
        if (asset != null)
        {
            Read(asset.text);
            Debug.Log("asset.txt loaded ");
        }
        else
        {
            Debug.LogWarning("Failed to load asset.txt");
        }
    }

    private void Read(string data)
    {
        data = data.Replace("\r\n", "\n");
        string[] lines = data.Split('\n');
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line)) 
            {
                
                var ary = line.Split(':');
                if (ary != null && ary.Length == 4)
                {
                    string assetName = ary[0];
                    string type = ary[1];
                    string assetPath = ary[2];
                    string bundleName = ary[3];

                    var map = new AssetInfo();
                    map.assetName = assetName;
                    map.type = type;
                    map.assetPath = assetPath.Replace("."+type,"");
                    map.bundleName = bundleName.ToLower();
                    //EDebug.Log(assetName + " , " + bundleName);
                    if(!_assetTextCache.ContainsKey(assetName))
                        _assetTextCache.Add(assetName, map);

                    if (_bundleListCache.ContainsKey(map.bundleName))
                    {
                        _bundleListCache[map.bundleName].Add(map);
                    }
                    else
                    {
                        var list = new List<AssetInfo> {map};
                        _bundleListCache.Add(map.bundleName, list);
                    }
                }
            }
        }
    }
}

