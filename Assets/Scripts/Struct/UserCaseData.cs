using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class UserCaseData : DataBase
{
    public int Id;
    public int CaseId;
    public int UserId;
    public string UserName;

    //数据填充
    public override DataBase GetData()
    {
    	this.Id = GetInt("id");
        this.CaseId = GetInt("caseid");
    	this.UserId = GetInt("userid");
    	this.UserName = GetString("name");

    	return (DataBase)this;
    }
}
