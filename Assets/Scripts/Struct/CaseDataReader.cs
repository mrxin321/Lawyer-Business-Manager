using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class CaseDataReader
{
    public static List<CaseData> GetMyDataList()
    {
        var userId = PlayerDataManager.Instance.GetUserId();
        var sql = "select * from 'case' join usercase on 'case'.id  = usercase.caseid where userid = {0}";
        var dataReader = SqliteManager.Instance.SelectParam("case",string.Format(sql,userId));
        var dataList = DataBase.GetDataList<CaseData>(dataReader,"id","mask","name","contractid");
        return dataList;
    }
}
