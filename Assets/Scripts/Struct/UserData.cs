using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class UserData : DataBase
{
    public int Id;
    public string Name;
    public string Password;

    //数据填充
    public override DataBase GetData()
    {
    	this.Id = GetInt("id");
    	this.Name = GetString("account");
    	this.Password = GetString("password");

    	return (DataBase)this;
    }
}
