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
    public int CaseId;
    public string CaseName;
    public int TodoCount;
    public string Todo1;
    public string Todo2;
    public string Todo3;

    //数据填充
    public override DataBase GetData()
    {
    	this.Id = GetInt("id");
    	this.State = GetInt("state");
    	this.StageId = GetInt("stageid");
    	this.Content = GetString("content");
        this.Des = GetString("des");
        this.StageName = GetString("stagename");
        this.CaseId = GetInt("caseid");
        this.CaseName = GetString("casename");
        this.TodoCount = GetInt("todocount");
        this.Todo1 = GetString("todo1");
        this.Todo2 = GetString("todo2");
    	this.Todo3 = GetString("todo3");

    	return (DataBase)this;
    }
}
