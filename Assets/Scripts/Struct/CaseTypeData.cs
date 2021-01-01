using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class CaseTypeData : DataBase
{
    public int Id;
    public string Name;

    //数据填充
    public override DataBase GetData()
    {
    	this.Id = GetInt("id");
    	this.Name = GetString("name");

    	return (DataBase)this;
    }
}
