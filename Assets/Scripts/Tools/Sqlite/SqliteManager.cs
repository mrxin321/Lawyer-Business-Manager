using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.Assertions;

public  class SqliteManager
{
    private static SqliteManager _instance;

    public static SqliteManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SqliteManager(); 
            }

            return _instance;
        }
    }

    private SqliteManager(){}
    
    
    public  SqliteConnection dbConnection;

    private SqliteCommand dbCommand;


    private SqliteDataReader reader;


    public SqliteDataReader ExecuteReader(string sqlString,bool isTransaction = false)
    {
        if (dbConnection == null)
        {
            Debug.LogError("=================数据库文件还没开启，请先开启");
            return null;
        }
        if (dbConnection.State != ConnectionState.Open) OpenDB();
        
        dbCommand = dbConnection.CreateCommand();
        dbCommand.Connection = dbConnection;
        dbCommand.CommandText = sqlString;
        if (isTransaction)
        {
            SqliteTransaction transaction = dbConnection.BeginTransaction();
            try
            {
                reader = dbCommand.ExecuteReader();
                transaction.Commit();
            }
            catch (Exception e)
            {
                Debug.LogError(" ============ Sqlite ExecuteQuery Error : " + e.Message);
                transaction.Rollback(); //回滚事务
            }
        }
        else
            reader = dbCommand.ExecuteReader();

        return reader;
    }

    /// <summary>
    /// 执行sql 语句
    /// </summary>
    /// <param name="sqlString"> sql 语句</param>
    /// <returns></returns>
    public SqliteDataReader ExecuteQuery(string tableName,string sqlString,bool isTransaction = false)
    {
        ViewUtils.Print(sqlString);
        // if (!isExistTable(tableName)) return null;
        return ExecuteReader(sqlString,isTransaction);
    }
    
    
    /// <summary>
    /// 创建数据表
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">数据表名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
    public SqliteDataReader CreateTable(string tableName,string[] colNames,string[] colTypes)
    {
        Assert.IsTrue(string.IsNullOrEmpty(tableName),"==============请输入正确的tableName");
        if (dbConnection == null)
        {
            Debug.LogError("=================数据库文件还没开启，请先开启");
            return null;
        }
        if (colNames.Length != colTypes.Length)
        {
            Debug.LogError("=================colNames And colTypes 长度不一致，无法创建数据表");
            return null;
        }
        
        if (isExistTable(tableName)) return null;
        
        string queryString = "CREATE TABLE " + tableName + "( " + colNames [0] + " " + colTypes [0];
        for (int i=1; i<colNames.Length; i++) 
        {
            queryString+=", " + colNames[i] + " " + colTypes[i];
        }
        queryString+= "  ) ";
        return ExecuteQuery(tableName,queryString);
    }
    
    
    public SqliteDataReader CreateTable(string tableName, string createTableBlock)
    {
        createTableBlock = String.Format(createTableBlock,tableName);
        if(!isExistTable(tableName))
            ExecuteReader(createTableBlock);

        return null;
    }

    public bool isExistTable(string tableName)
    {
        string sqlStr = string.Format("SELECT * FROM sqlite_master WHERE type = 'table' AND name = '{0}'",tableName);

        SqliteDataReader dataReader = ExecuteReader(sqlStr);
        if(dataReader != null && dataReader.Read())
        {
            dataReader.Close();
            CloseDB();
            return true;
        }

        return false;
    }



    private string _curDbpath = string.Empty;
    public void OpenDBFile(string dbFile)
    {
        _curDbpath = Application.persistentDataPath + "/" + dbFile + ".db";
        Debug.LogFormat("wtf 数据库位置 {0}",_curDbpath);
        FileInfo fileInfo = new FileInfo (_curDbpath);

        if (!fileInfo.Exists)
        {
            _isNeedCreateDB = true;
            // FileStream fileStream = fileInfo.Create();
            // fileStream.Close();
            //拷贝文件
            File.Copy(Application.streamingAssetsPath + "/" + dbFile + ".db", _curDbpath);
            ViewUtils.Print("wtf 初始初始化数据库 拷贝本地数据到用户目录");
        }

        dbConnection = new SqliteConnection("Data Source=" + _curDbpath);
        OpenDB();
    }

    private void OpenDB()
    {
        if(dbConnection.State == ConnectionState.Open) dbConnection.Close();

        dbConnection.Open();
    }

    public void CloseDB()
    {
        if (dbConnection != null)
        {
            ViewUtils.Print("wtf 关闭数据库连接=======================");
            dbConnection.Close();
        }
    }

    public void CloseDbConnet()
    {
        CloseDB();
        dbConnection.Dispose();
        dbConnection = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        ViewUtils.Print("wtf 关闭数据库连接");
    }

    private bool _isNeedCreateDB = false;
    /// <summary>
    /// 是否需要创建DB
    /// </summary>
    public bool IsNeedCreateDB => _isNeedCreateDB;

    public void initDB(string dbFile)
    {
        OpenDBFile(dbFile);

        // CreateTable(SqlConfig.MailTable, SqlConfig.MailTableBlock);

    }


    public int InsertValue(string tableName,string[] colName,Hashtable paramTable)
    {
        if (colName.Length != paramTable.Count)
        {
            Debug.LogError("=================colNames And paramTable 长度不一致");
            return -1;
        }

        string colStr = "";
        for (int i = 0; i < colName.Length; i++)
        {
            colStr += colName[i];
            if (i < colName.Length - 1)
            {
                colStr += ",";
            }
        }

        string paramStr = "";
        for (int i = 0; i < paramTable.Count; i++)
        {
            if (paramTable[i] is string)
            {
                paramTable[i] = "'"+paramTable[i]+"'";
            }
            paramStr += paramTable[i];
            if (i < paramTable.Count - 1)
            {
                paramStr += ",";
            }
        }
        string addSqlStr = string.Format("INSERT INTO '{0}' ({1}) VALUES({2})", tableName,colStr,paramStr);
        var reader = ExecuteQuery(tableName,addSqlStr);
        reader.Close();

        var id = SqliteManager.Instance.GetLastInsertId(tableName);

        CloseDB();
        return id;
    }

    public int GetLastInsertId(string tableName)
    {
        if(tableName == "case")tableName = string.Format("'{0}'",tableName);
        
        var insertId = -1;

        var sql = "select last_insert_rowid() as id from " + tableName ;
        var reader = SelectParam(tableName,sql);
        while(reader != null && reader.Read())
        {
            insertId = reader.GetInt32(reader.GetOrdinal("id"));
            if(insertId > 0)
            {
                reader.Close();
                CloseDB();
                return insertId;
            }
            ViewUtils.Print("wtf last id is {0}",insertId);
        }
        reader.Close();
        CloseDB();
        return insertId;
    }

    public SqliteDataReader UpateValue(string tableName,string[] colName,Hashtable paramTable)
    {
        
        if (colName.Length != paramTable.Count)
        {
            Debug.LogError("参数不对");
            return null;
        }

        string sqlStr = "";
        for (int i = 1; i < colName.Length; i++)
        {
            sqlStr += colName[i] + " = ";
            

            if (paramTable[i] is string)
            {
               sqlStr += "'"+paramTable[i]+"'";
            }else
            {
                sqlStr += paramTable[i];
            }

            if (i < colName.Length - 1)
            {
                sqlStr += ",";
            }
        }

        string addSqlStr = string.Format("Update '{0}' set {1} where id = {2}",tableName,sqlStr,paramTable[0]);
        var reader = ExecuteQuery(tableName,addSqlStr);
        reader.Close();
        CloseDB();
        return null;
    }
    //所有选择
    public SqliteDataReader SelectAllParam(string tableName)
    {
        string selectSqlStr = String.Format("SELECT * FROM '{0}'",tableName);
        return ExecuteQuery(tableName,selectSqlStr);
    }

    //条件查询        
    public SqliteDataReader SelectParam(string tableName,string param,string value)
    {
        string selectSqlStr = String.Format("SELECT * FROM '{0}' where {1} = {2}",tableName,param,value);
        return ExecuteQuery(tableName,selectSqlStr);
    }

    public SqliteDataReader SelectParam(string tableName,string selectSqlStrs)
    {
        return ExecuteQuery(tableName,selectSqlStrs);
    }

    public SqliteDataReader DeleteRecord(string tableName,string calname,Hashtable paramTable)
    {
        var str = paramTable[0] is string?"'"+paramTable[0]+"'":paramTable[0];
        string selectSqlStr = String.Format("Delete from '{0}' where {1} = {2}",tableName,calname,str);
        var reader = ExecuteQuery(tableName,selectSqlStr);
        reader.Close();
        CloseDB();
        return null;
    }

    public SqliteDataReader DeleteRecord(string tableName,string deleteSql)
    {
        var reader = ExecuteQuery(tableName,deleteSql);
        reader.Close();
        CloseDB();
        return null;
    }
}
