using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Assets.CustomAssets.Scripts.Foundation.Common;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.CustomAssets.Scripts.Tools.Sqlite
{
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


        private SqliteDataReader ExecuteReader(string sqlString,bool isTransaction = false)
        {
            if (dbConnection == null)
            {
                EDebug.LogError("=================数据库文件还没开启，请先开启");
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
                    EDebug.LogError(" ============ Sqlite ExecuteQuery Error : " + e.Message);
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

            if (!isExistTable(tableName)) return null;
           

            //dbConnection.Close();
            //}
            //CloseDB();
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
                EDebug.LogError("=================数据库文件还没开启，请先开启");
                return null;
            }
            if (colNames.Length != colTypes.Length)
            {
                EDebug.LogError("=================colNames And colTypes 长度不一致，无法创建数据表");
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
            if(dataReader != null)
                return dataReader.Read();

            return false;
        }



        private string _curDbpath = string.Empty;
        public void OpenDBFile(string dbFile)
        {
            _curDbpath = Application.persistentDataPath + "/" + dbFile + ".db";
            FileInfo fileInfo = new FileInfo (_curDbpath);
            if (!fileInfo.Exists)
            {
                _isNeedCreateDB = true;
                FileStream fileStream = fileInfo.Create();
                fileStream.Close();
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
                dbConnection.Close();
                dbConnection = null;
            }
        }

        private bool _isNeedCreateDB = false;
        /// <summary>
        /// 是否需要创建DB
        /// </summary>
        public bool IsNeedCreateDB => _isNeedCreateDB;

        public void initDB(string dbFile)
        {
            OpenDBFile(dbFile);

            CreateTable(SqlConfig.MailTable, SqlConfig.MailTableBlock);

        }


        public SqliteDataReader InsertValue(string tableName,string[] colName,Hashtable paramTable)
        {
            
            if (colName.Length != paramTable.Count)
            {
                EDebug.LogError("=================colNames And paramTable 长度不一致");
                return null;
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
            string addSqlStr = string.Format("INSERT INTO {0} ({1}) VALUES({2})", tableName,colStr,paramStr);
            return ExecuteQuery(tableName,addSqlStr);
        }


        public SqliteDataReader SelectAllParam(string tableName)
        {
            string selectSqlStr = String.Format("SELECT * FROM {0}",tableName);
            return ExecuteQuery(tableName,selectSqlStr);
        }
        
        
        

    }
}
