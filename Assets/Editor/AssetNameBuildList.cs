using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;

public class AssetNameBuildList
{
    private Dictionary<string, bool> _assetUsedMap;
    private string _assetfilePath;
    private const string _extend = ".resource";
    private bool _createAssetFile;
    public AssetBundleBuild[] GetBuildList(bool createAssetFile,string filePath="")
    {
        _createAssetFile = createAssetFile;
        
        _assetfilePath = Path.Combine(filePath==""?BuildConfig.ResourcesPath:filePath, BuildConfig.AssetTxtName);

        if (File.Exists(_assetfilePath) && _createAssetFile)
        {
            File.Delete(_assetfilePath);
            AssetDatabase.Refresh();
        }

        List<AssetBundleBuild> _buildlist = new List<AssetBundleBuild>();
        _assetUsedMap = new Dictionary<string, bool>();
        
       
        SetUIPrefab(_buildlist);

        AssetDatabase.Refresh();

        return _buildlist.ToArray();
    }

    private void SetUIPrefab(List<AssetBundleBuild> buildlist)
    {
        SetOneFolderOneBundle(buildlist, BuildConfig.UIPrefabPath, BuildConfig.ResourcesFolder, "t:Prefab", true, true);
    }

    private void SetTxt(List<AssetBundleBuild> buildlist)
    {
        SetOneFolderOneBundle(buildlist, BuildConfig.UIPrefabPath, BuildConfig.ResourcesFolder, "t:sql", true, true);
    }

    private void SetOneFolderOneBundle(List<AssetBundleBuild> buildlist, string path, string splitTag, string filter,bool needWrite, bool buildDependencies, bool selfFolder = false)
    {
        string[] folders = null;
        if (selfFolder)
            folders = new string[1] { path };
        else
            folders = AssetDatabase.GetSubFolders(path);

        foreach (var folder in folders)
        {
            var localFolder = folder.Replace("\\", "/");

            var bundleNameNoExtend = localFolder.Split(new string[] { splitTag }, StringSplitOptions.None)[0];
            bundleNameNoExtend = bundleNameNoExtend.Replace("/","___");
            var bundleName = bundleNameNoExtend + _extend;
            var bundleNameForDep = bundleNameNoExtend + "_dep" + _extend;
            //EDebug.Log("----->>>>" + bundleName);

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;

            List<string> assetList = new List<string>();
            var assets = AssetDatabase.FindAssets(filter, new[] { folder });
            foreach (var assetGuid in assets)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                //EDebug.Log(assetPath);
                PushToAssetNames(assetList, assetPath, bundleName,needWrite);
            }

            if(assetList.Count > 0)
            {
                build.assetNames = assetList.ToArray();

                buildlist.Add(build);

                if (buildDependencies)
                {
                    SetDependenciesBundle(buildlist, bundleNameForDep, assetList.ToArray());
                }
            }
        }
    }

    private void SetDependenciesBundle(List<AssetBundleBuild> buildlist,string bundleName, string[] assetPaths)
    {
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = bundleName;

        List<string> assetList = new List<string>();
        foreach(var assetPath in assetPaths)
        {
            var dependencies = AssetDatabase.GetDependencies(assetPath, false);
            foreach (var dependency in dependencies)
            {
                //EDebug.Log("dependency : " + dependency);
                bool needWrite = false;

                if(dependency.Replace("\\", "/").Contains(BuildConfig.ResourcesFolder))
                {
                    needWrite = true;
                }


                PushToAssetNames(assetList,dependency,bundleName,needWrite);
            }
        }

        if(assetList.Count > 0)
        {
            build.assetNames = assetList.ToArray();
            buildlist.Add(build);
        }
    }

    private void PushToAssetNames(List<string> assetList, string assetPath, string bundleName,bool needWrite)
    {

        if(!assetPath.EndsWith(".cs", StringComparison.Ordinal)
           && !assetPath.EndsWith(".js", StringComparison.Ordinal)
           && !assetPath.EndsWith(".meta", StringComparison.Ordinal)
          )
        {
            if (!_assetUsedMap.ContainsKey(assetPath))
            {
                //EDebug.Log(assetPath);
                assetList.Add(assetPath);
                if(needWrite)
                {
                    WriteToTextFile(assetPath, bundleName);
                }
                _assetUsedMap.Add(assetPath, true);
            }
        }
    }


    private void WriteToTextFile(string assetPath,string bundleName)
    {
        if (_createAssetFile == false)
            return;

        int startIndex = assetPath.LastIndexOf("/", StringComparison.Ordinal) + 1;

        string fileName = assetPath.Substring(startIndex);

        //EDebug.Log(fileName);

        var fileAry = fileName.Split(new string[] { "." }, StringSplitOptions.None);

        string assetName = fileAry[0];
        string extension = "";
        if(fileAry.Length >= 2)extension = fileAry[1];

        assetPath = assetPath.Replace("Assets/Resources/","");

        var assetLineStr = assetName.ToLower() + ":" + extension + ":" + assetPath + ":" + bundleName.ToLower() + "\n";

        WriteToFile(_assetfilePath,assetLineStr);
    }
    public static List<string> GetAllFilePath(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        return _GetAllFilePath(dir);
    }

    private static List<string> _GetAllFilePath(DirectoryInfo dir)
    {
        List<string> FileList = new List<string>();

        FileInfo[] allFile = dir.GetFiles();
        foreach (FileInfo fi in allFile)
        {
            if (fi.Extension.ToLower() != ".meta" && !fi.Extension.Equals(".DS_Store") && fi.Extension.ToLower() != ".cs")
            {
                FileList.Add(fi.FullName);
            }
        }

        DirectoryInfo[] allDir = dir.GetDirectories();
        foreach (DirectoryInfo d in allDir)
        {
            FileList.AddRange(_GetAllFilePath(d));
        }
        return FileList;
    }

    public static void WriteToFile(string filePath, string content)
    {
        using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
        {
            byte[] byTotalText = Encoding.UTF8.GetBytes(content);
            fs.Position = fs.Length;
            fs.Write(byTotalText, 0, byTotalText.Length);
        }
    }
}

