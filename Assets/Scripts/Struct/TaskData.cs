using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class TaskData : DataBase
{
    public int Id;
    public int State;
    public int StageId;
    public string Content;
    public string Des;
    public string StageName;
    public string CaseName;

    //数据填充
    public override DataBase GetData()
    {
    	this.Id = GetInt("id");
    	this.State = GetInt("state");
    	this.StageId = GetInt("stageid");
    	this.Content = GetString("content");
        this.Des = GetString("des");
        this.StageName = GetString("stagename");
    	this.CaseName = GetString("casename");

    	return (DataBase)this;
    }
}
