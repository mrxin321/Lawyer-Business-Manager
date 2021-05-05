using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UrlPostData
{
    public int Id;
    public int PostStatus =0;           //0未请求 1请求中 2请求成功 3请求失败
    public string Sql = "";
}

public class SqlManager : MonoBehaviour
{
	string ExecuteUrl = "http://120.78.87.126:8080/executedb/{0}";
    string InsertUrl = "http://120.78.87.126:8080/insertdb/{0}";
    string QueryUrl = "http://120.78.87.126:8080/selectdb/{0}";

    public static int PostId = 0;
    public Dictionary<int,UrlPostData> SqlList = new Dictionary<int,UrlPostData>();

    public static SqlManager Instance;

    void Awake()
    {
    	if(Instance == null)Instance = this;
    }

    public void SendExecuteSql(string sql,Action callback = null)
    {
        StartCoroutine(CorSendExecuteSql(sql,callback));
    }

    public IEnumerator CorSendExecuteSql(string sql,Action callback = null)
    {
        string url = string.Format(ExecuteUrl,sql);

        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        
        if( webRequest.responseCode == 200 && webRequest.downloadHandler.text == "ok")
        {
            Utility.SafePostEvent(callback);
        }
    }

    public void SendInsertSql(string sql,Action<int> callback)
    {
    	StartCoroutine(CorSendInsertSql(sql,callback));
    }

    private IEnumerator CorSendInsertSql(string sql,Action<int> callback)
    {
		string url = string.Format(InsertUrl,sql);

    	UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        
        if( webRequest.responseCode == 200 )
        {
        	var insertId = 0;
        	int.TryParse(webRequest.downloadHandler.text, out insertId);
        	Utility.SafePostEvent(callback,insertId);
        }

    }

	void OnDestroy()
	{
		SqliteManager.Instance.CloseDbConnet();
	}
}


/*
    // public void SendExecuteSql(string sql)
    // {
    //     var data = new UrlPostData();
    //     data.PostStatus = 0;
    //     data.Sql = sql;
    //     data.Id = ++PostId; 

    //     SqlList.Add(data.Id,data);
    // }

    // private void Update()
    // {
    //     if(SqlList.Count > 0)
    //     {
    //         foreach(var item in SqlList.Values)
    //         {
    //             if(item.PostStatus == 0 || item.PostStatus == 3)
    //             {
    //                 StartCoroutine(CorSendExecuteSql(item));
    //                 break;
    //             }
    //         }
    //     }
    // }

    // private IEnumerator CorSendExecuteSql(UrlPostData postUrlData)
    // {
    //     string url = string.Format(ExecuteUrl,postUrlData.Sql);
    //     SqlList[postUrlData.Id].PostStatus = 1;
    //     UnityWebRequest webRequest = UnityWebRequest.Get(url);
    //     yield return webRequest.SendWebRequest();

    //     SqlList[postUrlData.Id].PostStatus = webRequest.responseCode == 200?2:3;

    //     if( SqlList[postUrlData.Id].PostStatus == 2)
    //     {
    //      Debug.LogFormat("wtf 数据操作成功 ===============");
    //      SqlList.Remove(postUrlData.Id);
    //     }else
    //     {
       //      Debug.LogErrorFormat("wtf 数据操作失败 ==============={0}",postUrlData.Sql);
    //     }
    // }
*/