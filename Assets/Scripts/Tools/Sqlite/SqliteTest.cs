using System;
using System.Collections;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class SqliteTest : MonoBehaviour
{
    // Start is called before the first frame update
    private SqliteManager _sqliteManager;

    private string tableName = "test";

    public Text contentText;

    public Button btn_Insert;
    public Button btn_Print;
    
    public Button btn_Delete;

    private void Awake()
    {
        _sqliteManager = SqliteManager.Instance;
        _sqliteManager.OpenDBFile(SqlConfig.myDB+"1");

        _sqliteManager.CreateTable(tableName, SqlConfig.testTableBlock);
    }

    void Start()
    {
        

        // btn_Insert.onClick.AddListener(delegate()
        // {
        //     string addContent = DateTime.Now.ToString();
        //     Hashtable hashtable = new Hashtable();
        //     hashtable.Add(0,Random.Range(1,100));
        //     hashtable.Add(1,addContent);
        //     hashtable.Add(2,0);
        //     _sqliteManager.InsertValue(tableName,new string[]{"testId","content","state"},hashtable);
        // });
        
//         btn_Print.onClick.AddListener(delegate
//         {
//             SqliteDataReader dataReader= _sqliteManager.SelectAllParam(-);
            
//             string showContent = "";
//             while(dataReader != null && dataReader.Read()) 
//             {
//                 //读取ID
// //                Debug.Log(dataReader.GetInt32(dataReader.GetOrdinal("testId")));
// //                //读取Name
// //                Debug.Log(dataReader.GetString(dataReader.GetOrdinal("content")));
// //                Debug.Log(dataReader.GetInt32(dataReader.GetOrdinal("state")));
//                 int msgId = dataReader.GetInt32(dataReader.GetOrdinal("id"));
//                 int testId = dataReader.GetInt32(dataReader.GetOrdinal("testId"));

//                 string content = dataReader.GetString(dataReader.GetOrdinal("content"));

//                 int state = dataReader.GetInt32(dataReader.GetOrdinal("state"));
//                 showContent += string.Format("(id: {0} testId : {1},content : {2}, state : {3} )",msgId ,testId, content, state) + "\n";
//             }

//             contentText.text =string.IsNullOrEmpty(showContent)?"表中还没有数据":showContent;
//             if(dataReader!=null)
//                 dataReader.Close();
//         });
        
        // btn_Delete.onClick.AddListener(delegate
        // {
        //     string sqlStr = "DELETE FROM test WHERE testId = 10";
        //     _sqliteManager.ExecuteQuery(tableName,sqlStr);
        // });
        
    }

    
    
    
}
