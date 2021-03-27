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
    	var sqlStr = @"select task.*,stage.name as stagename,stage.caseid,'case'.name as casename from task join stage on task.stageid = stage.id join 'case' on 'case'.id = stage.caseid join usercase on usercase.caseid = 'case'.id where usercase.userid = {0} and state < 2 ORDER BY id ASC";
    	sqlStr = string.Format(sqlStr,userId);

        var dataReader = SqliteManager.Instance.SelectParam("task",sqlStr);

        var dataList = DataBase.GetDataList<TaskData>(dataReader,"id","content","des","stageid","state","casename","stagename","caseid");

        var list = new Dictionary<int,TaskData>();
        var taskList = new List<TaskData>();
        foreach(var item in dataList)
        {
            if(!list.ContainsKey(item.CaseId))
            {
                list.Add(item.CaseId,item);
                taskList.Add(item);
            }
        }
        return taskList;
    }

    public static List<TaskData> GetTaskListByStageId(int stageId)
    {
        var dataReader = SqliteManager.Instance.SelectParam("task","stageid",stageId.ToString());
        return DataBase.GetDataList<TaskData>(dataReader,"id","des","content","stageid","state","todocount","todo1","todo2","todo3");
    }
}
