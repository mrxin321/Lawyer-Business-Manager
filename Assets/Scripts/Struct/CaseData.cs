using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class CaseData : DataBase
{
    public int Id;
    public string Name;
    public int Master;
    public int CaseType;
    public string ContractId;
    public string Customer;
    public string Content;
    public string Mask;
    public string Plaintiff;
    public string Defendant;
    public string Other;
    public string Institution;
    public int Paytype;
    public string Money;
    public string Paydes;
    public string Createtime;

    //数据填充
    public override DataBase GetData()
    {
    	this.Id = GetInt("id");
    	this.Name = GetString("name");
    	this.Master = GetInt("master");
        this.ContractId = GetString("contractid");
    	this.Customer = GetString("customer");
        this.Content = GetString("content");
        this.Mask = GetString("mask");
        this.CaseType = GetInt("casetype");
        this.Plaintiff = GetString("plaintiff");
        this.Defendant = GetString("defendant");
        this.Other = GetString("other");
        this.Institution = GetString("institution");
        this.Paytype = GetInt("paytype");
        this.Money = GetString("money");
        this.Paydes = GetString("paydes");
        this.Createtime = GetString("createtime");

    	return (DataBase)this;
    }
}
