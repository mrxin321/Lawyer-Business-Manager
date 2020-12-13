using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class CaseData : DataBase
{
    public int Id;
    public string Name;
    public int Master;
    public int ContractId;
    public int CustomerId;
    public string Content;
    public string Des;

    //数据填充
    public override DataBase GetData()
    {
    	this.Id = GetInt("id");
    	this.Name = GetString("name");
    	this.Master = GetInt("master");
        this.ContractId = GetInt("constractid");
    	this.CustomerId = GetInt("customerid");
        this.Content = GetString("content");
    	this.Des = GetString("des");

    	return (DataBase)this;
    }
}
