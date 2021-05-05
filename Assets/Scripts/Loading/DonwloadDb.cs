using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;


public class DonwloadDb : LoadingItemBase
{
    public override string GetLoadingTips()
    {
        return "加载中 {0}%";
    }

    public override float GetLoadingProgress()
    {
        return LoadingProgress;   
    }

    public override void StartProgress()
    {
        OneFrameProgress = LoadingRate / ExpectTime / 60f;
        LoadingProgress = 0;

        StartCoroutine(StartDownLoad());
    }

    private void Update()
    {
        if(LoadingProgress + OneFrameProgress >= LoadingRate)return;

        LoadingProgress += OneFrameProgress;
    }

    public override bool IsLoadingFinish()
    {
        return LoadingFinish;
    }

    private IEnumerator StartDownLoad()
    {
        string Url = "http://120.78.87.126:8080/download/db";
 
        //发送请求
        UnityWebRequest webRequest = UnityWebRequest.Get(Url);
 
        yield return webRequest.SendWebRequest();

        //获取二进制数据
        Debug.LogFormat("wtf 下载完成 ==================");
        var file= webRequest.downloadHandler.data;
            
        #if UNITY_EDITOR
            SqlConfig.myDB = "lbmdb" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
        #endif

        string fileName = Application.persistentDataPath + "/" + SqlConfig.myDB + ".db";// @"C:/Users/xin/AppData/LocalLow/DefaultCompany/Lawyer Business Manager" + "lbmdb.db";

        //创建文件写入对象
        FileStream nFile = new FileStream(fileName, FileMode.Create);
    
        //写入数据
        nFile.Write(file, 0, file.Length);
 
        nFile.Close();
        LoadingProgress = LoadingRate;
        Utility.DoWait(()=>{
            LoadingFinish = true;
        },1f,this);
    }
}
