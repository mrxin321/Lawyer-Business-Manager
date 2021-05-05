using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class CaseTypeNumData : DataBase
{
    public int Id;
    public int Num1;
    public int Num2;
    public int Num3;
    public int Num4;
    public int Num5;
    public int Num6;
    

    //数据填充
    public override DataBase GetData()
    {
    	this.Id = GetInt("id");
        this.Num1 = GetInt("num1");
        this.Num2 = GetInt("num2");
        this.Num3 = GetInt("num3");
        this.Num4 = GetInt("num4");
        this.Num5 = GetInt("num5");
    	this.Num6 = GetInt("num6");

    	return (DataBase)this;
    }

    public int GetNum(int index)
    {
        switch(index)
        {
            case 1:
                return Num1;
            case 2:
                return Num2;
            case 3:
                return Num3;
            case 4:
                return Num4;
            case 5:
                return Num5;
            case 6:
                return Num6;
        }
        return 0;
    }
}
