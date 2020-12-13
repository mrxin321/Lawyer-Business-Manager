using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class CaseDataReader
{
    public List<CaseData> GetDataList()
    {
        var userId = PlayerDataManager.Instance.GetUserId();
        var dataReader = SqliteManager.Instance.SelectParam("case","master",userId.ToString());
        var dataList = DataBase.GetDataList<CaseData>(dataReader,"id","des","content","name","master","contractid","customerid");
        return dataList;
    }
}
