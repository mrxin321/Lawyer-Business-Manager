using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqlConfig
{

    public static string myDB = "slggamedb";
    
    public const string testTableBlock =
        "CREATE TABLE {0}(id integer primary key autoincrement unique not null,testId integer,content text,state integer)";


    public const string MailTableBlock =
        "CREATE TABLE {0}(id integer primary key autoincrement unique not null,mailId text unique,content text,state integer,mailtype integer,createTime integer,other text)";




//    public const string ReportMail = "ReportMail";
//    public const string AllianceMail = "AllianceMail";
//    public const string SystemMail = "SystemMail";
//    public const string MarkMail = "MarkMail";
//    public const string SelfSendMail = "SelfSendMail";
//    public const string SelfReceiveMail = "SelfReceiveMail";

    public const string MailTable = "MailTable";


}
