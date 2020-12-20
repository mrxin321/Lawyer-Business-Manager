using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class StageData : DataBase
{
    public int Id;
    public int CaseId;
    public string Name;
    public string Des;
    public int Temp;

    //数据填充
    public override DataBase GetData()
    {
        this.Id = GetInt("id");
    	this.CaseId = GetInt("caseid");
    	this.Name = GetString("name");
        this.Des = GetString("des");
    	this.Temp = GetInt("temp");

    	return (DataBase)this;
    }
}
