using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CustomAssets.Scripts.Foundation.Common;
using Com.Happycreation.Protocol;
using Google.Protobuf;
using Mono.Data.Sqlite;
using UnityEngine;



namespace Assets.CustomAssets.Scripts.Tools.Sqlite
{
    [Serializable]
    internal class MailData
    {
        [SerializeField]
        private byte[] _bytes;

        public byte[] MailInfoBytes
        {
            get { return _bytes; }
        }
        public MailData(byte[] bytes)
        {
            this._bytes = bytes;
        }
        
    }
    
    public class MailSqliteManager
    {
        
        private static MailSqliteManager _instance;

        public static MailSqliteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MailSqliteManager(); 
                }

                return _instance;
            }
        }
        
        private MailSqliteManager(){}

        private StringBuilder mulSqlStrBuilder = new StringBuilder();
        public void AddMail(ProtoMailInfo mailInfo)
        {
            if (mailInfo == null || mailInfo.Id == 0) return;
            string mailId ="'"+ mailInfo.Id.ToString()+"'";
            int mailType = (int) mailInfo.MailType;
            long createTime = mailInfo.CreateTime;
            byte[] bytes = mailInfo.ToByteArray();
            
            MailData mailData = new MailData(bytes);
            string content = "'"+ JsonUtility.ToJson(mailData)+"'";
            string sqlStr = string.Format("REPLACE INTO {0} (mailId,mailType,createTime,content) VALUES({1},{2},{3},{4})",
                SqlConfig.MailTable, mailId, mailType,(int)createTime,content);
            SqliteManager.Instance.ExecuteQuery(SqlConfig.MailTable,sqlStr);
        }

        private int maxLength = 420;
        List<long> mailIdList = new List<long>();
        public void AddMulMail(List<ProtoMailInfo> mailInfos)
        {
            if (mailInfos.Count == 0) return;
            mailIdList.Clear();
            for (int i = 0; i < mailInfos.Count; i++)
            {
                mailIdList.Add(mailInfos[i].Id);
            }
            DeleteMailByMailIdList(mailIdList);
            int addCount = Mathf.Min(mailInfos.Count, maxLength);
            
            for (int i = 0; i < addCount; i++)
            {
                ProtoMailInfo mailInfo = mailInfos[i];
                string mailId = mailInfo.Id.ToString();
                int mailType = (int) mailInfo.MailType;
                long createTime = mailInfo.CreateTime;
                byte[] bytes = mailInfo.ToByteArray();
                MailData mailData = new MailData(bytes);
                string content = JsonUtility.ToJson(mailData);
                mulSqlStrBuilder.Append(string.Format("('{0}',{1},{2},'{3}'",mailId,mailType,(int)createTime,content));
                string sign = i == addCount - 1 ? ")":"),";
                mulSqlStrBuilder.Append(sign);
            }

            string sqlStr = string.Format("INSERT INTO {0} (mailId,mailType,createTime,content) VALUES{1}", SqlConfig.MailTable,mulSqlStrBuilder.ToString());
            SqliteDataReader sqliteDataReader = SqliteManager.Instance.ExecuteQuery(SqlConfig.MailTable,sqlStr,true);
            mulSqlStrBuilder.Clear();
            if (mailInfos.Count > maxLength)
            {
                List<ProtoMailInfo> newMailInfos =new List<ProtoMailInfo>();
                for (int i = maxLength; i < mailInfos.Count; i++)
                {
                    ProtoMailInfo mailInfo = mailInfos[i];
                    newMailInfos.Add(mailInfo);
                }
                
                AddMulMail(newMailInfos);
            }
        }

        public ProtoMailInfo GetMailByMailId(long mailId)
        {
            if (mailId == 0) return null;
            string sqlStr = string.Format("SELECT id,content FROM {0} WHERE mailId = {1} LIMIT 1",SqlConfig.MailTable,mailId.ToString());
            SqliteDataReader dataReader = SqliteManager.Instance.ExecuteQuery(SqlConfig.MailTable,sqlStr);
            if (dataReader.Read())
            {
               return GetMailInfo(dataReader);
            }

            return null;
        }
        
        public Dictionary<long,ProtoMailInfo> SelectAllMail()
        {
            SqliteDataReader dataReader =  SqliteManager.Instance.SelectAllParam(SqlConfig.MailTable);
            if (dataReader != null)
            {
                long maxMailId = 0;
                ProtoMailInfo testMailInfo = null;
                Dictionary<long,ProtoMailInfo> newMailInfos = new Dictionary<long, ProtoMailInfo>();
                while ( dataReader.Read())
                {
                    long mailId = long.Parse(dataReader.GetString(dataReader.GetOrdinal("mailId")));

                    ProtoMailInfo info = GetMailInfo(dataReader);
                    //if(!newMailInfos.TryGetValue(mailId,out ProtoMailInfo protoMailInfo))
                    newMailInfos.Add(mailId,info);
                }
                dataReader.Close();
                return newMailInfos;
            }
            return null;
        }


        public void DeleteMailByMailId(long mailId)
        {
            if (mailId == 0) return;
            string sqlStr = string.Format("DELETE FROM {0} WHERE mailId = '{1}'",SqlConfig.MailTable,mailId.ToString());
            SqliteManager.Instance.ExecuteQuery(SqlConfig.MailTable,sqlStr);
        }
        
        
        public StringBuilder mailIdList_StrBuider = new StringBuilder();
        public void DeleteMailByMailIdList(List<long> mailIdList)
        {
            if (mailIdList.Count == 0) return;
            mailIdList_StrBuider.Clear();
            for (int i = 0; i < mailIdList.Count; i++)
            {
                mailIdList_StrBuider.Append(mailIdList[i].ToString());
                string sign = i == mailIdList.Count - 1 ? "" : ",";
                mailIdList_StrBuider.Append(sign);
            }
            string sqlStr = string.Format("DELETE FROM {0} WHERE mailId IN({1})",SqlConfig.MailTable,mailIdList_StrBuider.ToString());
            SqliteManager.Instance.ExecuteQuery(SqlConfig.MailTable,sqlStr);
        }
        
        public void DeleteAllMail()
        {
            
            string sqlStr =  string.Format("DELETE FROM sqlite_sequence WHERE name = '{0}'",SqlConfig.MailTable);
            SqliteDataReader dataReader = SqliteManager.Instance.ExecuteQuery(SqlConfig.MailTable,sqlStr);
        }

        private ProtoMailInfo GetMailInfo(SqliteDataReader dataReader)
        {
            string content = dataReader.GetString(dataReader.GetOrdinal("content"));
            MailData mailData = JsonUtility.FromJson<MailData>(content);
            byte[] bytes = mailData.MailInfoBytes;
            ProtoMailInfo info = ProtoMailInfo.Parser.ParseFrom(bytes);
            return info;
        }
    }
}
