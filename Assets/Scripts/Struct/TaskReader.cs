using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class TaskReader
{
    // public List<TaskData> GetDataList()
    // {
    //     var userId = PlayerDataManager.Instance.GetUserId();
    //     var dataReader = SqliteManager.Instance.SelectParam('case',"master",userId.ToString());
    //     var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","des","content","name","master","contractid","customerid");
    //     return dataList;
    // }

    public static List<TaskData> GetToDoList(int userId)
    {
    	var sqlStr = @"SELECT * from task inner join (SELECT stage.id as findid,'case'.id as caseid,stage.name as stagename,'case'.name as casename FROM stage inner join 'case' on stage.caseid = 'case'.Id where 'case'.master = 3) where stageid = findid and state < 2 group by caseid";
    	sqlStr = string.Format(sqlStr,userId);

        var dataReader = SqliteManager.Instance.SelectParam("task",sqlStr);

        var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","content","des","stageid","state","casename","stagename");

        return dataList;
    }
}
