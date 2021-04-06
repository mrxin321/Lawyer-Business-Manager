using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class AssertBundleCreator  {

  [MenuItem("AssetsBundles/BuildAndroid AssetBundles")]
  static void CreateAndroidAssertBundle(){
    string assetBundleDirectory = "Assets/AssetBundles/Android";
    if(!Directory.Exists(assetBundleDirectory)){
        Directory.CreateDirectory(assetBundleDirectory);
    }

    BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
  }


  [MenuItem("AssetsBundles/BuildiOS AssetBundles")]
  static void CreateiOSAssertBundle()
  {

    string assetBundleDirectory = "Assets/AssetBundles/iOS";
    if (!Directory.Exists(assetBundleDirectory))
    {
        Directory.CreateDirectory(assetBundleDirectory);
    }

    BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.iOS);
  }

  [MenuItem("AssetsBundles/CreateAssetsBundles")]
    public static void CreateBundles(){
        string path = "AssetBundles";
        if(!Directory.Exists(path)){
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}