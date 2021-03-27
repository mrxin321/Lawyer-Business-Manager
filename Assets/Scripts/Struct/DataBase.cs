using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class DataBase
{
	public Dictionary<string,string> DataDic;

	protected DataBase()
	{}

	public void GetDataBase(SqliteDataReader reader,params string[] args)
	{
		DataDic = new Dictionary<string,string>();
		if(reader != null)
		{
			foreach(var paramid in args)
			{	
				DataDic.Add(paramid,reader[paramid].ToString());
			}
		}
	}
	
	public int GetInt(string paramId)
	{
		var intData = 0;

		if(DataDic != null && DataDic.ContainsKey(paramId))
		{
			int.TryParse(DataDic[paramId],out intData);
		}
		return intData;
	}

	public string GetString(string paramId)
	{
		if(DataDic != null && DataDic.ContainsKey(paramId))
		{
			return DataDic[paramId];
		}
		return "";
	}

	public T GetTypeData<T>(SqliteDataReader reader,params string[] args) where T : DataBase
	{
		reader.Read();
    	GetDataBase(reader,args);
		var data = GetData();
		reader.Close();
		SqliteManager.Instance.CloseDB();
		return data as T;
	}

	public virtual DataBase GetData()
	{
		return null;
	}

	public static List<T> GetDataList<T>(SqliteDataReader reader,params string[] args) where T : DataBase
    {
    	var dataList = new List<T>();
    	while(reader != null && reader.Read())
    	{
    		var data = GetNewInstance<T>();
    		data.GetDataBase(reader,args);
    		data.GetData();
    		dataList.Add(data);
    	}
    	reader.Close();
    	SqliteManager.Instance.CloseDB();
    	return dataList;
    }

    public static T GetNewInstance<T>() where T : DataBase
    {
    	DataBase data = new DataBase();
    	switch(typeof(T).FullName)
    	{
    		case "TaskData":
    			data = new TaskData();
    			break;
    		case "CaseData":
    			data = new CaseData();
    			break;
    		case "StageData":
    			data = new StageData();
    			break;
    		case "CaseTypeData":
    			data = new CaseTypeData();
    			break;
    		case "UserData":
    			data = new UserData();
    			break;
    		case "PayTypeData":
    			data = new PayTypeData();
    			break;
    		case "UserCaseData":
    			data = new UserCaseData();
    			break;
    	}
    	return data as T;;
    }
}
